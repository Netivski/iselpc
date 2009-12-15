using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Tools;

namespace Tester
{
    class Cache_tester
    {
        private const Int32 N_THREADS = 50;
        static Cache<Int32, String> cache = new Cache<Int32, String>(keyFunction, 3000);

        public static void Run()
        {
            //Criar as primeiras entradas
            for (int i = 0; i < N_THREADS; i++)
            {
                Thread t = new Thread(Cache_tester.RunThread);
                t.Start((i % 10) == 0 ? 2 : i);
            }
            Console.WriteLine(DateToString(DateTime.UtcNow) + " : Vou dormir 15 segundos para ver se os sets ficam todos feitos..");
            Thread.Sleep(15000);
            Console.WriteLine(DateToString(DateTime.UtcNow) + " : Acordei dos 15 segundos !!");

            Console.WriteLine(DateToString(DateTime.UtcNow) + " : A pedir as 10 primeiras Threads...");
            for (int i = 0; i < N_THREADS / 5; i++)
            {
                Thread t = new Thread(Cache_tester.RunThread);
                t.Start(i);
            }
            Console.WriteLine(DateToString(DateTime.UtcNow) + " : Vou dormir 3 segundos..");
            Thread.Sleep(3000);
            Console.WriteLine(DateToString(DateTime.UtcNow) + " : Acordei dos 3 segundos!!");

            Console.WriteLine(DateToString(DateTime.UtcNow) + " : A pedir novamente as 10 primeiras Threads...");
            for (int i = 0; i < N_THREADS / 5; i++)
            {
                Thread t = new Thread(Cache_tester.RunThread);
                t.Start(i);
            }            
        }
        public static String DateToString(DateTime dt)
        {
            return dt.Hour + ":" + dt.Minute + ":" + dt.Second + " " + dt.Millisecond;
        }

        public static void RunThread(object obj)
        {
            Console.WriteLine(DateToString(DateTime.UtcNow) + " : A tentar obter o valor " + (Int32)obj);
            String s = cache.Get((Int32)obj);
            Console.WriteLine(DateToString(DateTime.UtcNow) + " : Obtive a string " + s);
        }

        static String keyFunction(Int32 i)
        {
            Thread.Sleep(5000);
            return i.ToString();
        }
    }
}
