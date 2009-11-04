///////////////////////////////////////////////////////////
//
// ISEL - LEIC - Programação Concorrente
// October 2007
//
// uthread library:
//     User threads supporting cooperative multithreading.
//     The current version of the library provides:
//        - Threads
//        - Mutexes
//        - Semaphores
//
// Author: Carlos Martins
//    (revised by João Trindade)
// 

#include <stdlib.h>
#include <assert.h>
#include <windows.h>
#include <time.h>

#include "lists.h"
#include "uthread.h"

///////////////////////////////////////////////////////////
//
// uthread_t
//

// Fixed stack size for a user thread.
#define STACK_SIZE (16 * 4096)

// Data structure representing the arrangement of a thread's
// context saved in the stack.
typedef struct uthread_context {
	unsigned int reg_edi;
	unsigned int reg_esi;
	unsigned int reg_ebx;
	unsigned int reg_ebp;
	void      (* ret_addr)();
} uthread_context_t;


// Data structure to hold information about a user thread.
// The first entry must be a dlist_node_t as we use
// intrusive lists to support the ready queue and the wait
// queues in synchronization objects.
typedef struct uthread {
	dlist_node_t        node;        // node data for intrusive lists
	unsigned int        tid;         // unique thread id
	uthread_function_t  function;    // function to be executed
	uthread_argument_t  argument;    // argument to thread function
	unsigned char     * pStackArena; // memory block for the thread's stack
	uthread_context_t * pContext_;   // pointer to context saved in the thread's stack
    unsigned int        waitn;
    unsigned char       type;
    unsigned long       sleep_absolute_time;
    dlist_node_t        joined_thread;
} uthread_t;

///////////////////////////////////////////////////////////
//
// Internal variables supporting the execution of user threads.
//

// Number of existing user threads.
static unsigned int uthread_internal_numThreads;
static uthread_countdownlatch_t foreground_latch;

// The ready queue is an intrusive list of uthread_t instances.
// All the threads which are READY to run are in this queue, including
// the running thread. A thread is selected to run by placing it at the
// head of the ready queue and calling uthread_internal_schedule.
static dlist_t uthread_internal_readyQueue;
static dlist_t uthread_internal_sleepQueue; 

// Reference to the currently running thread.
static uthread_t * uthread_internal_pRunningThread;

// When a thread finishes the execution of its associated function we
// need to release the respective uthread_t instance and stack space.
// However, this can only be done after the thread is switched out of
// the running state. This variable, if not NULL, holds a reference to
// a thread that has terminated and needs to have its resources freed.
static uthread_t * uthread_internal_pZombieThread;
static uthread_t * uthread_cThreads[1024]; //cThreads == Current Threads

///////////////////////////////////////////////////////////
//
// Forward declarations of internal functions supporting the
// execution of user threads.
//

// Frees the resources used by a recently terminated thread.
void uthread_internal_cleanupZombieThread();

// All threads run this function. This allows us to easily intercept
// the beginning and end of the execution of all user threads.
void uthread_internal_start();

// The thread at the head of the ready queue should be the running thread.
// Call uthread_internal_schedule after any changes to the ready queue
// that might alter its head node.
void uthread_internal_schedule();

// Perform a context switch between two user threads.
void __fastcall uthread_internal_contextSwitch(uthread_t * pOutThread, uthread_t * pInThread);

// Generate a unique id for a user thread.
// Should never return 0 and id 1 is reserved for the main thread.
// Remark: ids are not tracked; watch-out if you create more than 2^32 - 2 user threads.
static unsigned int uthread_internal_generateUniqueThreadId() {
	static unsigned int s_utid = 2;
	if (s_utid == 0) s_utid = 2;
	return s_utid++;
}


