using System;
using System.Threading;

namespace Tools
{
    public class PhasedGate
    {
        volatile int cdLatch; // cdLatch == Count Down Latch
        object monitor;

        public PhasedGate( int max )
        {
            if (max < 1) throw new ArgumentOutOfRangeException();

            cdLatch = max;
            monitor = new object();
        }

        protected virtual void DoWork(int millisecondsTimeout, bool blocking)
        {
            lock (monitor)
            {
                #region - Nota
                // Não há necessidade de recorrer à class Interlocked.Decrement
                // uma vez que a thread está em posse do monitor.
                // Será que há problemas de visibilidade ?? ( com o modificador volatile não ) 
                #endregion

                if (cdLatch > 0)
                {
                    try
                    {
                        cdLatch -= 1;
                        if (blocking) Monitor.Wait(monitor, millisecondsTimeout);
                    }
                    catch (ThreadInterruptedException ex)
                    {
                        if (cdLatch < 0) Thread.CurrentThread.Interrupt();
                        else throw ex;
                    }
                }
                else Monitor.PulseAll(monitor);
            }
        }

        public void Wait(int millisecondsTimeout)
        {
            DoWork(millisecondsTimeout, true);
        }

        public void RemoveParticipant()
        {
            DoWork(Timeout.Infinite, false);
        }

    }
}
