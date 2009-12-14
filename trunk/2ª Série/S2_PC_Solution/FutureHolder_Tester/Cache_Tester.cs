using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Tools;

namespace Tester
{
    class Cache_tester
    {
        private const Int32 N_THREADS = 10;
        static Cache<Int32, String> cache = new Cache<Int32, String>(keyFunction);

        public static void Run()
        {           
            for (int i = 0; i < 50; i++)
            {
                Thread t = new Thread(Cache_tester.RunThread);
                t.Start((i % 10) == 0? 2 : i);
            }
        }

        public static void RunThread(object obj)
        {
            Console.WriteLine("A tentar obter o valor " + (Int32)obj);
            String s = cache.Get((Int32)obj);
            Console.WriteLine("Obtive a string " + s);
        }

        static String keyFunction(Int32 i)
        {
            Thread.Sleep(30000);
            return i.ToString();
        }
    }
}
