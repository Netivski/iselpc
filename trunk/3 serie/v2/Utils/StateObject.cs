using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace Utils
{
    public class StateObject
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
}
