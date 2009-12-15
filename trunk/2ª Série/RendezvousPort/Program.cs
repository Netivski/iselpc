using System;
using System.Collections.Generic;
using System.Threading;

namespace RendezvousPort
{
    class Program
    {
        static int count = 0;
        static RendezvousPort<Package> safePort = new RendezvousPort<Package>();

        private class Package
        {
            public int val;
            public Package(int val) { this.val = val; }
        }

        public static void Client()
        {
            Package myPack = new Package(count++);

            Console.WriteLine("Client thread {0} with pack {1}",
                Thread.CurrentThread.ManagedThreadId, myPack.val);

            try { safePort.RequestService(myPack); }

            catch (ThreadInterruptedException ie)
            {
                Console.WriteLine("Client thread {0} throwed interrupted exception!",
                    Thread.CurrentThread.ManagedThreadId);
                return;
            }

            catch (TimeoutException te)
            {
                Console.WriteLine("Client thread {0} throwed timeout exception!",
                    Thread.CurrentThread.ManagedThreadId);
                return;
            }

            Console.WriteLine("** Client thread {0} new pack {1}",
                Thread.CurrentThread.ManagedThreadId, myPack.val);
        }

        public static void Server()
        {

            Package pack = null;

            try
            {
                pack = safePort.AcceptService();

                Console.WriteLine("Server thread {0} received pack {1}",
                    Thread.CurrentThread.ManagedThreadId, pack.val);
                pack.val += 1000;
                Thread.SpinWait(100000);

                safePort.CompleteService(pack);
            }
            catch (ThreadInterruptedException ie)
            {
                Console.WriteLine("Server thread {0} throwed interrupted exception!",
                    Thread.CurrentThread.ManagedThreadId);
                return;
            }
            catch (TimeoutException ie)
            {
                Console.WriteLine("Server thread {0} throwed timeout exception!",
                    Thread.CurrentThread.ManagedThreadId);
                return;
            }
        }

        public static void Test1()
        {
            Console.WriteLine("\n** Test 1\nLaunch 5 Server threads\nSleep 1s\nLaunch 5 Client threads with a 0,5s cadence\n");

            for (int j = 0; j < 5; j++)
                new Thread(new ThreadStart(Server)).Start();

            Thread.Sleep(1000);

            for (int i = 0; i < 5; i++)
            {
                new Thread(new ThreadStart(Client)).Start();
                Thread.Sleep(500);
            }

            Console.WriteLine("\n** Test 1 ended! Press any key!\n");
            Console.ReadKey();
        }

        public static void Test2()
        {
            Console.WriteLine("\n** Test 2\n");

            for (int i = 0; i < 200; i++)
                new Thread(new ThreadStart(Server)).Start();

            for (int j = 0; j < 200; j++)
                new Thread(new ThreadStart(Client)).Start();

            Console.WriteLine("\n** Test 2 ended! Press any key!\n");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            Test1();
            //Test2();
        }
    }
}