void uthread_sleep_wake_up(){
    uthread_t   *sThread; //sThread == Sleep Thread
    long         cTicks;  
    
    //Trivial Case
    if( dlist_isEmpty(&uthread_internal_sleepQueue) ) return;  //Evita o System Call para obter os ticks

    cTicks = uthread_get_internal_ticks();
    if( (((uthread_t *)dlist_getFirst(&uthread_internal_sleepQueue))->sleep_absolute_time) > cTicks ) return;
    
     sThread = dlist_dequeue( &uthread_internal_sleepQueue );
     dlist_enqueue(&uthread_internal_readyQueue, &(sThread->node));
     uthread_sleep_wake_up();
}

///////////////////////////////////////////////////////////
//
// uthread library main functions.
//

// Initialize the uthread library.
// Call uthread_init before using any other functionality of the
// uthread library. The operating system thread that calls
// uthread_init becomes the main user thread. This must be the
// last user thread to call uthread_exit, which means it must
// wait for all other user threads to terminate.

int uthread_compare_sleep_absolute_time(uthread_t * t1, uthread_t * t2){
    return t1->sleep_absolute_time - t2->sleep_absolute_time;
}

int uthread_compare_waitn(uthread_t * t1, uthread_t * t2){
    return t1->waitn - t2->waitn;
}

void dlist_sorted_push( dlist_t *pHeader, dlist_t *pNode, int (*uthread_compareTo)( uthread_t * t1, uthread_t * t2 )  ){
    
    if( dlist_isEmpty(pHeader) ){
        dlist_enqueue( pHeader, pNode );
    }else{

        if( ((uthread_t *)dlist_getLast(pHeader))->sleep_absolute_time < ((uthread_t *)pNode)->sleep_absolute_time ){
            dlist_enqueue( pHeader, pNode );
        }else{
            dlist_t * tempNext;
            dlist_t * aux = pHeader;
            while( 
                        ( ( aux = dlist_getPrevious( pHeader, aux) ) != 0 ) 
                      && ( uthread_compareTo( (uthread_t *)aux, (uthread_t *)pNode ) > 0 ) 
                 );

            if( aux == NULL ) aux = pHeader;

            tempNext = aux->pNextNode;
            aux->pNextNode = pNode;
            pNode->pPrevNode = aux;
            pNode->pNextNode = tempNext;
            pNode->pNextNode->pPrevNode = pNode;
        }
    }
}

void uthread_init() {
    uthread_t * pMainThread;
	dlist_init(&uthread_internal_readyQueue);
    dlist_init(&uthread_internal_sleepQueue);


	pMainThread = (uthread_t *)malloc(sizeof(uthread_t));
	assert(pMainThread != NULL);
	pMainThread->tid = 1;            // the main thread has a fixed id of 1
	pMainThread->function = NULL;    // the thread is already running
	pMainThread->argument = 0;       // ditto
	pMainThread->pStackArena = NULL; // uses the operating system thread's stack
	pMainThread->pContext_ = 0;      // will be set after the first context switch

    dlist_init( &(pMainThread->joined_thread ) );

	// Initialize the counter of user threads.
	uthread_internal_numThreads           = 1;
    pMainThread->type = foreground;
    pMainThread->sleep_absolute_time = 0;
    uthread_countdownlatch_init( &foreground_latch, 0 );
	
	// the main thread is set as the first ready thread
	dlist_enqueue(&uthread_internal_readyQueue, &(pMainThread->node));
	// the main thread is the starts as the running thread
	uthread_internal_pRunningThread = pMainThread;
}

// Voluntarily relinquishing the processor to the next thread in the
// ready queue.
// The calling thread is moved to the end of the ready queue.
// In the particular case where only the running thread is ready to
// run, calling uthread_yield does not change the running thread.
void uthread_yield() {
	// Remove the running thread from the head of the ready queue.
	uthread_t * pRunningThread;
    uthread_sleep_wake_up();
    
    pRunningThread = (uthread_t *)dlist_dequeue(&uthread_internal_readyQueue);

    //printf("*\n");

    // Do a quick test to check whether we have another ready thread or not.
	if (!dlist_isEmpty(&uthread_internal_readyQueue)) {
		// We have another ready thread, so place the running thread at the end
		// of the ready queue and call schedule to perform a context switch.
		dlist_enqueue(&uthread_internal_readyQueue, &(pRunningThread->node));
		uthread_internal_schedule();
	} else {
		// There's only one ready thread, which must be the running thread.
		// Place the thread back on the queue and yield from the operating system's thread.
		dlist_enqueue(&uthread_internal_readyQueue, &(pRunningThread->node));
		Sleep(1);
	}
}


