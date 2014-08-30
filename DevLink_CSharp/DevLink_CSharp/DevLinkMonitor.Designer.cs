namespace DevLink_CSharp
{
	partial class DevLinkMonitor
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
			this.buttonStartStop = new System.Windows.Forms.Button();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.buttonClear = new System.Windows.Forms.Button();
			this.tbIpAddress = new System.Windows.Forms.TextBox();
			this.tbPassword = new System.Windows.Forms.TextBox();
			this.lblIpAddress = new System.Windows.Forms.Label();
			this.lblPassword = new System.Windows.Forms.Label();
			this.lblConnTimeout = new System.Windows.Forms.Label();
			this.tbConnTimeout = new System.Windows.Forms.TextBox();
			this.groupBoxControls = new System.Windows.Forms.GroupBox();
			this.groupBoxControls.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonStartStop
			// 
			this.buttonStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonStartStop.Location = new System.Drawing.Point(630, 10);
			this.buttonStartStop.Name = "buttonStartStop";
			this.buttonStartStop.Size = new System.Drawing.Size(75, 23);
			this.buttonStartStop.TabIndex = 0;
			this.buttonStartStop.Text = "Start";
			this.buttonStartStop.UseVisualStyleBackColor = true;
			this.buttonStartStop.Click += new System.EventHandler(this.buttonStartStop_Click);
			// 
			// textBoxLog
			// 
			this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxLog.Location = new System.Drawing.Point(0, 81);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxLog.Size = new System.Drawing.Size(717, 269);
			this.textBoxLog.TabIndex = 2;
			// 
			// buttonClear
			// 
			this.buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClear.Location = new System.Drawing.Point(630, 52);
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.Size = new System.Drawing.Size(75, 23);
			this.buttonClear.TabIndex = 3;
			this.buttonClear.Text = "Clear";
			this.buttonClear.UseVisualStyleBackColor = true;
			this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
			// 
			// tbIpAddress
			// 
			this.tbIpAddress.Location = new System.Drawing.Point(103, 19);
			this.tbIpAddress.Name = "tbIpAddress";
			this.tbIpAddress.Size = new System.Drawing.Size(123, 20);
			this.tbIpAddress.TabIndex = 4;
			// 
			// tbPassword
			// 
			this.tbPassword.Location = new System.Drawing.Point(103, 45);
			this.tbPassword.Name = "tbPassword";
			this.tbPassword.Size = new System.Drawing.Size(123, 20);
			this.tbPassword.TabIndex = 5;
			// 
			// lblIpAddress
			// 
			this.lblIpAddress.AutoSize = true;
			this.lblIpAddress.Location = new System.Drawing.Point(12, 19);
			this.lblIpAddress.Name = "lblIpAddress";
			this.lblIpAddress.Size = new System.Drawing.Size(82, 13);
			this.lblIpAddress.TabIndex = 6;
			this.lblIpAddress.Text = "IPO IP Address:";
			// 
			// lblPassword
			// 
			this.lblPassword.AutoSize = true;
			this.lblPassword.Location = new System.Drawing.Point(12, 45);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(56, 13);
			this.lblPassword.TabIndex = 7;
			this.lblPassword.Text = "Password:";
			// 
			// lblConnTimeout
			// 
			this.lblConnTimeout.AutoSize = true;
			this.lblConnTimeout.Location = new System.Drawing.Point(241, 22);
			this.lblConnTimeout.Name = "lblConnTimeout";
			this.lblConnTimeout.Size = new System.Drawing.Size(105, 13);
			this.lblConnTimeout.TabIndex = 9;
			this.lblConnTimeout.Text = "Connection Timeout:";
			// 
			// tbConnTimeout
			// 
			this.tbConnTimeout.Location = new System.Drawing.Point(352, 21);
			this.tbConnTimeout.Name = "tbConnTimeout";
			this.tbConnTimeout.Size = new System.Drawing.Size(98, 20);
			this.tbConnTimeout.TabIndex = 8;
			// 
			// groupBoxControls
			// 
			this.groupBoxControls.Controls.Add(this.tbIpAddress);
			this.groupBoxControls.Controls.Add(this.lblConnTimeout);
			this.groupBoxControls.Controls.Add(this.lblPassword);
			this.groupBoxControls.Controls.Add(this.tbPassword);
			this.groupBoxControls.Controls.Add(this.lblIpAddress);
			this.groupBoxControls.Controls.Add(this.tbConnTimeout);
			this.groupBoxControls.Location = new System.Drawing.Point(7, 3);
			this.groupBoxControls.Name = "groupBoxControls";
			this.groupBoxControls.Size = new System.Drawing.Size(460, 72);
			this.groupBoxControls.TabIndex = 11;
			this.groupBoxControls.TabStop = false;
			// 
			// DevLinkMonitor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(717, 350);
			this.Controls.Add(this.groupBoxControls);
			this.Controls.Add(this.buttonClear);
			this.Controls.Add(this.textBoxLog);
			this.Controls.Add(this.buttonStartStop);
			this.Name = "DevLinkMonitor";
			this.Text = "DevLinkMonitor";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.groupBoxControls.ResumeLayout(false);
			this.groupBoxControls.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonStartStop;
		private System.Windows.Forms.TextBox textBoxLog;
		private System.Windows.Forms.Button buttonClear;
		private System.Windows.Forms.TextBox tbIpAddress;
		private System.Windows.Forms.TextBox tbPassword;
		private System.Windows.Forms.Label lblIpAddress;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.Label lblConnTimeout;
		private System.Windows.Forms.TextBox tbConnTimeout;
		private System.Windows.Forms.GroupBox groupBoxControls;
	}
}

