namespace MultiThreadGUIClient
{
    partial class Client
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lstRegFiles = new System.Windows.Forms.CheckedListBox();
            this.txtRegFile = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnRegAddFile = new System.Windows.Forms.Button();
            this.txtRegSend = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.txtServerIp = new System.Windows.Forms.TextBox();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.txtRegIp = new System.Windows.Forms.TextBox();
            this.txtRegPort = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabRegister = new System.Windows.Forms.TabPage();
            this.tabUnregister = new System.Windows.Forms.TabPage();
            this.txtUnregPort = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnUnregSend = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtUnregFile = new System.Windows.Forms.TextBox();
            this.txtUnregIp = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.tabListFiles = new System.Windows.Forms.TabPage();
            this.btnListFilesSend = new System.Windows.Forms.Button();
            this.lstServerFiles = new System.Windows.Forms.ListBox();
            this.tabControl1.SuspendLayout();
            this.tabRegister.SuspendLayout();
            this.tabUnregister.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabListFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(162, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(248, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(65, 23);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(218, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Port";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(51, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "IP Address";
            // 
            // lstRegFiles
            // 
            this.lstRegFiles.FormattingEnabled = true;
            this.lstRegFiles.Items.AddRange(new object[] {
            "foo.txt",
            "bar.txt",
            "xpto.txt",
            "xpty.txt",
            "xptz.txt"});
            this.lstRegFiles.Location = new System.Drawing.Point(86, 69);
            this.lstRegFiles.Name = "lstRegFiles";
            this.lstRegFiles.Size = new System.Drawing.Size(120, 79);
            this.lstRegFiles.TabIndex = 8;
            // 
            // txtRegFile
            // 
            this.txtRegFile.Location = new System.Drawing.Point(212, 128);
            this.txtRegFile.Name = "txtRegFile";
            this.txtRegFile.Size = new System.Drawing.Size(89, 20);
            this.txtRegFile.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(52, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Files";
            // 
            // btnRegAddFile
            // 
            this.btnRegAddFile.Location = new System.Drawing.Point(212, 99);
            this.btnRegAddFile.Name = "btnRegAddFile";
            this.btnRegAddFile.Size = new System.Drawing.Size(89, 23);
            this.btnRegAddFile.TabIndex = 11;
            this.btnRegAddFile.Text = "Add file to list";
            this.btnRegAddFile.UseVisualStyleBackColor = true;
            this.btnRegAddFile.Click += new System.EventHandler(this.btnRegAddFile_Click);
            // 
            // txtRegSend
            // 
            this.txtRegSend.Location = new System.Drawing.Point(55, 158);
            this.txtRegSend.Name = "txtRegSend";
            this.txtRegSend.Size = new System.Drawing.Size(246, 21);
            this.txtRegSend.TabIndex = 12;
            this.txtRegSend.Text = "Send REGISTER command";
            this.txtRegSend.UseVisualStyleBackColor = true;
            this.txtRegSend.Click += new System.EventHandler(this.txtRegSend_Click);
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.Black;
            this.txtLog.ForeColor = System.Drawing.Color.Lime;
            this.txtLog.HideSelection = false;
            this.txtLog.Location = new System.Drawing.Point(32, 331);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(375, 76);
            this.txtLog.TabIndex = 13;
            // 
            // txtServerIp
            // 
            this.txtServerIp.Location = new System.Drawing.Point(62, 14);
            this.txtServerIp.Name = "txtServerIp";
            this.txtServerIp.Size = new System.Drawing.Size(91, 20);
            this.txtServerIp.TabIndex = 14;
            this.txtServerIp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtServerPort
            // 
            this.txtServerPort.Location = new System.Drawing.Point(190, 14);
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(51, 20);
            this.txtServerPort.TabIndex = 15;
            this.txtServerPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtRegIp
            // 
            this.txtRegIp.Location = new System.Drawing.Point(115, 32);
            this.txtRegIp.Name = "txtRegIp";
            this.txtRegIp.Size = new System.Drawing.Size(91, 20);
            this.txtRegIp.TabIndex = 16;
            this.txtRegIp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtRegPort
            // 
            this.txtRegPort.Location = new System.Drawing.Point(250, 32);
            this.txtRegPort.Name = "txtRegPort";
            this.txtRegPort.Size = new System.Drawing.Size(51, 20);
            this.txtRegPort.TabIndex = 17;
            this.txtRegPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabRegister);
            this.tabControl1.Controls.Add(this.tabUnregister);
            this.tabControl1.Controls.Add(this.tabListFiles);
            this.tabControl1.Location = new System.Drawing.Point(32, 72);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(375, 237);
            this.tabControl1.TabIndex = 18;
            // 
            // tabRegister
            // 
            this.tabRegister.BackColor = System.Drawing.Color.White;
            this.tabRegister.Controls.Add(this.btnRegAddFile);
            this.tabRegister.Controls.Add(this.txtRegPort);
            this.tabRegister.Controls.Add(this.label4);
            this.tabRegister.Controls.Add(this.txtRegIp);
            this.tabRegister.Controls.Add(this.label3);
            this.tabRegister.Controls.Add(this.lstRegFiles);
            this.tabRegister.Controls.Add(this.txtRegFile);
            this.tabRegister.Controls.Add(this.label5);
            this.tabRegister.Controls.Add(this.txtRegSend);
            this.tabRegister.Location = new System.Drawing.Point(4, 22);
            this.tabRegister.Name = "tabRegister";
            this.tabRegister.Padding = new System.Windows.Forms.Padding(3);
            this.tabRegister.Size = new System.Drawing.Size(367, 211);
            this.tabRegister.TabIndex = 0;
            this.tabRegister.Text = "Register";
            // 
            // tabUnregister
            // 
            this.tabUnregister.Controls.Add(this.txtUnregPort);
            this.tabUnregister.Controls.Add(this.label8);
            this.tabUnregister.Controls.Add(this.btnUnregSend);
            this.tabUnregister.Controls.Add(this.label6);
            this.tabUnregister.Controls.Add(this.txtUnregFile);
            this.tabUnregister.Controls.Add(this.txtUnregIp);
            this.tabUnregister.Controls.Add(this.label7);
            this.tabUnregister.Location = new System.Drawing.Point(4, 22);
            this.tabUnregister.Name = "tabUnregister";
            this.tabUnregister.Padding = new System.Windows.Forms.Padding(3);
            this.tabUnregister.Size = new System.Drawing.Size(367, 211);
            this.tabUnregister.TabIndex = 1;
            this.tabUnregister.Text = "Unregister";
            this.tabUnregister.UseVisualStyleBackColor = true;
            // 
            // txtUnregPort
            // 
            this.txtUnregPort.Location = new System.Drawing.Point(250, 80);
            this.txtUnregPort.Name = "txtUnregPort";
            this.txtUnregPort.Size = new System.Drawing.Size(51, 20);
            this.txtUnregPort.TabIndex = 24;
            this.txtUnregPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(51, 51);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Filename";
            // 
            // btnUnregSend
            // 
            this.btnUnregSend.Location = new System.Drawing.Point(55, 118);
            this.btnUnregSend.Name = "btnUnregSend";
            this.btnUnregSend.Size = new System.Drawing.Size(246, 21);
            this.btnUnregSend.TabIndex = 22;
            this.btnUnregSend.Text = "Send UNREGISTER command";
            this.btnUnregSend.UseVisualStyleBackColor = true;
            this.btnUnregSend.Click += new System.EventHandler(this.btnUnregSend_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(51, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "IP Address";
            // 
            // txtUnregFile
            // 
            this.txtUnregFile.Location = new System.Drawing.Point(115, 48);
            this.txtUnregFile.Name = "txtUnregFile";
            this.txtUnregFile.Size = new System.Drawing.Size(186, 20);
            this.txtUnregFile.TabIndex = 21;
            // 
            // txtUnregIp
            // 
            this.txtUnregIp.Location = new System.Drawing.Point(115, 80);
            this.txtUnregIp.Name = "txtUnregIp";
            this.txtUnregIp.Size = new System.Drawing.Size(91, 20);
            this.txtUnregIp.TabIndex = 23;
            this.txtUnregIp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(218, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(26, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Port";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnDisconnect);
            this.panel1.Controls.Add(this.btnConnect);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtServerPort);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtServerIp);
            this.panel1.Location = new System.Drawing.Point(32, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(374, 54);
            this.panel1.TabIndex = 19;
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(318, 12);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(47, 23);
            this.btnDisconnect.TabIndex = 16;
            this.btnDisconnect.Text = "Close";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // tabListFiles
            // 
            this.tabListFiles.Controls.Add(this.lstServerFiles);
            this.tabListFiles.Controls.Add(this.btnListFilesSend);
            this.tabListFiles.Location = new System.Drawing.Point(4, 22);
            this.tabListFiles.Name = "tabListFiles";
            this.tabListFiles.Padding = new System.Windows.Forms.Padding(3);
            this.tabListFiles.Size = new System.Drawing.Size(367, 211);
            this.tabListFiles.TabIndex = 2;
            this.tabListFiles.Text = "List Files";
            this.tabListFiles.UseVisualStyleBackColor = true;
            // 
            // btnListFilesSend
            // 
            this.btnListFilesSend.Location = new System.Drawing.Point(60, 173);
            this.btnListFilesSend.Name = "btnListFilesSend";
            this.btnListFilesSend.Size = new System.Drawing.Size(246, 21);
            this.btnListFilesSend.TabIndex = 23;
            this.btnListFilesSend.Text = "Send LIST_FILES command";
            this.btnListFilesSend.UseVisualStyleBackColor = true;
            this.btnListFilesSend.Click += new System.EventHandler(this.btnListFilesSend_Click);
            // 
            // lstServerFiles
            // 
            this.lstServerFiles.FormattingEnabled = true;
            this.lstServerFiles.Location = new System.Drawing.Point(60, 29);
            this.lstServerFiles.MultiColumn = true;
            this.lstServerFiles.Name = "lstServerFiles";
            this.lstServerFiles.Size = new System.Drawing.Size(246, 134);
            this.lstServerFiles.TabIndex = 24;
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 437);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.txtLog);
            this.Name = "Client";
            this.Text = "MultiThreadGUIClient";
            this.tabControl1.ResumeLayout(false);
            this.tabRegister.ResumeLayout(false);
            this.tabRegister.PerformLayout();
            this.tabUnregister.ResumeLayout(false);
            this.tabUnregister.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabListFiles.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckedListBox lstRegFiles;
        private System.Windows.Forms.TextBox txtRegFile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnRegAddFile;
        private System.Windows.Forms.Button txtRegSend;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TextBox txtServerIp;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.TextBox txtRegIp;
        private System.Windows.Forms.TextBox txtRegPort;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabRegister;
        private System.Windows.Forms.TabPage tabUnregister;
        private System.Windows.Forms.TextBox txtUnregPort;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnUnregSend;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtUnregFile;
        private System.Windows.Forms.TextBox txtUnregIp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.TabPage tabListFiles;
        private System.Windows.Forms.ListBox lstServerFiles;
        private System.Windows.Forms.Button btnListFilesSend;
    }
}

