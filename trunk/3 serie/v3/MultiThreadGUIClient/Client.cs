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
using MultiThreadGUIClient.HttpClient;

namespace MultiThreadGUIClient
{
    public partial class Client : Form
    {

        public Client()
        {
            InitializeComponent();

            txtServerPort.TextChanged += OnPortChanged; //Optimização para detecção de alterações.
        }

        void OnPortChanged(object sender, EventArgs args)
        {
            port = int.MinValue;
        }

        string RemoteHostName
        {
            get
            {
                return txtServerIp.Text;
            }
        }


        int port = int.MinValue;
        int RemotePort
        {
            get
            {
                if (port == int.MinValue) int.TryParse(txtServerPort.Text, out port);

                return port;
            }
        }



        private void LogMessage(string value)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.BeginInvoke(new Action<string>(LogMessage), value);
                return;
            }
            txtLog.AppendText(value + "\n");
        }

        void ListBoxAddEntry(ListBox ctrl, string entry)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.BeginInvoke(new Action<ListBox, string>(ListBoxAddEntry), ctrl, entry);
                return;
            }

            ctrl.Items.Add(entry);
        }

        void LstServerFilesAddEntry( string entry )
        {
            ListBoxAddEntry(lstServerFiles, entry);
        }

        void LstFileLocsAddEntry(string entry)
        {
            ListBoxAddEntry(lstFileLocs, entry);
        }       
        

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
            IPAddress localIp = Dns.GetHostEntry(txtRegIp.Text).AddressList[0];
            int localPort;
            int.TryParse(txtRegPort.Text, out localPort);


            StringBuilder message = new StringBuilder();
            message.Append("REGISTER\r\n");
            foreach (string file in lstRegFiles.CheckedItems) message.AppendFormat("{0}:{1}:{2}\r\n", file, localIp, localPort);
            message.Append("\r\n\r\n");

            AsynchronousTcpClient tcpClient = new AsynchronousTcpClient(RemoteHostName, RemotePort, new StringReader(message.ToString()));
            tcpClient.EndRequest += SendRegisterEndRequest;
            tcpClient.Start();
        }

        void SendRegisterEndRequest(object sender, EventArgs args)
        {
            ((AsynchronousTcpClient)sender).Stop();

            EndRequestArgs endReq   = (EndRequestArgs)args;
            LogMessage(endReq.State == RequestState.Success ? "Register, runs successufuly": string.Format("Register, error message ( {0} )", endReq.Content ) );

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

            IPAddress localIp = Dns.GetHostEntry(txtUnregIp.Text).AddressList[0];
            int localPort;
            int.TryParse(txtUnregPort.Text, out localPort);

            StringBuilder message = new StringBuilder();
            message.Append("UNREGISTER\r\n");
            message.AppendFormat("{0}:{1}:{2}\r\n", txtUnregFile.Text, localIp, localPort);
            message.Append("\r\n");

            AsynchronousTcpClient tcpClient = new AsynchronousTcpClient(RemoteHostName, RemotePort, new StringReader(message.ToString()));
            tcpClient.EndRequest += SendUnRegisterEndRequest;
            tcpClient.Start();
        }

        void SendUnRegisterEndRequest(object sender, EventArgs args)
        {
            ((AsynchronousTcpClient)sender).Stop();

            EndRequestArgs endReq = (EndRequestArgs)args;
            LogMessage(endReq.State == RequestState.Success ? "UnRegister, runs successufuly" : string.Format("UnRegister, error message ( {0} )", endReq.Content));
        }

        #endregion

        #region List Files Message
        private void btnListFilesSend_Click(object sender, EventArgs e)
        {
            IPAddress localIp = Dns.GetHostEntry(txtUnregIp.Text).AddressList[0];
            int localPort;
            int.TryParse(txtUnregPort.Text, out localPort);

            StringBuilder message = new StringBuilder();
            message.Append("LIST_FILES\r\n");
            message.Append("\r\n");

            AsynchronousTcpClient tcpClient = new AsynchronousTcpClient(RemoteHostName, RemotePort, new StringReader(message.ToString()));
            tcpClient.EndRequest += ListFilesSendEndRequest;
            tcpClient.Start();
        }

        void ListFilesSendEndRequest(object sender, EventArgs args)
        {
            ((AsynchronousTcpClient)sender).Stop();

            EndRequestArgs endReq = (EndRequestArgs)args;
            LogMessage(endReq.State == RequestState.Success ? "List Files, runs successufuly" : string.Format("List Files, error message ( {0} )", endReq.Content));

            if (endReq.State == RequestState.Success)
            {
                StringReader sr = new StringReader(endReq.Content);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    LstServerFilesAddEntry(line);
                }
            }            
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

            IPAddress localIp = Dns.GetHostEntry(txtUnregIp.Text).AddressList[0];
            int localPort;
            int.TryParse(txtUnregPort.Text, out localPort);

            StringBuilder message = new StringBuilder();
            message.Append("LIST_LOCATIONS\r\n");
            message.AppendFormat("{0}\r\n\r\n", txtFileLocs.Text);

            AsynchronousTcpClient tcpClient = new AsynchronousTcpClient(RemoteHostName, RemotePort, new StringReader(message.ToString()));
            tcpClient.EndRequest += ListLocsSendEndRequest;
            tcpClient.Start();
        }

        void ListLocsSendEndRequest(object sender, EventArgs args)
        {
            ((AsynchronousTcpClient)sender).Stop();

            EndRequestArgs endReq = (EndRequestArgs)args;
            LogMessage(endReq.State == RequestState.Success ? "List Locations, runs successufuly" : string.Format("List Locations, error message ( {0} )", endReq.Content));

            if (endReq.State == RequestState.Success)
            {
                StringReader sr = new StringReader(endReq.Content);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    LstFileLocsAddEntry(line);
                }
            }
        }
       #endregion
    }
}