// Terminate the execution of the currently running thread.
// The thread is definitely removed from the ready queue, and its
// associated resources will be automatically released after the
// context switch to the next ready thread. The main user thread
// is a special case: it can call uthread_exit only after all other
// user threads have terminated and it releases all the internal
// resources of the uthread library.
void uthread_exit() {

    if( !dlist_isEmpty( &(uthread_internal_pRunningThread->joined_thread)) ){
	    uthread_t * pThread = (uthread_t *)dlist_dequeue(&(uthread_internal_pRunningThread->joined_thread));
	    dlist_enqueue(&uthread_internal_readyQueue, &(pThread->node));
    }

    //printf( "%d\n",  dlist_isEmpty( &(uthread_internal_pRunningThread->joined_thread) ));

	// Are we trying to exit from the main thread?
	if (uthread_internal_pRunningThread->function != NULL) {
        uthread_cThreads[ uthread_internal_pRunningThread->tid ] = NULL;
        if( uthread_internal_pRunningThread->type == foreground  ) uthread_countdownlatch_countdown( &foreground_latch );

		// No. A normal user thread is terminating.
		// Remove it from the ready queue.
		dlist_remove(&uthread_internal_readyQueue, &(uthread_internal_pRunningThread->node));
		// The resources associated with this thread are used in
		// uthread_internal_contextSwitch, so we can't clean them up now.
		// Cleanup will be done in uthread_internal_schedule or 
		// uthread_internal_start, whichever runs first.
		uthread_internal_pZombieThread = uthread_internal_pRunningThread; 
		// Switch to the next ready thread.
		uthread_internal_schedule();
		assert(0); // should never get here
	} else {

        uthread_countdownlatch_await( &foreground_latch );

		// We're trying to exit from the main thread.
		// This should be the last user thread to exit.
		// An alternative to this assert is to keep a list of all threads
		// and cleanup all their resources here.
		//assert(uthread_internal_numThreads == 1); /????? 
		// OK to cleanup main thread.
		dlist_init(&uthread_internal_readyQueue);
        dlist_init(&uthread_internal_sleepQueue);
		free(uthread_internal_pRunningThread);
		uthread_internal_pRunningThread = NULL;
		uthread_internal_numThreads = 0;
	}
}

///////////////////////////////////////////////////////////
//
// Internal functions supporting the execution of user threads.
//

// Frees the resources used by a recently terminated thread.
static void uthread_internal_cleanupZombieThread() {

	uthread_t * pThread = uthread_internal_pZombieThread;
	assert(uthread_internal_pZombieThread != NULL);

	uthread_internal_pZombieThread = NULL;
	--uthread_internal_numThreads;

	free(pThread->pStackArena);
	free(pThread);
}


// All threads (except the main thread) run this function.
// This allows us to easily intercept the beginning and the end
// of the execution of user threads.
static void uthread_internal_start() {
	uthread_t * pThread = uthread_internal_pRunningThread;
	assert(pThread != 0);
	assert(pThread->function != 0);

	// Before running the new thread, cleanup the resources of
	// any recently terminated thread.
	if (uthread_internal_pZombieThread != NULL) uthread_internal_cleanupZombieThread();
	
	// Execute the thread's function.
	pThread->function(pThread->argument);
	
	// Make sure uthread_exit is called.
	uthread_exit();
}

