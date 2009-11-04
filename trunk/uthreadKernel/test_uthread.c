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
uthread_monitor_t   q5Monitor;


void question1_testCS_thread(uthread_argument_t arg){	
	int i;
	for(i = 0; i < 1000000; i++){
		uthread_yield();
	}
	uthread_exit();
}
void question1_testCS(){
	int i;
	DWORD _startTime;
	DWORD _endTime;
    uthread_create_foreground(question1_testCS_thread, (uthread_argument_t)1);
	_startTime = GetTickCount();
	for(i = 0; i < 1000000; i++){
		uthread_yield();
	}
	_endTime = GetTickCount();
	printf("Tempo medio no uthread = %.4f \n", (_endTime - _startTime)/(double)2);
}
void question1_testCSWin_thread(){	
	int i;
	for(i = 0; i < 1000000; i++){
		Sleep(0);
	}
	ExitThread(0);
}
void question1_testCSWin(){
	int i;
	DWORD _startTime;
	DWORD _endTime;
	CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE)question1_testCSWin_thread, NULL, 0, NULL);
	_startTime = GetTickCount();
	for(i = 0; i < 1000000; i++){
		Sleep(0);
	}
	_endTime = GetTickCount();
	printf("Tempo medio na WIN32= %.4f \n", (_endTime - _startTime)/(double)2);
}


void question1(){
    printf("\n-:: Question 1 (a) - BEGIN ::-\n\n");

    question1_testCS();
    question1_testCSWin();

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

void question5_thread1(uthread_argument_t arg) {
    printf("Start Thread 1\n");
    uthread_monitor_enter( &q5Monitor );
    //Faz cenas ... 

       //Não há condições psicológicas para ...
       //O lock é removido na primitiva de wait
       uthread_monitor_wait( &q5Monitor );

       //Pela implementação do padrão Lampson e Redell
       //seria necesário verificar as condições que levaram ao wait.
       //Implementação com recurso a um while(!condição){ wait(); }
       //Neste cenário aparece o conceito de notificação espúria 

    //Fim de Faz Cenas ...
    printf("End Thread 1\n");
}

void question5_thread2(uthread_argument_t arg) {
    printf("Start Thread 2\n");
    uthread_monitor_enter( &q5Monitor );
    //Faz cenas ... 
    uthread_monitor_pulse( &q5Monitor );
    uthread_monitor_exit( &q5Monitor );
    printf("End Thread 2\n");
}

void question5(){
	int i;
	printf("\n-:: Question 5 (a) - BEGIN ::-\n\n");
	
    printf("Init Monitor\n");
    uthread_monitor_init( &q5Monitor );

    printf("Thread Create 1 & 2\n");
    uthread_create_foreground(question5_thread1, (uthread_argument_t)('1'));
    uthread_create_foreground(question5_thread2, (uthread_argument_t)('1'));
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

//
///////////////////////////////////////////////////////////

int main()
{
    int option = 1;

    printf("\n-:: START  ::-\n");
	uthread_init();
	{
        switch( option ){
            case 1:
                question1();
                break;
            case 2:
                question2();
                break;
            case 3:
                question3();
                break;
            case 4:
                question4();
                break;
            case 5:
                question5();
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
