using System;
using System.Threading;

namespace Tools
{
    public class FutureHolder<T>
    {
        NullableType<T> nVal;
        Object          monitor;

        public FutureHolder()
        {            
            monitor = new Object();            
        }

        public T Get()
        {
            return Get(Timeout.Infinite);
        }
        
        public T Get(int timeout)
        {
            lock (monitor)
            {                
                if (nVal.HasValue)
                    return nVal.Value;
         
                try
                {
                    Monitor.Wait(monitor, timeout);
                    return nVal.Value;
                }
                catch (ThreadInterruptedException ex)
                {
                    if (nVal.HasValue)
                    {
                        Thread.CurrentThread.Interrupt();
                        return nVal.Value;
                    }
                    else
                    {
                        throw ex;
                    }
                }         
            }
        }        
        
        public void Set(T val)
        {
            lock (monitor)
            {
                nVal.Value = val;
                Monitor.PulseAll(monitor);
            }
        }
    }
}