// The thread at the head of the ready queue should be the running thread.
// Call uthread_internal_schedule after any changes to the ready queue
// that might alter its head node.
static void uthread_internal_schedule() {
	// Get a reference to the first thread on the ready queue.
	uthread_t * pFirstReadyThread = (uthread_t *)dlist_getFirst(&uthread_internal_readyQueue);
	assert(pFirstReadyThread != NULL); // If there are no ready threads we have a deadlock.
	
	// If the first thread on the ready queue is not the currently running thread...
	if (uthread_internal_pRunningThread != pFirstReadyThread) {
		// ...perform a context switch.
		uthread_internal_contextSwitch(uthread_internal_pRunningThread, pFirstReadyThread);
		// In case pOutThread was terminating and pInThread is not starting,
		// cleanup the resources of pOutThread (now zombie) here.
		// If pOutThread was terminating and pInThread is starting, the cleanup
		// of pOutThread will be done by pInThread in uthread_internal_start.
		if (uthread_internal_pZombieThread != NULL) uthread_internal_cleanupZombieThread();
	}
}


// Perform a context switch between two user threads.
// __fastcall: set calling convention such that pOutThread is in ECX and pInThread in EDX
// __declspec(naked): directs the compiler to omit any prologue or epilogue code
static __declspec(naked) void __fastcall uthread_internal_contextSwitch(uthread_t * pOutThread, uthread_t * pInThread) {
	__asm {
		// Phase 1: Saving Current Context 
		// The return address at the top of stack (placed by the call to this
		// function) will be used to proceed with this thread when needed.
		// The current context of the processor is saved in the running
		// thread's stack.
		push	ebp
		push	ebx
		push	esi
		push	edi
		// Save ESP into pOutThread->pContext_
		mov		dword ptr [ecx].pContext_, esp
		// Phase 2: Loading the New Context 
		// Set the new running thread
		mov		uthread_internal_pRunningThread, edx
		// Load ESP from pInThread->pContext_
		mov		esp, dword ptr [edx].pContext_
		// Load the remaining context from the running thread's stack
		pop		edi
		pop		esi
		pop		ebx
		pop		ebp
		// The next "ret" instruction restores the IP of the new running
		// thread with:
		//    a) the address of uthread_internal_start, when the thread
		//       runs for the first time (placed manually on the stack, at
		//       creation time;
		// or, in the subsequent switches, with:
		//    b) the return address placed on the thread's stack by the call
		//       to uthread_internal_contextSwitch that switched it out. 
		ret
	}
}

///////////////////////////////////////////////////////////
//
// uthread_countdownlatch_t
//

// Set the initial state of a count-down latch to start with "initial_units" units.
void uthread_countdownlatch_init(uthread_countdownlatch_t * latch, unsigned int initial_units)
{
	dlist_init(&(latch->queue));
	latch->units = initial_units;
}

// Blocks invoking threads until the latch's units count reaches 0.
void uthread_countdownlatch_await(uthread_countdownlatch_t * latch)
{
	// Check current unit count
	if(latch->units != 0)
	{
		// Unit count has not yet reached zero. Block current thread.
		// 1. Remove "running" thread from the ready list.
		dlist_remove(&uthread_internal_readyQueue,&(uthread_internal_pRunningThread->node));
		// 2. Insert "running" thread at the end of the latch queue.
		dlist_enqueue(&(latch->queue), &(uthread_internal_pRunningThread->node));
		// 3. Context switch to the next ready thread.
		uthread_internal_schedule();
	}
}

void uthread_countdownlatch_countup(uthread_countdownlatch_t * latch){
    latch->units += 1;
}

// Decrements the current unit count.
// If the count reaches 0, all waiting threads are unblocked, meaning, removed from the
// latch's queue and appended to the ready list.
void uthread_countdownlatch_countdown(uthread_countdownlatch_t * latch)
{
	// Decrement unit count and check if waiting threads should be unblocked
	if(--(latch->units) <= 0)
	{
		if(latch->units == 0)
		{
			// Unit count reached 0. Unblock all waiting threads.
			// -> For each thread in the latch queue ...
			while(!dlist_isEmpty(&(latch->queue)))
			{
				// 1. Get the next blocked thread
				uthread_t * pThread = dlist_dequeue(&(latch->queue));
				// 2. Insert unblocked thread into the ready list
				dlist_enqueue(&uthread_internal_readyQueue, &(pThread->node));
			}
		} else {
			// Something is wrong. The unit count has dropped below 0. 
			// If in debug mode, terminate with an error message. 
			assert(latch->units == 0);
			// If not in debug mode, ignore it while ensuring that the latch invariant
			// is preserved (units is never a negative value).
			latch->units = 0;
		}
	} // else: latch unit count as not yet reached 0.
}




