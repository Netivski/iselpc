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

        private TcpClient socket = new TcpClient();

        public Client()
        {
            InitializeComponent();
        }

        #region Private Members

        private void LogMessage(string value)
        {
            txtLog.AppendText(value + "\n");
        }

        private bool VerifyIpPortPair(TextBox txtIp, TextBox txtPort, out IPAddress ipAddress, out ushort port)
        {
            ipAddress = null;
            port = 0;
            if (!IPAddress.TryParse(txtIp.Text, out ipAddress))
            {
                LogMessage("Client - Invalid server IP.");
                return false;
            }
            if (!ushort.TryParse(txtPort.Text, out port))
            {
                LogMessage("Client - Invalid server port.");
                return false;
            }
            return true;
        }
        #endregion

        #region Registration Message

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
            if (socket == null || !socket.Connected)
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
                //string message = file + ":" + txtRegIp.Text + ":" + txtRegPort.Text;
                try
                {
                    Handler.SendRegister(socket, file, txtRegIp.Text, txtRegPort.Text);
                    LogMessage("Sent file '" + file + "' @ " + txtRegIp.Text + ":" + txtRegPort.Text);
                }
                catch (IOException) { LogMessage("Register - Connection closed by server."); }
            }
        }

        #endregion

        #region Connection
        private void btnConnect_Click(object sender, EventArgs e)
        {
            IPAddress ipAddress;
            ushort port;
            if (!VerifyIpPortPair(txtServerIp, txtServerPort, out ipAddress, out port))
            {
                return;
            }
            socket = new TcpClient();
            socket.Connect(ipAddress, port);
        }
        #endregion
    }
}
