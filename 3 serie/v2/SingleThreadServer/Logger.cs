/*
 * INSTITUTO SUPERIOR DE ENGENHARIA DE LISBOA
 * Licenciatura em Engenharia Informática e de Computadores
 *
 * Programação Concorrente - Inverno de 2009-2010
 * João Trindade
 *
 * Código base para a 3ª Série de Exercícios.
 *
 */

using System;
using System.Collections.Generic;// 
using System.IO;

namespace Tracker
{
    enum LoggerStatus //
    {
         Started //
        ,Stoped  //
    }


	// Logger single-threaded.
	public class Logger
	{
        const int BUFFER_SIZE = 512;

		private readonly TextWriter writer;
		private DateTime start_time;
		private int num_requests;
        private volatile LoggerStatus status = LoggerStatus.Stoped; //
        private object startMon; //
        private LinkedList<string> buffer; // 

		public Logger() : this(Console.Out) {}
		public Logger(string logfile) : this(new StreamWriter(new FileStream(logfile, FileMode.Append, FileAccess.Write))) {}
		public Logger(TextWriter awriter)
		{
		    num_requests = 0;
		    writer = awriter;

            startMon = new object(); //
            buffer   = new LinkedList<string>(); //
		}

        void StartThread()
        {
            while (true)
            {

            }
        }

	    public void Start()
		{
            ////start_time = DateTime.Now;
            ////writer.WriteLine();
            ////writer.WriteLine(String.Format("::- LOG STARTED @ {0} -::", DateTime.Now));
            ////writer.WriteLine();

            lock (startMon) // 
            {
                if (status == LoggerStatus.Started) return; // 

                start_time = DateTime.Now; //Confinamento//       //
                WriteLine(); //
                LogMessage(String.Format("::- LOG STARTED @ {0} -::", DateTime.Now)); //
                WriteLine(); //

                //iniciar a thread 
                //prioridade low
                //StartThread
                //Implementar o produtor / consumidor
            }

		}

		public void LogMessage(string msg)
		{			
            // Escreve no buffer e pulsa o writer //

            lock( buffer ){
                if (buffer.Count < BUFFER_SIZE)
                {
                    buffer.AddLast(msg);
                    return; 
                }

                // verificar se é necessario um monitor para o writers e outro para o reader
            }
		}

        void WriteMessage(string msg) // 
        {
            writer.WriteLine(String.Format("{0}: {1}", DateTime.Now, msg)); //
        }

        void WriteLine() //
        {
            LogMessage(Environment.NewLine); //
        }

		public void IncrementRequests()
		{
			System.Threading.Interlocked.Increment( ref num_requests );
		}

		public void Stop()
		{
			long elapsed = DateTime.Now.Ticks - start_time.Ticks;
			writer.WriteLine();
			LogMessage(String.Format("Running for {0} second(s)", elapsed / 10000000L));
			LogMessage(String.Format("Number of request(s): {0}", num_requests));
			writer.WriteLine();
			writer.WriteLine(String.Format("::- LOG STOPPED @ {0} -::", DateTime.Now));
			writer.Close();
		}
	}
}