///////////////////////////////////////////////////////////
//
// uthread_mutex_t
//

// Set the initial state of a uthread_mutex_t to FREE (owner = 0).
void uthread_mutex_init(uthread_mutex_t * pMutex) {
	dlist_init(&(pMutex->queue));
	pMutex->owner = pMutex->count = 0;
}

// Acquire (lock) a mutex. 
// Locking succeeds if the mutex is FREE or if the calling thread
// is the current owner of the mutex, in which case the internal
// recursion counter is incremented.
// Blocked threads will be released in FIFO order.
void uthread_mutex_lock(uthread_mutex_t * pMutex) {
	if (pMutex->owner == uthread_internal_pRunningThread->tid) {
		// Recursive aquisition. Increment recursion counter.
		pMutex->count++;
	} else if (pMutex->owner == 0) {
		// Mutex is free. Acquire the mutex by marking it with current thread's id.
		pMutex->owner = uthread_internal_pRunningThread->tid;
		pMutex->count = 1;
	} else {
		// The mutex is locked. Block the calling thread.
		// 1. Remove *running* thread from ready list.
		dlist_remove(&uthread_internal_readyQueue, &(uthread_internal_pRunningThread->node));
		// 2. Insert the *running* thread at the end of the mutex's wait queue.
		dlist_enqueue(&(pMutex->queue), &(uthread_internal_pRunningThread->node));
		// 3. Context switch to the next thread in the ready queue.
		uthread_internal_schedule();
	}
}

// Release (unlock) a mutex.
// Unlocking is only effective after the thread which owns the mutex
// has called "unlock" as many times as it has called "lock".
// If there are threads blocked on the mutex, the lock is transferred
// to the next one (in FIFO order).
void uthread_mutex_unlock(uthread_mutex_t * pMutex) {
	// Check that the running thread is the owner.
	if ((pMutex->owner == uthread_internal_pRunningThread->tid)) {
		// Decrement the recursion counter.
		if (--(pMutex->count) == 0) {
			// The mutex is free. Transfer ownership to the next blocked thread, if any.
			if (dlist_isEmpty(&(pMutex->queue))) {
				// No threads blocked. The mutex is free.
				pMutex->owner = 0;
			} else {
				// Get the next blocked thread.
				uthread_t * pThread = (uthread_t *)dlist_dequeue(&(pMutex->queue));
				// Transfer mutex ownership the the woken thread.
				pMutex->owner = pThread->tid;
				pMutex->count = 1;
				// Insert woken thread into the ready queue.
				dlist_enqueue(&uthread_internal_readyQueue, &(pThread->node));
				// The woken thread will execute after getting
				// to the front of the ready queue.
			}
		} // else: lock is still owned by the running thread
	} else {
		// The mutex is not owned by this thread. Ignore or...
		// If in debug mode, terminate with an error message. 
		assert(pMutex->owner == uthread_internal_pRunningThread->tid);
	}
}

///////////////////////////////////////////////////////////
//
// uthread_monitor_t
//
void uthread_monitor_init( uthread_monitor_t * monitor ){
    dlist_init( &(monitor->cv) );
    uthread_mutex_init( &(monitor->lock) );

}
void uthread_monitor_enter( uthread_monitor_t * monitor ){
    uthread_mutex_lock( &(monitor->lock) );
}

void uthread_monitor_exit( uthread_monitor_t * monitor ){
    uthread_mutex_unlock( &(monitor->lock) );
}

void uthread_monitor_wait( uthread_monitor_t * monitor ){

    dlist_remove(&uthread_internal_readyQueue, &(uthread_internal_pRunningThread->node));
    dlist_enqueue( &(monitor->cv), &(uthread_internal_pRunningThread->node) );

    if( monitor->lock.owner == uthread_internal_pRunningThread->tid ) uthread_mutex_unlock( &(monitor->lock) );
    uthread_internal_schedule();
}

