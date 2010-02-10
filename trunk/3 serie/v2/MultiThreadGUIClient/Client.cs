using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Utils;
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

        #region Private Members

        private TcpClient socket = new TcpClient();

        private void LogMessage(string value)
        {
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
        #endregion

        #region Connection
        private void btnConnect_Click(object sender, EventArgs e)
        {
            IPAddress ipAddress;
            ushort port;
            if (!VerifyIpPortPair(txtServerIp, txtServerPort, out ipAddress, out port))
            {
                LogMessage("MTGC - Please check server ip and port.");
                return;
            }
            socket = new TcpClient();
            socket.Connect(ipAddress, port);
            LogMessage(String.Format("MTGC - Connection established @ {0}:{1}."
                , ipAddress.ToString(), port));
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (socket.Connected)
            {
                Handler.SendCloseConnection(socket);
            }
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
                LogMessage("Register - Filename must have a length.\n");
        }

        private void txtRegSend_Click(object sender, EventArgs e)
        {
            if (!socket.Connected)
            {
                LogMessage("Register - No connection available.");
                return;
            }
            IPAddress ipAddress;
            ushort port;
            if (!VerifyIpPortPair(txtRegIp, txtRegPort, out ipAddress, out port))
            {
                LogMessage("Register - Please check file's ip and port.");
                return;
            }
            foreach (String file in lstRegFiles.CheckedItems)
            {
                try
                {
                    Handler.SendRegister(socket, file, txtRegIp.Text, txtRegPort.Text);
                    LogMessage("Register - Sent file '" + file + "' @ " + txtRegIp.Text + ":" + txtRegPort.Text);
                }
                catch (IOException) { LogMessage("Register - Connection closed by server."); }
            }
        }

        #endregion

        #region Unregister Message
        private void btnUnregSend_Click(object sender, EventArgs e)
        {
            if (!socket.Connected)
            {
                LogMessage("Unregister - No connection available.");
                return;
            }
            IPAddress ipAddress;
            ushort port;
            if (!VerifyIpPortPair(txtUnregIp, txtUnregPort, out ipAddress, out port))
            {
                LogMessage("Unregister - Please check file's ip and port.");
                return;
            }
            if (String.IsNullOrEmpty(txtUnregFile.Text))
            {
                LogMessage("Unregister - Please check file name.");
                return;
            }
            try
            {
                Handler.SendUnregister(socket, txtUnregFile.Text, txtUnregIp.Text, txtUnregPort.Text);
                LogMessage("Unregister - Removed file '" + txtUnregFile.Text + "' @ " + txtRegIp.Text + ":" + txtRegPort.Text);
            }
            catch (IOException) { LogMessage("Unregister - Connection closed by server."); }
        }
        #endregion

        #region List Files Message
        private void btnListFilesSend_Click(object sender, EventArgs e)
        {
            if (!socket.Connected)
            {
                LogMessage("ListFiles - No connection available.");
                return;
            }
            try
            {
                Handler.SendListFiles(socket, this.UpdateListFiles);
                LogMessage("ListFiles - Sent request for file list.");
            }
            catch (IOException) { LogMessage("ListFiles - Connection closed by server."); }
        }

        delegate void StringParameterDelegate (string value);

        private void UpdateListFiles(string response)
        {
            if (lstServerFiles.InvokeRequired)
            {
                lstServerFiles.BeginInvoke(new StringParameterDelegate(UpdateListFiles),response);
                return;
            }
            LogMessage("Arrived!");
            LogMessage(response);

        }
        #endregion

        #region List Locations Message
        #endregion
    }
}
