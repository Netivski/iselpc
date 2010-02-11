using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace MultiThreadGUIClient
{
    public partial class Client : Form
    {

        public Client()
        {
            InitializeComponent();
        }

        private volatile TcpClient socket;

        private void LogMessage(string value)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.BeginInvoke(new Action<string>(LogMessage), value);
                return;
            }
            txtLog.AppendText(value + "\n");
        }

        private bool VerifyIpPortPair(TextBox txtIp, TextBox txtPort, out IPAddress ipAddress, out ushort port)
        {
            ipAddress = null;
            port = 0;
            if (!IPAddress.TryParse(txtIp.Text, out ipAddress))
                return false;
            if (!ushort.TryParse(txtPort.Text, out port))
                return false;
            return true;
        }

        #region Connection
        private void StartConnect(Action<TcpClient> callback)
        {
            IPAddress ipAddress;
            ushort port;
            if (!VerifyIpPortPair(txtServerIp, txtServerPort, out ipAddress, out port))
            {
                LogMessage("MTGC - Please check location of tracking server.");
                return;
            }
            socket = new TcpClient();
            StateObject<TcpClient> state = new StateObject<TcpClient>(socket, callback);
            socket.BeginConnect(ipAddress, port, EndConnect, state);
            LogMessage(String.Format("MTGC - Attempting connection @ {0}:{1}", ipAddress.ToString(), port));
        }

        private void EndConnect(IAsyncResult iaR)
        {
            StateObject<TcpClient> state = (StateObject<TcpClient>)iaR.AsyncState;
            try
            {
                state.Socket.EndConnect(iaR);
                LogMessage(String.Format("MTGC - Connection established @ {0}", state.Socket.Client.RemoteEndPoint));
            }
            catch (SocketException)
            {
                LogMessage("MTGC - Error establishing connection!");
                return;
            }
            state.Callback(state.Socket);
        }
        #endregion

        #region Register Message
        private void btnRegAddFile_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtRegFile.Text))
            {
                lstRegFiles.Items.Add(txtRegFile.Text);
                txtRegFile.Text = "";
            }
            else
                LogMessage("Register - Filename must have a length.");
        }

        private void btnRegSend_Click(object sender, EventArgs e)
        {
            IPAddress ipAddress;
            ushort port;
            if (!VerifyIpPortPair(txtRegIp, txtRegPort, out ipAddress, out port))
            {
                LogMessage("Register - Please check location of files.");
                return;
            }
            StartConnect(RequestRegister);
        }

        private void RequestRegister(TcpClient socket)
        {
            List<string> files = new List<string>();
            foreach (string file in lstRegFiles.CheckedItems)
            {
                files.Add(file);
                LogMessage(String.Format("Register - Sent '{0}' from tracking @ {1}:{2}", file, txtRegIp.Text, txtRegPort.Text));
            }
            try
            {
                Handler.SendRegister(socket, files, txtRegIp.Text, txtRegPort.Text);
            }
            catch (IOException) { LogMessage("Register - Connection closed by server."); }
        }
        #endregion

        #region Unregister Message
        private void btnUnregSend_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtUnregFile.Text))
            {
                LogMessage("Unregister - Please check file name.");
                return;
            }
            IPAddress ipAddress;
            ushort port;
            if (!VerifyIpPortPair(txtRegIp, txtRegPort, out ipAddress, out port))
            {
                LogMessage("Unregister - Please check location of file.");
                return;
            }
            if (!VerifyIpPortPair(txtServerIp, txtServerPort, out ipAddress, out port))
            {
                LogMessage("Unregister - Please check location of tracking server.");
                return;
            }
            StartConnect(RequestUnregister);
        }

        private void RequestUnregister(TcpClient socket)
        {
            string file = txtUnregFile.Text;
            LogMessage(String.Format("Unregister - Removed '{0}' @ {1}:{2}", file, txtRegIp.Text, txtRegPort.Text));
            try
            {
                Handler.SendUnregister(socket, file, txtUnregIp.Text, txtUnregPort.Text);
            }
            catch (IOException) { LogMessage("Unregister - Connection closed by server."); }
        }
        #endregion

        #region List Files Message
        private void btnListFilesSend_Click(object sender, EventArgs e)
        {
            IPAddress ipAddress;
            ushort port;
            if (!VerifyIpPortPair(txtServerIp, txtServerPort, out ipAddress, out port))
            {
                LogMessage("ListFiles - Please check location of tracking server.");
                return;
            }
            StartConnect(RequestListFiles);
        }

        private void RequestListFiles(TcpClient socket)
        {
            LogMessage("ListFiles - Sent request for file list.");
            try
            {
                Handler.SendListFiles(socket, UpdateListFiles);
            }
            catch (IOException) { LogMessage("ListFiles - Connection closed by server."); }
        }

        private void UpdateListFiles(string response)
        {
            if (lstServerFiles.InvokeRequired)
            {
                lstServerFiles.BeginInvoke(new Action<string>(UpdateListFiles), response);
                return;
            }
            StringReader files = new StringReader(response);
            string line;
            lstServerFiles.Items.Clear();
            while ((line = files.ReadLine()) != null)
                lstServerFiles.Items.Add(line);

        }
        #endregion

        #region List Locations Message
        private void btnListLocsSend_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtFileLocs.Text))
            {
                LogMessage("ListLocations - Filename must have a length.");
                return;
            }
            IPAddress ipAddress;
            ushort port;
            if (!VerifyIpPortPair(txtServerIp, txtServerPort, out ipAddress, out port))
            {
                LogMessage("ListLocations - Please check location of tracking server.");
                return;
            }
            StartConnect(RequestListLocs);
        }

        private void RequestListLocs(TcpClient socket)
        {
            string file = txtFileLocs.Text;
            LogMessage(String.Format("ListLocations - Sent request for locations of '{0}'.", file));
            try
            {
                Handler.SendListLocs(socket, file, UpdateListLocs);
            }
            catch (IOException) { LogMessage("ListLocations - Connection closed by server."); }
        }

        private void UpdateListLocs(string response)
        {
            if (lstFileLocs.InvokeRequired)
            {
                lstFileLocs.BeginInvoke(new Action<string>(UpdateListLocs), response);
                return;
            }
            StringReader files = new StringReader(response);
            string line;
            lstFileLocs.Items.Clear();
            while ((line = files.ReadLine()) != null)
                lstFileLocs.Items.Add(line);
        }
        #endregion
    }
}