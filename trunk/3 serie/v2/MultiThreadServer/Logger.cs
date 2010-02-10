using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace Tracker
{
    enum LoggerStatus 
    {
        Started
       ,Stoped
    }


	public class Logger
	{
        const int QUEUE_MAX_SIZE = 2048;

		readonly TextWriter         writer;
		DateTime                    start;
        int                         requestCount;
        volatile LoggerStatus       status;
        volatile Thread             workerThread;
        readonly LinkedList<string> messageQueue;

		public Logger() : this(Console.Out) {}
		public Logger(string logfile) : this(new StreamWriter(new FileStream(logfile, FileMode.Append, FileAccess.Write))) {}
		public Logger(TextWriter awriter)
		{
		    requestCount = 0;
		    writer       = awriter;
            status       = LoggerStatus.Stoped;
            messageQueue = new LinkedList<string>();
		}

        void WriterWork()
        {            
            while (true)
            {
                string queueEntry = null;
                lock (messageQueue)
                {
                    if (messageQueue.Count > 0)
                    {
                        queueEntry = messageQueue.First.Value;
                        messageQueue.RemoveFirst();
                    }
                    else
                    {
                        try
                        {
                            Monitor.Wait(messageQueue);
                        }
                        catch (ThreadInterruptedException) { 
                        }
                    }
                }

                if (queueEntry != null) writer.Write(queueEntry);
            }
        }

        void Enqueue(string text)
        {
            lock (messageQueue)
            {
                if (messageQueue.Count < QUEUE_MAX_SIZE) //fast path
                {
                    messageQueue.AddLast(text);
                    Monitor.PulseAll(messageQueue); //Pulse All Threads, podia ser implementado com delegação especifica.
                    return;
                }
                
                while (true)
                {
                    //Implementar o cancelamento e desistência
                    Monitor.Wait(messageQueue);

                    if (messageQueue.Count < QUEUE_MAX_SIZE) //fast path
                    {
                        messageQueue.AddLast(text);
                        Monitor.PulseAll(messageQueue);
                        return;
                    }
                }
            }
        }

        void WriteLine(string text)
        {
            Enqueue(string.Format("{0}{1}", text, Environment.NewLine));
        }

        void WriteLine()
        {
            Enqueue(Environment.NewLine);
        }

        public bool Start()
        {
            lock (this)
            {
                if (status == LoggerStatus.Started) return false;

                start  = DateTime.Now;
                status = LoggerStatus.Started;
            }

            workerThread = new Thread(new ThreadStart(WriterWork));
            workerThread.Priority = ThreadPriority.Lowest;
            workerThread.Start();

            WriteLine();
            WriteLine(String.Format("::- LOG STARTED @ {0} -::", start));
            WriteLine();

            return true;
        }


		public void LogMessage(string msg)
		{
            WriteLine(String.Format("{0}: {1}", DateTime.Now, msg));
		}

		public int IncrementRequests()
		{
            return System.Threading.Interlocked.Increment(ref requestCount);
		}

		public bool Stop()
		{
            WriteLine();
            LogMessage(String.Format("Running for {0} second(s)", (DateTime.Now.Ticks - start.Ticks) / 10000000L));
            LogMessage(String.Format("Number of request(s): {0}", requestCount));
            WriteLine();
            LogMessage(String.Format("::- LOG STOPPED @ {0} -::", DateTime.Now));


            lock (this)
            {
                if (status == LoggerStatus.Stoped) return false;
                workerThread.Interrupt(); //Start Stop 
                status = LoggerStatus.Stoped;                    
            }

            return true;			
		}
	}
}
