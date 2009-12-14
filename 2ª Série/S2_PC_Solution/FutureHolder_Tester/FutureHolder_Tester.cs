using System;
using System.Threading;
using Tools;

namespace Tester
{
    class FutureHolder_tester
    {
        private const Int32 N_THREADS = 10;
        static FutureHolder<Int32>[] _arrVT = new FutureHolder<Int32>[N_THREADS]; //Para testar Value Type
        static FutureHolder<String>[] _arrRT = new FutureHolder<String>[N_THREADS]; //PAra testar Reference Type
        private static FutureHolder_tester fht = new FutureHolder_tester();

        public static void Run()
        {
            Console.WriteLine("\nValue Types Test is starting...");
            ValueTypesTester();
            Console.WriteLine("\nValue Types Test has ended");
            Console.WriteLine("\nReference Types Test is starting...");
            ReferenceTypesTester();
            Console.WriteLine("\nReference Types Test has ended");          
        }

        public static void ValueTypesTester()
        {
            //Inicia os objectos presentes no array de FutureHolder's
            for (int i = 0; i < N_THREADS; i++)
            {
                _arrVT[i] = new FutureHolder<Int32>();
            }
           
            //Inicia as Threads para correr o Get
            for (int i = 0; i < N_THREADS; i++)
            {
                Thread t = new Thread(FutureHolder_tester.GetVal);
                t.Start(i);
            }            
            
            //Inicia as Threads para correr o Set
            for (int i = 0; i < N_THREADS; i++)
            {
                Thread t = new Thread(FutureHolder_tester.SetVal);
                t.Start(i);                
            }
        }
        public static void GetVal(object index)
        {
            Console.WriteLine("A tentar is buscar o id = " + (int)index + " ...");
            try
            {
                int i;
                if ((int)index % 3 == 0)
                    i = _arrVT[(int)index].Get((int)index * 100);
                else
                    i = _arrVT[(int)index].Get();

                Console.WriteLine(i != 0 ? "Adquiri o Valor " + i + " no id = " + (int)index : "Sai por TimeOut VT ... no id = " + (int)index);
            }
            catch (ThreadInterruptedException)
            {
                Console.WriteLine("Sai por cancelamento do get id=" + (int)index);
            }
        }
        public static void SetVal(object index)
        {
            if ((int)index % 3 != 0)
            {
                Random rnd = new Random();
                int val = rnd.Next(0, 1000);
                Console.WriteLine("A colocar o valor " + val + " no id = " + (int)index + "...");
                _arrVT[(int)index].Set(val);
            }
        }


        public static void ReferenceTypesTester()
        {
            //Inicia os objectos presentes no array de FutureHolder's
            for (int i = 0; i < N_THREADS; i++)
            {
                _arrRT[i] = new FutureHolder<String>();
            }

            //Inicia as Threads para obter valores
            for (int i = 0; i < N_THREADS; i++)
            {
                Thread t = new Thread(FutureHolder_tester.GetRef);
                t.Start(i);
            }

            //Inicia as Threads para definir valores
            for (int i = 0; i < N_THREADS; i++)
            {
                Thread t = new Thread(FutureHolder_tester.SetRef);
                t.Start(i);
            }
        }
        public static void GetRef(object index)
        {
            Console.WriteLine("A tentar is buscar o id = " + (int)index + " ...");
            try
            {
                String str;
                if ((int)index % 3 == 0)
                    str = _arrRT[(int)index].Get((int)index);
                else
                    str = _arrRT[(int)index].Get();
                Console.WriteLine(str != null ? "Adquiri a String " + str + " no id = " + (int)index : "Sai por TimeOut RT ... no id = " + (int)index);
            }
            catch (ThreadInterruptedException)
            {
                Console.WriteLine("Sai por cancelamento do get id=0");
            }
        }
        public static void SetRef(object index)
        {
            if ((int)index % 3 != 0)
            {
                Random rnd = new Random();
                String str = rnd.Next(0, 1000).ToString();
                Console.WriteLine("A colocar a String " + str + " no id = " + (int)index + "...");
                _arrRT[(int)index].Set(str);
            }
        }        
    }    
}
