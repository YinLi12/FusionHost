namespace MainUI
{
    partial class PumpSettings
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
            this.label_Description = new System.Windows.Forms.Label();
            this.labe_Detail = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_EnablePumpComm = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnSavePriceConfig = new System.Windows.Forms.Button();
            this.btnSavePumpConfig = new System.Windows.Forms.Button();
            this.textBoxPumpConfig = new System.Windows.Forms.TextBox();
            this.textBoxPriceConfig = new System.Windows.Forms.TextBox();
            this.textBox_Transaction = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_Description
            // 
            this.label_Description.AutoSize = true;
            this.label_Description.Location = new System.Drawing.Point(13, 13);
            this.label_Description.Name = "label_Description";
            this.label_Description.Size = new System.Drawing.Size(92, 13);
            this.label_Description.TabIndex = 0;
            this.label_Description.Text = "overall description";
            // 
            // labe_Detail
            // 
            this.labe_Detail.AutoSize = true;
            this.labe_Detail.Location = new System.Drawing.Point(13, 36);
            this.labe_Detail.Name = "labe_Detail";
            this.labe_Detail.Size = new System.Drawing.Size(41, 13);
            this.labe_Detail.TabIndex = 0;
            this.labe_Detail.Text = "detail...";
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(16, 70);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(542, 93);
            this.panel1.TabIndex = 2;
            // 
            // btn_EnablePumpComm
            // 
            this.btn_EnablePumpComm.Location = new System.Drawing.Point(483, 8);
            this.btn_EnablePumpComm.Name = "btn_EnablePumpComm";
            this.btn_EnablePumpComm.Size = new System.Drawing.Size(75, 23);
            this.btn_EnablePumpComm.TabIndex = 4;
            this.btn_EnablePumpComm.Text = "开启通信";
            this.btn_EnablePumpComm.UseVisualStyleBackColor = true;
            this.btn_EnablePumpComm.Click += new System.EventHandler(this.btn_EnablePumpComm_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 266);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(542, 266);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBoxPriceConfig);
            this.tabPage1.Controls.Add(this.btnSavePriceConfig);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(534, 240);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "油价配置";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnSavePumpConfig);
            this.tabPage2.Controls.Add(this.textBoxPumpConfig);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(534, 240);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "油机配置";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnSavePriceConfig
            // 
            this.btnSavePriceConfig.Location = new System.Drawing.Point(453, 211);
            this.btnSavePriceConfig.Name = "btnSavePriceConfig";
            this.btnSavePriceConfig.Size = new System.Drawing.Size(75, 23);
            this.btnSavePriceConfig.TabIndex = 4;
            this.btnSavePriceConfig.Text = "保存油价";
            this.btnSavePriceConfig.UseVisualStyleBackColor = true;
            this.btnSavePriceConfig.Click += new System.EventHandler(this.btnSavePriceConfig_Click);
            // 
            // btnSavePumpConfig
            // 
            this.btnSavePumpConfig.Location = new System.Drawing.Point(455, 211);
            this.btnSavePumpConfig.Name = "btnSavePumpConfig";
            this.btnSavePumpConfig.Size = new System.Drawing.Size(75, 23);
            this.btnSavePumpConfig.TabIndex = 6;
            this.btnSavePumpConfig.Text = "保存配置";
            this.btnSavePumpConfig.UseVisualStyleBackColor = true;
            this.btnSavePumpConfig.Click += new System.EventHandler(this.btnSavePumpConfig_Click);
            // 
            // textBoxPumpConfig
            // 
            this.textBoxPumpConfig.Location = new System.Drawing.Point(5, 6);
            this.textBoxPumpConfig.Multiline = true;
            this.textBoxPumpConfig.Name = "textBoxPumpConfig";
            this.textBoxPumpConfig.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxPumpConfig.Size = new System.Drawing.Size(525, 199);
            this.textBoxPumpConfig.TabIndex = 5;
            // 
            // textBoxPriceConfig
            // 
            this.textBoxPriceConfig.Location = new System.Drawing.Point(4, 7);
            this.textBoxPriceConfig.Multiline = true;
            this.textBoxPriceConfig.Name = "textBoxPriceConfig";
            this.textBoxPriceConfig.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxPriceConfig.Size = new System.Drawing.Size(517, 198);
            this.textBoxPriceConfig.TabIndex = 5;
            // 
            // textBox_Transaction
            // 
            this.textBox_Transaction.Location = new System.Drawing.Point(16, 170);
            this.textBox_Transaction.Multiline = true;
            this.textBox_Transaction.Name = "textBox_Transaction";
            this.textBox_Transaction.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Transaction.Size = new System.Drawing.Size(542, 90);
            this.textBox_Transaction.TabIndex = 6;
            // 
            // PumpSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 535);
            this.Controls.Add(this.textBox_Transaction);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btn_EnablePumpComm);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labe_Detail);
            this.Controls.Add(this.label_Description);
            this.Name = "PumpSettings";
            this.Text = "PumpSettings";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_Description;
        private System.Windows.Forms.Label labe_Detail;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_EnablePumpComm;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox textBoxPriceConfig;
        private System.Windows.Forms.Button btnSavePriceConfig;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnSavePumpConfig;
        private System.Windows.Forms.TextBox textBoxPumpConfig;
        private System.Windows.Forms.TextBox textBox_Transaction;
    }
}