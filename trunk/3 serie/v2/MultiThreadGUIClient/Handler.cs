using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using Utils;
using System.IO;

namespace MultiThreadGUIClient
{
    internal sealed class StateObject
    {
        public const int BufferSize = 256;

        public readonly TcpClient Socket;
        public readonly byte[] Buffer;
        public readonly StringBuilder Content;
        public readonly Action<string> Callback;

        public StateObject(TcpClient cSocket, Action<string> cCallback)
        {
            Socket = cSocket;
            Callback = cCallback;
            Buffer = new byte[BufferSize];
            Content = new StringBuilder();
        }

        public NetworkStream Stream { get { return Socket.GetStream(); } }
    }

    public static class Handler
    {
        public static void SendRegister(TcpClient socket, string file, string ip, string port)
        {
            string message = "REGISTER\r\n" + file + ":" + ip + ":" + port + "\r\n\r\n";
            SendMessage(socket, message);
        }

        public static void SendUnregister(TcpClient socket, string file, string ip, string port)
        {
            string message = "UNREGISTER\r\n" + file + ":" + ip + ":" + port + "\r\n\r\n";
            SendMessage(socket, message);
        }

        public static void SendCloseConnection(TcpClient socket)
        {
            byte[] buffer = Encoding.ASCII.GetBytes("QUIT\r\n\r\n");
            socket.GetStream().BeginWrite(buffer, 0, buffer.Length,
                Handler.CloseConnectionCallback, socket);
        }

        private static void SendMessage(TcpClient socket, string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            socket.GetStream().BeginWrite(buffer, 0, buffer.Length,
                Handler.WriteDataCallback, socket);
        }

        public static void WriteDataCallback(IAsyncResult iaR)
        {
            TcpClient socket = (TcpClient)iaR.AsyncState;
            socket.GetStream().EndWrite(iaR);
        }

        public static void SendListFiles(TcpClient socket, Action<string> callback)
        {
            string message = "LIST_FILES\r\n\r\n";
            byte[] buffer = Encoding.ASCII.GetBytes(message);

            StateObject state = new StateObject(socket, callback);
            state.Stream.BeginWrite(buffer, 0, buffer.Length,
                Handler.RequestFilesCallback, state);
        }

        public static void RequestFilesCallback(IAsyncResult iaR)
        {
            StateObject state = (StateObject)iaR.AsyncState;
            state.Stream.EndWrite(iaR);
            state.Stream.BeginRead(state.Buffer, 0, StateObject.BufferSize,
                new AsyncCallback(ReadFilesCallback), state);

        }

        public static void ReadFilesCallback(IAsyncResult iaR)
        {
            StateObject state = (StateObject)iaR.AsyncState;
            int bytesRead = state.Stream.EndRead(iaR);
            if (bytesRead > 0)
            {
                state.Content.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
                state.Stream.BeginRead(state.Buffer, 0, StateObject.BufferSize,
                            new AsyncCallback(ReadFilesCallback), state);
            }
            else {
                state.Callback.BeginInvoke(state.Content.ToString(),null,null);
            }
        }

        public static void CloseConnectionCallback(IAsyncResult iaR)
        {
            TcpClient socket = (TcpClient)iaR.AsyncState;
            socket.GetStream().EndWrite(iaR);
            socket.Close();
        }
    }
}
