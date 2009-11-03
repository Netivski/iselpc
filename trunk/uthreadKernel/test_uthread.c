///////////////////////////////////////////////////////////
//
// ISEL - LEIC - Programação Concorrente
// October 2007
//
// uthread library test
//
// Author: Carlos Martins
//    (revised by João Trindade)
// 

#include <assert.h>
#include <stdlib.h>
#include <stdio.h>
#include <conio.h>
#include <time.h>
#include <windows.h>

#include "uthread.h"

uthread_semaphore_t q2s;


///////////////////////////////////////////////////////////
//
// Test 1 (a): 10 threads, each one printing its number 16 times
//
unsigned int test1a_count;
//
void test1a_thread(uthread_argument_t arg) {
	int i, c = (int)arg;
	for (i = 0; i < 16; ++i) {
		putchar(c);
		if ((rand() % 4) == 0) 
			uthread_yield();
	}
	++test1a_count;
	uthread_exit(); // not really needed
}
//
void test1a() {
	int i;
	printf("\n-:: Test 1 (a) - BEGIN ::-\n\n");
	test1a_count = 0; // makes test1 non-reentrant
	for (i = 0; i < 10; ++i) {
		uthread_create_foreground(test1a_thread, (uthread_argument_t)('0' + i));
	}
	while (test1a_count != 10) uthread_yield();
	printf("\n-:: Test 1 (a) -  END  ::-\n");
}

void question4_thread(uthread_argument_t arg) {
	int i, c = (int)arg;
	for (i = 0; i < 16; ++i) {
		putchar(c);
        if ((rand() % 4) == 0){
            putchar('*');
			uthread_yield();
        }
	}
	//uthread_exit(); // not really needed
}

void question4(){
	int i;
	printf("\n-:: Question 4 (a) - BEGIN ::-\n\n");
	
    for (i = 0; i < 100; ++i) {
        if( (i % 10) == 0 ){
		  uthread_create_foreground(question4_thread, (uthread_argument_t)('f'));
        }
        uthread_create_background(question4_thread, (uthread_argument_t)('b'));
	}
}


void question6_thread(uthread_argument_t arg) {
    int i; 
    for( i = 0; i < 1500; i++ ){
        printf("Index{%d}, question6_thread -> Method , char {%c} \n", i, (int)arg);
      if( (i % 4 ) == 0 ) uthread_yield();
    }
}

void question6(){
	int i;
	printf("\n-:: Question 6 (a) - BEGIN ::-\n\n");
	
    printf("Thread Create\n");
    uthread_create_foreground(question6_thread, (uthread_argument_t)('1'));
    
    printf("Thread Before Join\n");
    uthread_join( 2 );
    printf("Thread After Join\n");
}


void question3_thread(uthread_argument_t arg) {
    int i;
    long start;

    start = GetTickCount();
    for( i = 0; i < 15000; i++ ){
        printf("Index{%d}, question3_thread -> Method , char {%c} \n", i, (int)arg);
      if( (i % 4 ) == 0 ) uthread_yield();
    }

    printf("%ldms", GetTickCount() - start);
}

void question3_sleep_thread(uthread_argument_t arg){
    int start, end, i;
    start = GetTickCount();
    printf("Antes de esperar 2000ms\n");
    uthread_sleep( 2000 );
    end = GetTickCount();
    //printf("Depois de esperar %ldms\n", end - start);
    for( i = 0; i < 1500; i++ ){
        printf("Sleep %ldms\n", end - start);
    }
}

void question3(){
	int i;
	printf("\n-:: Question 3 (a) - BEGIN ::-\n\n");
	
    uthread_create_foreground(question3_thread, (uthread_argument_t)('1'));
    uthread_create_foreground(question3_sleep_thread, (uthread_argument_t)('1'));
}

void question2_1_thread(uthread_argument_t arg) {
    printf( "Start question2_1_thread;\n" );
    uthread_semaphore_wait( &q2s );
    printf( "Yield question2_1_thread;\n" );
    uthread_yield();
    printf( "post question2_1_thread;\n" );
    uthread_semaphore_post( &q2s );
}