void uthread_monitor_pulse( uthread_monitor_t * monitor ){
    uthread_t *uTread; 
    if( !dlist_isEmpty( &(monitor->cv) ) ){
        uTread = dlist_dequeue( &(monitor->cv) );
        dlist_enqueue(&uthread_internal_readyQueue, &(uTread->node)); 
        //Implementação pelo padrão Lampson e Redell  
    }
}

void uthread_monitor_pulseall( uthread_monitor_t * monitor ){
    uthread_t *uTread; 
    while( !dlist_isEmpty( &(monitor->cv) ) ){
        uTread = dlist_dequeue( &(monitor->cv) );
        dlist_enqueue(&uthread_internal_readyQueue, &(uTread->node)); 
        //Implementação pelo padrão Lampson e Redell  
    }
}


///////////////////////////////////////////////////////////
//
// uthread_semaphore_t
//

// Set the initial state of a semaphore to start with "initial_permits" units.
void uthread_semaphore_init(uthread_semaphore_t * pSemaphore, unsigned int initial_permits) {
	dlist_init(&(pSemaphore->queue));
	pSemaphore->permits = initial_permits;
}


void uthread_semaphore_wait_n(uthread_semaphore_t * pSemaphore, unsigned int n) {
		if (pSemaphore->permits >= n) {
		// There are permits available. Get one and keep running.
		 pSemaphore->permits -= n;
	} else {
        uthread_internal_pRunningThread->waitn = n;
		// No permits available. Block on the semaphore queue and schedule another thread.
		// 1. Remove *running* thread from ready list.
		dlist_remove(&uthread_internal_readyQueue, &(uthread_internal_pRunningThread->node));
		// 2. Insert *running* thread at the end of the semaphore queue.
		//dlist_enqueue(&(pSemaphore->queue), &(uthread_internal_pRunningThread->node));
		dlist_sorted_push( &(pSemaphore->queue), &(uthread_internal_pRunningThread->node), &uthread_compare_waitn );
        // 3. Context switch to the next ready thread.
		uthread_internal_schedule();
	}
}

// Get one unit from the semaphore.
// If there are no units available, the calling thread is blocked in the
// semaphore's internal wait queue until a unit is made available by a
// call to "post". Blocked threads are serviced in FIFO order. 
void uthread_semaphore_wait(uthread_semaphore_t * pSemaphore) {
	uthread_semaphore_wait_n( pSemaphore, 1 );
}

// Add one unit to the semaphore.
// If there are threads blocked in the semaphore, the one at the head
// of the wait queue is given the unit and becomes ready to run.
void uthread_semaphore_post(uthread_semaphore_t * pSemaphore) {
     uthread_t * pThread;

     ++(pSemaphore->permits);
     if (dlist_isEmpty(&(pSemaphore->queue))) return; // No threads waiting. Just add a permit.

     pThread = ((uthread_t *)dlist_getFirst(&(pSemaphore->queue)));
     if( pSemaphore->permits >= pThread->waitn ){
         pSemaphore->permits -=  ( pThread->waitn);
         // Release one thread from the semaphore's waiting queue.
         pThread = (uthread_t *)dlist_dequeue(&(pSemaphore->queue));
         // Insert woken thread into the ready queue
         dlist_enqueue(&uthread_internal_readyQueue, &(pThread->node));
         // The woken thread will execute after getting to the front of the ready queue.
     }
}

long uthread_get_internal_ticks(){
    return( GetTickCount() );
}

void uthread_sleep( unsigned int millisecondsTimeout ){
    uthread_t *pThread; 

	//Trivial case
    if( millisecondsTimeout == 0 ){
        uthread_yield(); //Especificação
        return;
    }
 
	pThread = (uthread_t *)dlist_dequeue(&(uthread_internal_pRunningThread));
    pThread->sleep_absolute_time = uthread_get_internal_ticks() + millisecondsTimeout;
    dlist_sorted_push( &uthread_internal_sleepQueue, &(pThread->node), &uthread_compare_sleep_absolute_time);
	uthread_internal_schedule();
}


