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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(207, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(305, 22);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
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
            this.txtServerIp.Location = new System.Drawing.Point(107, 24);
            this.txtServerIp.Name = "txtServerIp";
            this.txtServerIp.Size = new System.Drawing.Size(91, 20);
            this.txtServerIp.TabIndex = 14;
            this.txtServerIp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtServerPort
            // 
            this.txtServerPort.Location = new System.Drawing.Point(239, 24);
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
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(32, 72);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(375, 237);
            this.tabControl1.TabIndex = 18;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnRegAddFile);
            this.tabPage1.Controls.Add(this.txtRegPort);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.txtRegIp);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.lstRegFiles);
            this.tabPage1.Controls.Add(this.txtRegFile);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.txtRegSend);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(367, 211);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Register";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 437);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.txtServerPort);
            this.Controls.Add(this.txtServerIp);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Client";
            this.Text = "MultiThreadGUIClient";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
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
        private System.Windows.Forms.TabPage tabPage1;
    }
}

