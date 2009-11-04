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

#ifndef UTHREAD_H
#define UTHREAD_H

#include "lists.h"

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus

///////////////////////////////////////////////////////////
//
// uthread_t
//

// Auxiliary typedefs for uthread_create
// The main argument for creating a thread is the function to
// be executed in its context. This function should accept a
// void pointer as its single argument.
typedef void * uthread_argument_t;
typedef void (*uthread_function_t)(uthread_argument_t argument);

enum uthread_type{
    foreground = 0x0
   ,background = 0x1
}uthread_type_t;


// Initialize the uthread library.
// Call uthread_init before using any other functionality of the
// uthread library. The operating system thread that calls
// uthread_init becomes the main user thread. This must be the
// last user thread to call uthread_exit, which means it must
// wait for all other user threads to terminate.
void uthread_init();

// Create a user thread to run "function".
// The created thread is placed at the end of the ready queue.
void uthread_create(uthread_function_t function, uthread_argument_t argument, unsigned int type);

void uthread_create_foreground(uthread_function_t function, uthread_argument_t argument);
void uthread_create_background(uthread_function_t function, uthread_argument_t argument);


// Voluntarily relinquishing the processor to the next thread in the
// ready queue.
// The calling thread is moved to the end of the ready queue.
// In the particular case where only the running thread is ready to
// run, calling uthread_yield does not change the running thread.
void uthread_yield();

// Terminate the execution of the currently running thread.
// The thread is definitely removed from the ready queue, and its
// associated resources will be automatically released after the
// context switch to the next ready thread. The main user thread
// is a special case: it can call uthread_exit only after all other
// user threads have terminated and it releases all the internal
// resources of the uthread library.
void uthread_exit();

///////////////////////////////////////////////////////////
//
// uthread_countdownlatch_t
//

// Data structure to hold the state of a count-down latch.
// The user is responsible for the allocation of any needed
// uthread_countdownlatch_t (statically, dinamically or
// automatically) and for calling uthread_countdownlatch_init
// to set its initial state.
typedef struct uthread_countdownlatch {
	unsigned int	units;		// Unit counter
	dlist_t			queue;		// Waiting threads
} uthread_countdownlatch_t;



// Set the initial state of a count-down latch to start with "initial_units" units.
void uthread_countdownlatch_init(uthread_countdownlatch_t * latch, unsigned int initial_units);

// Blocks invoking threads until the latch's units count reaches 0.
void uthread_countdownlatch_await(uthread_countdownlatch_t * latch);

// Decrements the current unit count.
// If the count reaches 0, all waiting threads are unblocked, meaning, removed from the
// latch's queue and appended to the ready queue.
void uthread_countdownlatch_countdown(uthread_countdownlatch_t * latch);

///////////////////////////////////////////////////////////
//
// uthread_mutex_t
//

// Data structure to hold the state of a mutex.
// The user is responsible for the allocation of any needed
// uthread_mutex_t (statically, dinamically or
// automatically) and for calling uthread_mutex_init
// to set its initial state.
typedef struct uthread_mutex {
	unsigned int owner; // 0 means FREE
	unsigned int count; // recursion counter
	dlist_t      queue; // waiting threads
} uthread_mutex_t;

// Set the initial state of a uthread_mutex_t to FREE.
void uthread_mutex_init(uthread_mutex_t * mutex);

// Acquire (lock) a mutex. 
// Locking succeeds if the mutex is FREE or if the calling thread
// is the current owner of the mutex, in which case the internal
// recursion counter is incremented.
// Blocked threads will be released in FIFO order.
void uthread_mutex_lock(uthread_mutex_t * mutex);

// Release (unlock) a mutex.
// Unlocking is only effective after the thread which owns the mutex
// has called "unlock" as many times as it has called "lock".
// If there are threads blocked on the mutex, the lock is transferred
// to the next one (in FIFO order).
// Remark: enumerate reasonable options for the places in the ready
//    queue that should be taken by the releasing and the released
//    threads after a call to "unlock".
void uthread_mutex_unlock(uthread_mutex_t * mutex);

///////////////////////////////////////////////////////////
//
// uthread_monitor_t
//
typedef struct uthread_monitor {
	uthread_mutex_t lock; // unit counter
	dlist_t         cv;   // waiting threads
} uthread_monitor_t;

void uthread_monitor_init( uthread_monitor_t * monitor );
void uthread_monitor_enter( uthread_monitor_t * monitor );
void uthread_monitor_exit( uthread_monitor_t * monitor );
void uthread_monitor_wait( uthread_monitor_t * monitor );
void uthread_monitor_pulse( uthread_monitor_t * monitor );
void uthread_monitor_pulseall( uthread_monitor_t * monitor );


///////////////////////////////////////////////////////////
//
// uthread_semaphore_t
//

// Data structure to hold the state of a semaphore.
// The user is responsible for the allocation of any needed
// uthread_semaphore_t (statically, dinamically or
// automatically) and for calling uthread_semaphore_init
// to set its initial state.
typedef struct uthread_semaphore {
	unsigned int permits; // unit counter
	dlist_t      queue; // waiting threads
} uthread_semaphore_t;

// Set the initial state of a semaphore to start with "initial_permits" units.
void uthread_semaphore_init(uthread_semaphore_t * semaphore, unsigned int initial_permits);

// Get one unit from the semaphore.
// If there are no units available, the calling thread is blocked in the
// semaphore's internal wait queue until a unit is made available by a
// call to "post". Blocked threads are serviced in FIFO order. 
void uthread_semaphore_wait(uthread_semaphore_t * semaphore);

// Add one unit to the semaphore.
// If there are threads blocked in the semaphore, the one at the head
// of the wait queue is given the unit and becomes ready to run.
// Remark: as for the mutex, there is more than one reasonable 
//     option for which places the releasing and released thread
//     should take in the ready queue.
void uthread_semaphore_post(uthread_semaphore_t * semaphore);

void uthread_sleep( unsigned int millisecondsTimeout );

void uthread_join( unsigned int thread_id );

#ifdef __cplusplus
}
#endif // __cplusplus

#endif /* UTHREAD_H */
