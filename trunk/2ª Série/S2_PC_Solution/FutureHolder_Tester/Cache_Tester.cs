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
        static Cache<Int32, String> cache = new Cache<int, string>(keyFunction, 10000);

        public static void Run()
        {
            cache.Get(1);
            Thread.Sleep(10000);
            cache.Get(2);
            Thread.Sleep(10000);
            cache.Get(3);
        }


        static String keyFunction(Int32 i)
        {
            return i.ToString();
        }
    }
}