void question2_2_thread(uthread_argument_t arg) {
    printf( "Start question2_2_thread;\n" );
    uthread_semaphore_wait( &q2s );
    printf( "Yield question2_2_thread;\n" );
    uthread_yield();
    printf( "post question2_2_thread;\n" );
    uthread_semaphore_post( &q2s );
}

void question2_3_thread(uthread_argument_t arg) {
    printf( "Start question2_3_thread;\n" );
    uthread_semaphore_wait_n( &q2s, 3 );
    printf( "Yield question2_3_thread;\n" );
    uthread_yield();
    printf( "post question2_3_thread;\n" );
    uthread_semaphore_post( &q2s );
    printf( "post question2_3_thread;\n" );
    uthread_semaphore_post( &q2s );
    printf( "post question2_3_thread;\n" );
    uthread_semaphore_post( &q2s );
}

void question2_4_thread(uthread_argument_t arg) {
    printf( "Start question2_4_thread;\n" );
    uthread_semaphore_wait( &q2s );
    printf( "Yield question2_4_thread;\n" );
    uthread_yield();
    printf( "post question2_4_thread;\n" );
    uthread_semaphore_post( &q2s );
}

void question2(){
	printf("\n-:: Question 2 (a) - BEGIN ::-\n\n");

    uthread_semaphore_init( &q2s, 3 );

    uthread_create_foreground(question2_1_thread, (uthread_argument_t)('1'));
    uthread_create_foreground(question2_2_thread, (uthread_argument_t)('1'));
    uthread_create_foreground(question2_3_thread, (uthread_argument_t)('1'));
    uthread_create_foreground(question2_4_thread, (uthread_argument_t)('1'));
}


//
///////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////
//
// Test 1 (b): 10 threads, each one printing its number 16 times
//

