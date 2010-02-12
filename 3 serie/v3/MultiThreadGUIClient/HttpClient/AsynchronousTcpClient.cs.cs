using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace MultiThreadGUIClient.HttpClient
{
    internal class AsynchronousTcpClient
    {
        const int BufferSize = 256;

        public EventHandler EndRequest    = null;

        volatile Socket        workSocket = null;
        volatile byte[]        buffer     = null;

        readonly StringReader  dataReader = null;
        readonly IPEndPoint    remoteEp   = null;
        readonly StringBuilder output     = null;


        public AsynchronousTcpClient(IPEndPoint remoteEp, StringReader dataReader )
        {
            if (remoteEp   == null) throw new ArgumentNullException("remoteEp");
            if (dataReader == null) throw new ArgumentNullException("dataReader");

            this.remoteEp   = remoteEp;
            this.dataReader = dataReader;
            this.output     = new StringBuilder();
        }

        public AsynchronousTcpClient(string hostName, int port, StringReader dataReader) : this(new IPEndPoint(Dns.GetHostEntry(hostName).AddressList[0], port), dataReader) { }


        public void Start()
        {
            try
            {
                workSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                workSocket.BeginConnect(remoteEp, new AsyncCallback(ConnectCallback), this);
            }
            catch (Exception e)
            {
                if (EndRequest != null) EndRequest.Invoke(this, new EndRequestArgs(e.ToString(), RequestState.Error));
            }
        }

        public void Stop()
        {
            if (workSocket.Connected)
            {
                workSocket.Shutdown(SocketShutdown.Both);
                workSocket.Close();
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                workSocket.EndConnect(ar);
                Send(dataReader.ReadToEnd());
            }
            catch (Exception e)
            {
                if (EndRequest != null) EndRequest.Invoke(this, new EndRequestArgs(e.ToString(), RequestState.Error));
            }
        }

        private void Receive()
        {
            try
            {
                buffer = new byte[BufferSize];

                workSocket.BeginReceive(buffer, 0, BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), this);
            }
            catch (Exception e)
            {
                if (EndRequest != null) EndRequest.Invoke(this, new EndRequestArgs(e.ToString(), RequestState.Error));
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int bytesRead = workSocket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    output.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));

                    workSocket.BeginReceive(buffer, 0, BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), this);
                }
                else
                {
                    if (EndRequest != null) EndRequest.Invoke(this, new EndRequestArgs(output.ToString(), RequestState.Success));
                }
            }
            catch (Exception e)
            {
                if (EndRequest != null) EndRequest.Invoke(this, new EndRequestArgs(e.ToString(), RequestState.Error));
            }
        }

        private void Send(String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            workSocket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), this);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                int bytesSent = workSocket.EndSend(ar);

                Receive();
            }
            catch (Exception e)
            {
                if (EndRequest != null) EndRequest.Invoke(this, new EndRequestArgs(e.ToString(), RequestState.Error));
            }
        }
    }
}