void uthread_join( unsigned int thread_id ){
    uthread_t * pThread; 

    if( uthread_cThreads[ thread_id ] == NULL ) return;

	pThread = (uthread_t *)dlist_dequeue(&(uthread_internal_pRunningThread));
	dlist_enqueue(&(uthread_cThreads[ thread_id ]->joined_thread), &(pThread->node));

    uthread_internal_schedule();
}

// Create a user thread to run "function".
// The created thread is placed at the end of the ready queue.
void uthread_create(uthread_function_t function, uthread_argument_t argument, unsigned int type) {

	uthread_t * pThread;
	
	// Dinamically allocate an instance of uthread_t
	pThread = (uthread_t *)malloc(sizeof(uthread_t));
	assert(pThread != NULL);
    dlist_init( &(pThread->joined_thread) );
	pThread->tid = uthread_internal_generateUniqueThreadId();
    uthread_cThreads[ pThread->tid ] = pThread;
	pThread->function = function;
	pThread->argument = argument;
	pThread->pStackArena = (unsigned char *)malloc(STACK_SIZE);
	assert(pThread->pStackArena != NULL);
	memset(pThread->pStackArena, 0, STACK_SIZE); // zero all the stack space

	// Map a uthread_context_t on the thread's stack.
	// We'll use it to save the initial context of the thread.
	//
	// +------------+
	// | 0x00000000 |    <- highest word of a thread's stack space
	// +============+      (needs to be set to 0 for Visual Studio to
	// |  ret_addr  | \     correctly present a thread's call stack)
	// +------------+  |
	// |  reg_ebp   |  |
	// +------------+  |
	// |  reg_ebx   |   > uthread_context_t mapped on the stack
	// +------------+  |
	// |  reg_esi   |  |
	// +------------+  |
	// |  reg_edi   | /  <- stack pointer will be set to this address
	// +============+       at the next context switch to this thread
	// |            | \
	// +------------+  |
	// |     :      |  |
	//       :          > remaining stack space
	// |     :      |  |
	// +------------+  |
	// |            | /  <- lowest word of a thread's stack space
	// +------------+      (pStackArena always points to this location)
	//
	pThread->pContext_ = (uthread_context_t *)(pThread->pStackArena
	                                                  + STACK_SIZE
	                                                  - sizeof(unsigned int)
	                                                  - sizeof(uthread_context_t));

	// Set the thread's initial context through pContext_.
	// 1. Set the initial values of EBP, EBX, ESI and EDI.
	pThread->pContext_->reg_edi = 0x33333333; // this value is just for debug purposes
	pThread->pContext_->reg_esi = 0x22222222; // ditto
	pThread->pContext_->reg_ebx = 0x11111111; // ditto
	pThread->pContext_->reg_ebp = 0x00000000; // EBP must be zero, for Visual Studio to
	                                          // correctly present a thread's call stack.
	// 2. Place the address of uthread_initial_start on the stack.
	//    During the first context switch to this thread, after popping
	//    the values of the "saved" registers, a ret instruction will
	//    place this address on the processor's instruction pointer.
	pThread->pContext_->ret_addr = uthread_internal_start;

    pThread->type                = type;
    pThread->sleep_absolute_time = 0;
	// One more user thread.
	++uthread_internal_numThreads;
    if( type == foreground ) uthread_countdownlatch_countup( &foreground_latch );

	// Threads are born ready to run and are placed at the end of the
	// ready queue. The thread will run when it gets to the head of
	// the queue.
	dlist_enqueue(&uthread_internal_readyQueue, &(pThread->node));

    
}

void uthread_create_foreground(uthread_function_t function, uthread_argument_t argument){
    uthread_create(function, argument, foreground );
}

void uthread_create_background(uthread_function_t function, uthread_argument_t argument){
  uthread_create(function, argument, background );
}
