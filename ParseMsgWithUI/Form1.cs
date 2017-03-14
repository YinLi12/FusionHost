using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessageParser;

namespace ParseMsgWithUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] rawBytes = this.textBox1.Text.Replace(" ", "").ToBytes();
                Parser p = new Parser();
                var msg = p.Deserialize(rawBytes);
                this.textBox2.Text = msg.ToLogString();
            }
            catch (Exception ex)
            {
                this.textBox2.Text = "Parse Error: " + ex;
            }
        }
    }
}