typedef struct test1b_thread_argument
{
	int thread_number;
	uthread_countdownlatch_t * sharedLatch;

} test1b_thread_argument_t;
//
void test1b_thread(uthread_argument_t arg) {
	test1b_thread_argument_t * pArg = (test1b_thread_argument_t *) arg;
	int i, c = pArg->thread_number;
	uthread_countdownlatch_t * pLatch = pArg->sharedLatch;
	for (i = 0; i < 16; ++i) {
		putchar(c);
		if ((rand() % 4) == 0) 
			uthread_yield();
	}
	uthread_countdownlatch_countdown(pLatch);
	uthread_exit(); // not really needed
}
//
#define TEST1B_COUNT	10
//
void test1b() {
	int i;
	uthread_countdownlatch_t latch;
	test1b_thread_argument_t args[TEST1B_COUNT];

	uthread_countdownlatch_init(&latch, TEST1B_COUNT);

	printf("\n-:: Test 1 (b) - BEGIN ::-\n\n");
	// test1_count = 0; // makes test1 non-reentrant
	for (i = 0; i < TEST1B_COUNT; ++i) {
		args[i].thread_number = '0' + i;
		args[i].sharedLatch = &latch;
		uthread_create_foreground(test1b_thread, (uthread_argument_t) &args[i]);
	}
	uthread_countdownlatch_await(&latch);	// Passive wait !
	printf("\n-:: Test 1 (b) -  END  ::-\n");
}
//
///////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////
//
// Test 2: Testing mutexes
//
unsigned int test2_count;
//
void test2_thread1(uthread_argument_t arg) {
	uthread_mutex_t * pMutex = (uthread_mutex_t *)arg;
	printf("Thread1 running\n");

	printf("Thread1 acquiring the mutex...\n");
	uthread_mutex_lock(pMutex);
	printf("Thread1 acquired the mutex...\n");

	uthread_yield();
	
	printf("Thread1 acquiring the mutex again...\n");
	uthread_mutex_lock(pMutex);
	printf("Thread1 acquired the mutex again...\n");

	uthread_yield();
	
	printf("Thread1 releasing the mutex...\n");
	uthread_mutex_unlock(pMutex);
	printf("Thread1 released the mutex...\n");

	uthread_yield();

	printf("Thread1 releasing the mutex again...\n");
	uthread_mutex_unlock(pMutex);
	printf("Thread1 released the mutex again...\n");

	printf("Thread1 exiting\n");
	++test2_count;
}
//
void test2_thread2(uthread_argument_t arg) {
	uthread_mutex_t * pMutex = (uthread_mutex_t *)arg;
	printf("Thread2 running\n");
	
	printf("Thread2 acquiring the mutex...\n");
	uthread_mutex_lock(pMutex);
	printf("Thread2 acquired the mutex...\n");
	
	uthread_yield();

	printf("Thread2 releasing the mutex...\n");
	uthread_mutex_unlock(pMutex);
	printf("Thread2 released the mutex...\n");

	printf("Thread2 exiting\n");
	++test2_count;
}
//
void test2_thread3(uthread_argument_t arg) {
	uthread_mutex_t * pMutex = (uthread_mutex_t *)arg;
	printf("Thread3 running\n");
	
	printf("Thread3 acquiring the mutex...\n");
	uthread_mutex_lock(pMutex);
	printf("Thread3 acquired the mutex...\n");
	
	uthread_yield();

	printf("Thread3 releasing the mutex...\n");
	uthread_mutex_unlock(pMutex);
	printf("Thread3 released the mutex...\n");
	
	printf("Thread3 exiting\n");
	++test2_count;
}
//
void test2() {
	uthread_mutex_t mutex;
	uthread_mutex_init(&mutex);
	printf("\n-:: Test 2 - BEGIN ::-\n\n");
	test2_count = 0; // makes test2 non-reentrant
	uthread_create_foreground(test2_thread1, &mutex);
	uthread_create_foreground(test2_thread2, &mutex);
	uthread_create_foreground(test2_thread3, &mutex);
	while (test2_count != 3) uthread_yield();
	printf("\n-:: Test 2 -  END  ::-\n");
}
//
///////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////
//
// Test 3: building a mailbox with a mutex and a semaphore
//
typedef struct mailbox {
	uthread_mutex_t     lock;      // lock to grant exclusive access
	uthread_semaphore_t msg_sem;   // semaphore to control message list
	dlist_t             msg_queue; // message queue
} mailbox_t;
//
typedef struct mailbox_message {
	dlist_node_t node;
	void * data;
} mailbox_message_t;
//
static void mailbox_init(mailbox_t * pMailbox) {
	uthread_mutex_init(&(pMailbox->lock));
	uthread_semaphore_init(&(pMailbox->msg_sem), 0);
	dlist_init(&(pMailbox->msg_queue));
}
//
static void mailbox_post(mailbox_t * pMailbox, void * data) {
	// Create an envelope.
	mailbox_message_t * pMessage = (mailbox_message_t *)malloc(sizeof(mailbox_message_t));
	assert(pMessage != NULL);
	pMessage->data = data;

	// Insert the message in the mailbox queue.
	// The lock is not really necessary in cooperative multithreading,
	// as there is only one thread accessing the queue at any moment.
	uthread_mutex_lock(&(pMailbox->lock));
	// uthread_yield(); // yield to test exclusion
	dlist_enqueue(&(pMailbox->msg_queue), &(pMessage->node));
	// printf("** enqueued: 0x%08x **\n", pMessage);
	uthread_mutex_unlock(&(pMailbox->lock));
	
	// Add one permit to indicate the availability of one more message.
	uthread_semaphore_post(&(pMailbox->msg_sem));
}
//
static void * mailbox_wait(mailbox_t * pMailbox) {
	mailbox_message_t * pMessage;
	void * data;

	// Wait for a message to available on the mailbox.
	uthread_semaphore_wait(&(pMailbox->msg_sem));
	
	// Get the envelope from the mailbox queue.
	// The lock is not really necessary in cooperative multithreading,
	// as there is only one thread accessing the queue at any moment.
	uthread_mutex_lock(&(pMailbox->lock));
	// uthread_yield(); // yield to test exclusion
	pMessage = (mailbox_message_t *)dlist_dequeue(&(pMailbox->msg_queue));
	// printf("** dequeued: 0x%08x **\n", pMessage);
	assert(pMessage != NULL);
	uthread_mutex_unlock(&(pMailbox->lock));
	
	// Extract the message from the envelope.
	data = pMessage->data;
	// Destroy the envelope and return the message.
	free(pMessage);
	return data;
}
//
unsigned int test3_countp;
unsigned int test3_countc;
//
void test3_producer_thread(uthread_argument_t arg) {
	static unsigned int current_id = 0;
	unsigned int producer_id = ++current_id;
	mailbox_t * pMailbox = (mailbox_t *)arg;

	char * msg;
	unsigned int msg_num;
	for (msg_num = 0; msg_num < 2000; ++msg_num) {
		msg = (char *)malloc(64);
		sprintf(msg, "Message %04d from producer %d", msg_num, producer_id);
		printf(" ** producer %d: sending message %04d [0x%08x]\n", producer_id, msg_num, msg);
		mailbox_post(pMailbox, msg);
		if ((rand() % 2) == 0) uthread_yield();
		// Sleep(1000); // to pause display
	}
	
	++test3_countp;
}
//
void test3_consumer_thread(uthread_argument_t arg) {
	static unsigned int current_id = 0;
	unsigned int consumer_id = ++current_id;
	mailbox_t * pMailbox = (mailbox_t *)arg;

	unsigned int num_msgs = 0;
	char * msg;
	while (1) {
		// Get a message from the mailbox.
		msg = (char *)mailbox_wait(pMailbox);
		if (msg != (char *)-1) {
			++num_msgs;
			printf(" ++ consumer %d: got \"%s\" [0x%08x]\n", consumer_id, msg, msg);
			// Free memory used by message
			free(msg);
		} else {
			printf(" ++ consumer %d: exiting after %d messages\n", consumer_id, num_msgs);
			break;
		}
	}
	
	++test3_countc;
}
//
void test3() {
	mailbox_t test_mailbox;
	mailbox_init(&test_mailbox);
	printf("\n-:: Test 3 - BEGIN ::-\n\n");
	test3_countp = 0; // makes test3 non-reentrant
	test3_countc = 0; // makes test3 non-reentrant
	uthread_create_foreground(test3_consumer_thread, &test_mailbox);
	uthread_create_foreground(test3_consumer_thread, &test_mailbox);
	uthread_create_foreground(test3_producer_thread, &test_mailbox);
	uthread_create_foreground(test3_producer_thread, &test_mailbox);
	uthread_create_foreground(test3_producer_thread, &test_mailbox);
	uthread_create_foreground(test3_producer_thread, &test_mailbox);
	while (test3_countp != 4) uthread_yield();
	mailbox_post(&test_mailbox, (void *)-1); // will terminate one consumer
	mailbox_post(&test_mailbox, (void *)-1); // will terminate one consumer
	while (test3_countc != 2) uthread_yield();
	printf("\n-:: Test 3 -  END  ::-\n");
}
//
///////////////////////////////////////////////////////////

int main()
{
    int option = 2;
    long a, b;
	srand((unsigned int)time(NULL));


    a = GetTickCount();
    Sleep(1000);
    b = GetTickCount();
    printf ( "a=%ld; b=%ld; B - A = %ld\n", a, b, b-a );

    printf("\n-:: START  ::-\n");
	uthread_init();
	{
        switch( option ){
            case 2:
                question2();
                break;
            case 3:
                question3();
                break;
            case 4:
                question4();
                break;
            case 6:
                question6();
                break;
        }
	}

    uthread_exit();
    printf("\n-:: END  ::-\n");
	
	return 0;
}
