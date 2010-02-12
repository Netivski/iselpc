using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;
using MultiThreadGUIClient.HttpClient;

namespace MultiThreadGUIClient
{    
    //public static class Handler
    //{
    //    public static void SendRegister(string hostName, int port, IEnumerable<string> files)
    //    {
    //        StringBuilder message = new StringBuilder();
    //        message.Append( "REGISTER\r\n" );
    //        foreach (string file in files) message.AppendFormat("{0}:{1}:{2}\r\n", file, hostName, port);
    //        message.Append("\r\n");

    //        AsynchronousTcpClient tcpClient = new AsynchronousTcpClient(hostName, port, new StringReader(message.ToString()));
    //        tcpClient.EndRequest += SendRegisterEndRequest;
    //        tcpClient.Start();
    //    }

    //    static void SendRegisterEndRequest(object sender, EventArgs args)
    //    {
    //        ((EndRequestArgs)args).Contect
    //    }

    //    public static void SendUnregister(TcpClient socket, string file, string ip, string port)
    //    {
    //        string message = "UNREGISTER\r\n" + file + ":" + ip + ":" + port + "\r\n\r\n";
    //        SendMessage(socket, message);
    //    }

    //    public static void SendCloseConnection(TcpClient socket)
    //    {
    //        byte[] buffer = Encoding.ASCII.GetBytes("QUIT\r\n\r\n");
    //        socket.GetStream().BeginWrite(buffer, 0, buffer.Length,
    //            Handler.CloseConnectionCallback, socket);
    //    }

    //    private static void SendMessage(TcpClient socket, string message)
    //    {
    //        byte[] buffer = Encoding.ASCII.GetBytes(message);
    //        socket.GetStream().BeginWrite(buffer, 0, buffer.Length,
    //            Handler.WriteDataCallback, socket);
    //    }

    //    public static void WriteDataCallback(IAsyncResult iaR)
    //    {
    //        TcpClient socket = (TcpClient)iaR.AsyncState;
    //        socket.GetStream().EndWrite(iaR);
    //        socket.Close();
    //    }

    //    public static void SendListFiles(TcpClient socket, Action<string> callback)
    //    {
    //        string message = "LIST_FILES\r\n\r\n";
    //        byte[] buffer = Encoding.ASCII.GetBytes(message);

    //        StateObject<string> state = new StateObject<string>(socket, callback);
    //        state.Stream.BeginWrite(buffer, 0, buffer.Length,
    //            Handler.RequestFilesCallback, state);
    //    }

    //    public static void RequestFilesCallback(IAsyncResult iaR)
    //    {
    //        StateObject<string> state = (StateObject<string>)iaR.AsyncState;
    //        state.Stream.EndWrite(iaR);
    //        state.Stream.BeginRead(state.Buffer, 0, StateObject<string>.BufferSize,
    //            new AsyncCallback(ReadFilesCallback), state);
    //    }

    //    public static void ReadFilesCallback(IAsyncResult iaR)
    //    {
    //        StateObject<string> state = (StateObject<string>)iaR.AsyncState;
    //        int bytesRead = state.Stream.EndRead(iaR);
    //        if (bytesRead > 0)
    //        {
    //            state.Content.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
    //            state.Stream.BeginRead(state.Buffer, 0, StateObject<string>.BufferSize,
    //                        new AsyncCallback(ReadFilesCallback), state);
    //        }
    //        else
    //        {
    //            state.Callback(state.Content.ToString());
    //        }
    //    }

    //    public static void SendListLocs(TcpClient socket, string file, Action<string> callback)
    //    {
    //        string message = String.Format("LIST_LOCATIONS\r\n{0}\r\n\r\n", file);
    //        byte[] buffer = Encoding.ASCII.GetBytes(message);

    //        StateObject<string> state = new StateObject<string>(socket, callback);
    //        state.Stream.BeginWrite(buffer, 0, buffer.Length,
    //            Handler.RequestLocsCallback, state);
    //    }

    //    public static void RequestLocsCallback(IAsyncResult iaR)
    //    {
    //        StateObject<string> state = (StateObject<string>)iaR.AsyncState;
    //        state.Stream.EndWrite(iaR);
    //        state.Stream.BeginRead(state.Buffer, 0, StateObject<string>.BufferSize,
    //            new AsyncCallback(ReadLocsCallback), state);
    //    }

    //    public static void ReadLocsCallback(IAsyncResult iaR)
    //    {
    //        StateObject<string> state = (StateObject<string>)iaR.AsyncState;
    //        int bytesRead = state.Stream.EndRead(iaR);
    //        if (bytesRead > 0)
    //        {
    //            state.Content.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
    //            state.Stream.BeginRead(state.Buffer, 0, StateObject<string>.BufferSize,
    //                        new AsyncCallback(ReadFilesCallback), state);
    //        }
    //        else
    //        {
    //            state.Callback.BeginInvoke(state.Content.ToString(), null, null);
    //        }
    //    }

    //    public static void CloseConnectionCallback(IAsyncResult iaR)
    //    {
    //        TcpClient socket = (TcpClient)iaR.AsyncState;
    //        socket.GetStream().EndWrite(iaR);
    //        socket.Close();
    //    }
    //}
}
