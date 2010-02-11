using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Tracker
{
    internal sealed class StateObject
    {
        public const int BufferSize = 256;

        volatile StringReader input;

        public readonly TcpClient Socket;
        public readonly byte[] Buffer;
        public readonly StringBuilder Content;
        public readonly Logger Log;

        public StateObject(TcpClient cSocket, Logger log)
        {
            Socket = cSocket;
            Log = log;
            Buffer = new byte[BufferSize];
            Content = new StringBuilder();
        }

        public NetworkStream Stream { get { return Socket.GetStream(); } }

        public string ReadLine()
        {
            lock (this)
            {
                if (input == null) input = new StringReader(Content.ToString());

                return input.ReadLine();
            }
        }
    }

    public sealed class Handler
    {
        #region Message handlers

        static readonly Dictionary<string, Action<StateObject>> MESSAGE_HANDLERS;

        static Handler()
        {
            MESSAGE_HANDLERS = new Dictionary<string, Action<StateObject>>();
            MESSAGE_HANDLERS["REGISTER"] = ProcessRegisterMessage;
            MESSAGE_HANDLERS["UNREGISTER"] = ProcessUnregisterMessage;
            MESSAGE_HANDLERS["LIST_FILES"] = ProcessListFilesMessage;
            MESSAGE_HANDLERS["LIST_LOCATIONS"] = ProcessListLocationsMessage;
        }

        static void ProcessRegisterMessage(StateObject state)
        {
            string line;
            while (!string.IsNullOrEmpty((line = state.ReadLine())))
            {
                string[] triple = line.Split(':');
                if (triple.Length != 3)
                {
                    state.Log.LogMessage("Handler - Invalid REGISTER message.");
                    return;
                }
                IPAddress ipAddress;
                if (!IPAddress.TryParse(triple[1], out ipAddress))
                {
                    state.Log.LogMessage("Handler - Invalid REGISTER message.");
                    return;
                }
                ushort port;
                if (!ushort.TryParse(triple[2], out port))
                {
                    state.Log.LogMessage("Handler - Invalid REGISTER message.");
                    return;
                }
                Store.Instance.Register(triple[0], new IPEndPoint(ipAddress, port));
            }
        }

        static void ProcessUnregisterMessage(StateObject state)
        {
            string line;
            if (!string.IsNullOrEmpty((line = state.ReadLine())))
            {
                string[] triple = line.Split(':');
                if (triple.Length != 3)
                {
                    state.Log.LogMessage("Handler - Invalid UNREGISTER message.");
                    return;
                }
                IPAddress ipAddress;
                if (!IPAddress.TryParse(triple[1], out ipAddress))
                {
                    state.Log.LogMessage("Handler - Invalid UNREGISTER message.");
                    return;
                }
                ushort port;
                if (!ushort.TryParse(triple[2], out port))
                {
                    state.Log.LogMessage("Handler - Invalid UNREGISTER message.");
                    return;
                }
                Store.Instance.Unregister(triple[0], new IPEndPoint(ipAddress, port));
            }
            else
            { state.Log.LogMessage("Handler - Invalid UNREGISTER message."); }
        }

        private static void ProcessListFilesMessage(StateObject state)
        {
            string[] trackedFiles = Store.Instance.GetTrackedFiles();

            StringBuilder sb = new StringBuilder();
            foreach (string file in trackedFiles)
                sb.AppendLine(file);

            byte[] response = Encoding.ASCII.GetBytes(sb.ToString());
            state.Stream.BeginWrite(response, 0, response.Length, null, null);
        }

        private static void ProcessListLocationsMessage(StateObject state)
        {
            string line;
            if (!string.IsNullOrEmpty((line = state.ReadLine())))
            {
                if (Store.Instance.ContainsKey(line))
                {
                    IPEndPoint[] fileLocations = Store.Instance.GetFileLocations(line);

                    StringBuilder sb = new StringBuilder();
                    foreach (IPEndPoint endpoint in fileLocations)
                        sb.AppendLine(string.Format("{0}:{1}", endpoint.Address, endpoint.Port));

                    byte[] response = Encoding.ASCII.GetBytes(sb.ToString());
                    state.Stream.BeginWrite(response, 0, response.Length, null, null);
                }
                else
                { state.Log.LogMessage("Handler - File indicated in LIST_LOCATIONS message not being hosted."); }
            }
            else
            { state.Log.LogMessage("Handler - Invalid LIST_LOCATIONS message."); }
        }

        #endregion

        static readonly int TIMEOUT = 30000;

        /// <summary>
        /// Reads data asynchronously from the StateObject stream
        /// </summary>
        /// <param name="ar">IAsyncResult which has the StateObject</param>
        static void ReadDataCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            try
            {
                int bytesRead = state.Stream.EndRead(ar);
                state.Log.LogMessage("Waiting.....");
                if (bytesRead > 0)
                {
                    state.Content.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                    // Tests if one blank line was sent meaning client submited request
                    if (state.Content.ToString().Contains("\r\n\r\n"))
                    {
                        string requestType = state.ReadLine();
                        if (!String.IsNullOrEmpty(requestType))
                        {
                            // If client typed a valid command then calls its corresponding handler
                            if (MESSAGE_HANDLERS.ContainsKey(requestType.ToUpper()))
                            {
                                MESSAGE_HANDLERS[requestType](state);
                                Program.ShowInfo(Store.Instance);
                            }
                            // Else command is not recognized
                            else
                            {
                                state.Log.LogMessage("Handler - Unknown message type.");
                            }
                        }
                        else
                        {
                            state.Log.LogMessage("Handler - Unknown message type.");
                        }
                        // After processing request starts a thread to monitor other incoming request
                        //new Action<TcpClient, Logger>(Handler.StartAcceptTcpClient)
                        //    .BeginInvoke(state.Socket, state.Log, null, null);
                        state.Log.LogMessage(string.Format("Handler - Closing connection @ {0}", state.Socket.Client.RemoteEndPoint));
                        state.Socket.Close();
                        return;
                    }
                    // If no blank line was sent continues to read the stream
                    else
                    {
                        state.Stream.BeginRead(state.Buffer, 0, StateObject.BufferSize,
                           new AsyncCallback(ReadDataCallback), state);
                    }
                }
                // If no bytes were received the connection was lost
                else
                {
                    state.Log.LogMessage("Handler - Connection to client was lost.");
                    state.Socket.Close();
                    //state.Stream.BeginRead(state.Buffer, 0, StateObject.BufferSize,
                    //    new AsyncCallback(ReadDataCallback), state);

                }
            }
            catch (IOException) { state.Log.LogMessage("Handler - Connection closed by client.\n"); }
            catch (ObjectDisposedException) { state.Log.LogMessage("Handler - Timeout expired while receivig request. Servicing ending.\n"); }
            catch (InvalidOperationException) { state.Log.LogMessage("Handler - Timeout expired while receivig request. Servicing ending.\n"); }
        }

        /// <summary>
        /// Performs request servicing.
        /// </summary>
        public static void StartAcceptTcpClient(TcpClient socket, Logger log)
        {
            try
            {
                StateObject state = new StateObject(socket, log);
                IAsyncResult iaR = state.Stream.BeginRead(state.Buffer, 0, StateObject.BufferSize,
                    new AsyncCallback(ReadDataCallback), state);
                new Timer(delegate
                {
                    if (!iaR.IsCompleted)
                        ((StateObject)iaR.AsyncState).Socket.Close();
                }, iaR, Handler.TIMEOUT, Timeout.Infinite);
            }
            catch (IOException ioe) { log.LogMessage(String.Format("Handler - Connection closed by client.\n{0}", ioe)); }
            catch (SocketException se) { log.LogMessage(String.Format("Handler - Client request timed out. Servicing ending.\n{0}", se)); }
        }
    }


    /// <summary>
    /// This class instances are file tracking servers. They are responsible for accepting 
    /// and managing established TCP connections.
    /// </summary>
    public sealed class Listener
    {
        /// <summary>
        /// TCP port number in use.
        /// </summary>
        private readonly int portNumber;

        /// <summary>
        /// Receive timeout for each Client connection.
        /// </summary>
        private readonly int timeOut;

        /// <summary> Initiates a tracking server instance.</summary>
        /// <param name="_portNumber"> The TCP port number to be used.</param>
        /// <param name="_timeOut"> The timeout value for data receiving.</param>
        public Listener(int _portNumber, int _timeOut)
        {
            portNumber = _portNumber;
            timeOut = _timeOut;
        }

        /// <summary>
        ///	Server's main loop implementation.
        /// </summary>
        /// <param name="log"> The Logger instance to be used.</param>
        public void Run(Logger log)
        {
            TcpListener srv = null;
            try
            {
                srv = new TcpListener(IPAddress.Loopback, portNumber);
                srv.Start();
                while (true)
                {
                    log.LogMessage("Listener - Waiting for connection requests.");

                    TcpClient socket = srv.AcceptTcpClient();
                    socket.ReceiveTimeout = timeOut;
                    socket.LingerState = new LingerOption(true, 10);
                    log.LogMessage(String.Format("Listener - Connection established with {0}.", socket.Client.RemoteEndPoint));

                    Handler.StartAcceptTcpClient(socket, log);
                }
            }
            finally
            {
                log.LogMessage("Listener - Ending.");
                srv.Stop();
            }
        }
    }

    class Program
    {
        public static void ShowInfo(Store store)
        {
            foreach (string fileName in store.GetTrackedFiles())
            {
                Console.WriteLine(fileName);
                foreach (IPEndPoint endPoint in store.GetFileLocations(fileName))
                {
                    Console.Write(endPoint + " ; ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        /*
                static void TestStore()
                {
                    Store store = Store.Instance;

                    store.Register("xpto", new IPEndPoint(IPAddress.Parse("193.1.2.3"), 1111));
                    store.Register("xpto", new IPEndPoint(IPAddress.Parse("194.1.2.3"), 1111));
                    store.Register("xpto", new IPEndPoint(IPAddress.Parse("195.1.2.3"), 1111));
                    ShowInfo(store);
                    Console.ReadLine();
                    store.Register("ypto", new IPEndPoint(IPAddress.Parse("193.1.2.3"), 1111));
                    store.Register("ypto", new IPEndPoint(IPAddress.Parse("194.1.2.3"), 1111));
                    ShowInfo(store);
                    Console.ReadLine();
                    store.Unregister("xpto", new IPEndPoint(IPAddress.Parse("195.1.2.3"), 1111));
                    ShowInfo(store);
                    Console.ReadLine();

                    store.Unregister("xpto", new IPEndPoint(IPAddress.Parse("193.1.2.3"), 1111));
                    store.Unregister("xpto", new IPEndPoint(IPAddress.Parse("194.1.2.3"), 1111));
                    ShowInfo(store);
                    Console.ReadLine();
                }
        */


        /// <summary>
        ///	Application's starting point. Starts a tracking server that listens at the TCP port 
        ///	specified as a command line argument.
        /// </summary>
        public static void Main(string[] args)
        {
            // Checking command line arguments
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: {0} <TCPPortNumber>", AppDomain.CurrentDomain.FriendlyName);
                Environment.Exit(1);
            }

            ushort port;
            if (!ushort.TryParse(args[0], out port))
            {
                Console.WriteLine("Usage: {0} <TCPPortNumber>", AppDomain.CurrentDomain.FriendlyName);
                return;
            }

            // Start servicing
            Logger log = new Logger();
            log.Start();
            try
            {
                new Listener(port,10).Run(log);
            }
            finally
            {
                log.Stop();
            }
        }
    }
}
