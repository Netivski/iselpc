using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace RendezvousPort
{
    class SyncUtils
    {

        public static void Wait(object masterLock, object condition)
        {
            Wait(masterLock, condition, Timeout.Infinite);
        }

        public static void Wait(object masterLock, object condition, long timeout)
        {
            Monitor.Enter(condition); //When exception is thrown execution terminates with masterLock possession

            bool _inTime;
            TimeSpan t = TimeSpan.FromMilliseconds(timeout);

            Monitor.Exit(masterLock); //Releases masterLock

            try
            {
                lock (condition)
                {
                    _inTime = Monitor.Wait(condition, t); //If exception is raised execution terminates with no control over masterLock
                }
            }
            finally
            {
                bool _interrupted = false;
                while (true)
                {
                    try
                    {
                        Monitor.Enter(masterLock); //Allways has obtain masterLock lock
                        break;
                    }
                    catch (ThreadInterruptedException ie) { _interrupted = true; }
                }
                Monitor.Exit(condition);
                if (_interrupted) throw new ThreadInterruptedException();
            }
            if (!_inTime)
                throw new TimeoutException();
        }

        public static void Pulse(object masterLock, object condition)
        {

            //Pulse is a non blocking primitive, therefore Monitor.Enter exception needs to be ofuscated

            bool _interrupted = false;

            while (true)
            {
                try
                {
                    lock (condition)
                    {
                        Monitor.Pulse(condition);
                        break;                    
                    }
                    
                }
                catch (ThreadInterruptedException) { _interrupted = true; }
            }
            

            if (_interrupted) Thread.CurrentThread.Interrupt();
        }
    }
}
