using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace MultiThreadGUIClient
{
    public static class Handler
    {
        public static void SendRegister(TcpClient socket, string file, string ip, string port)
        {
            string message = "REGISTER\r\n" + file + ":" + ip + ":" + port + "\r\n\r\n";
            SendMessage(socket, message);
        }

        public static void SendUnRegister(TcpClient socket, string message)
        {
            message = "UNREGISTER\r\n" + message + "\r\n\r\n";
            SendMessage(socket, message);
        }

        public static void SendListFiles(TcpClient socket)
        {
            string message = "LIST_FILES\r\n";
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            socket.GetStream().BeginWrite(buffer, 0, buffer.Length,
                Handler.WriteDataCallback, socket);
        }

        private static void SendMessage(TcpClient socket, string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            socket.GetStream().BeginWrite(buffer, 0, buffer.Length,
                Handler.WriteDataCallback, socket);
        }

        public static void WriteDataCallback(IAsyncResult iaR)
        {
            NetworkStream stream = (NetworkStream)iaR.AsyncState;
            stream.EndWrite(iaR);
        }
    }
}
