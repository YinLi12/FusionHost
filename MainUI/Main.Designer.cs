namespace MainUI
{
    partial class Main
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
            this.CheckedListBox_availableComPorts = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Btn_Connect = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Btn_Disconnect = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.BtnTest = new System.Windows.Forms.Button();
            this.textBoxTestCommand = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // CheckedListBox_availableComPorts
            // 
            this.CheckedListBox_availableComPorts.FormattingEnabled = true;
            this.CheckedListBox_availableComPorts.Location = new System.Drawing.Point(12, 24);
            this.CheckedListBox_availableComPorts.Name = "CheckedListBox_availableComPorts";
            this.CheckedListBox_availableComPorts.Size = new System.Drawing.Size(119, 79);
            this.CheckedListBox_availableComPorts.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Available Com Ports:";
            // 
            // Btn_Connect
            // 
            this.Btn_Connect.Location = new System.Drawing.Point(137, 24);
            this.Btn_Connect.Name = "Btn_Connect";
            this.Btn_Connect.Size = new System.Drawing.Size(75, 23);
            this.Btn_Connect.TabIndex = 2;
            this.Btn_Connect.Text = "Connect";
            this.Btn_Connect.UseVisualStyleBackColor = true;
            this.Btn_Connect.Click += new System.EventHandler(this.Btn_Connect_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 264);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(785, 124);
            this.textBox1.TabIndex = 3;
            // 
            // Btn_Disconnect
            // 
            this.Btn_Disconnect.Location = new System.Drawing.Point(218, 24);
            this.Btn_Disconnect.Name = "Btn_Disconnect";
            this.Btn_Disconnect.Size = new System.Drawing.Size(75, 23);
            this.Btn_Disconnect.TabIndex = 2;
            this.Btn_Disconnect.Text = "Disconnect";
            this.Btn_Disconnect.UseVisualStyleBackColor = true;
            this.Btn_Disconnect.Click += new System.EventHandler(this.Btn_Disconnect_Click);
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(12, 109);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(785, 125);
            this.panel2.TabIndex = 4;
            // 
            // BtnTest
            // 
            this.BtnTest.Location = new System.Drawing.Point(138, 54);
            this.BtnTest.Name = "BtnTest";
            this.BtnTest.Size = new System.Drawing.Size(155, 23);
            this.BtnTest.TabIndex = 5;
            this.BtnTest.Text = "Test";
            this.BtnTest.UseVisualStyleBackColor = true;
            this.BtnTest.Click += new System.EventHandler(this.BtnTest_Click);
            // 
            // textBoxTestCommand
            // 
            this.textBoxTestCommand.Location = new System.Drawing.Point(138, 84);
            this.textBoxTestCommand.Name = "textBoxTestCommand";
            this.textBoxTestCommand.Size = new System.Drawing.Size(659, 20);
            this.textBoxTestCommand.TabIndex = 6;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 396);
            this.Controls.Add(this.textBoxTestCommand);
            this.Controls.Add(this.BtnTest);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.Btn_Disconnect);
            this.Controls.Add(this.Btn_Connect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CheckedListBox_availableComPorts);
            this.Name = "Main";
            this.Text = "Wayne Host";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox CheckedListBox_availableComPorts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Btn_Connect;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button Btn_Disconnect;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button BtnTest;
        private System.Windows.Forms.TextBox textBoxTestCommand;
    }
}

