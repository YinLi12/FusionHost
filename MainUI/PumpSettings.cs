using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using MessageParser;

namespace MainUI
{
    public partial class PumpSettings : Form
    {
        private LogicalPump pump;
        private PictureBox pumpIcon;
        private string priceFileName;
        private string pumpConfigFileName;
        public PumpSettings(LogicalPump pump)
        {
            InitializeComponent();
            this.pump = pump;
            this.pumpIcon = pump.PumpIcon;
            this.priceFileName = @"Xml\" + this.pump.ComPortName + "_PriceConfig.xml";
            this.pumpConfigFileName = @"Xml\" + this.pump.ComPortName + "_PumpConfig.xml";
            this.Text = "连接在 " + this.pump.ComPortName + " 上的加油机";
            this.pump.PropertyChanged += (a, b) => this.Invoke(new Action(this.UpdateUI));
            UpdateUI();
        }

        private void UpdateUI()
        {
            this.label_Description.Text = "当前状态：" + this.pump.PumpState;
            this.label_Description.Text += "，此台为" + this.pump.Nozzles_油枪组.Count() + "枪机";
            this.panel1.Controls.Clear();
            int iconHorizentalOffset = 0;
            if (this.pump.Nozzles_油枪组 != null && this.pump.Nozzles_油枪组.Any())
            {
                foreach (var nozzle in this.pump.Nozzles_油枪组)
                {
                    var nozzleIcon = new PictureBox();
                    nozzleIcon.Location = new Point(iconHorizentalOffset, 0);
                    nozzleIcon.SizeMode = PictureBoxSizeMode.AutoSize;
                    nozzleIcon.Paint += (c, d) =>
                    {
                        using (Font myFont = new Font("Arial", 9))
                        {
                            d.Graphics.DrawString("枪" + nozzle.NozzleNumber + ",累升:" + System.Environment.NewLine + nozzle.VolumnAccumulator
                                , myFont, Brushes.Black, new Point(0, 0));
                        }
                    };
                    if (nozzle.NozzleState == LogicalNozzle.PumpNozzleState.Idle)
                        nozzleIcon.ImageLocation = @"Images/pump_liq_estado_idle.gif";
                    else if (nozzle.NozzleState == LogicalNozzle.PumpNozzleState.BusyLiftedOrFueling)
                        nozzleIcon.ImageLocation = @"Images/pump_liq_estado_fuelling.gif";
                    else if (nozzle.NozzleState == LogicalNozzle.PumpNozzleState.BusyCardInserted)
                        nozzleIcon.ImageLocation = @"Images/pump_liq_estado_fuelling.gif";
                    this.panel1.Controls.Add(nozzleIcon);
                    iconHorizentalOffset += 70;
                }
            }

            if (this.pump.Transactions.Any())
            {
                foreach (var trx in this.pump.Transactions)
                {
                    this.textBox_Transaction.AppendText("成交于：" + trx.TIME.ToString("u") + ", " + trx.AMN数额 + "元，" + trx.VOL_升数
                                                        + "升，于枪" + trx.NZN_枪号 + "上，卡号" + trx.ASN卡应用号
                                                        + "，油品代码：" + trx.G_CODE_油品代码
                                                        + System.Environment.NewLine + "============" +
                                                        System.Environment.NewLine);
                }
            }
            else
            {
                this.textBox_Transaction.Text = "";
            }

            LoadAndShowFiles();
        }

        private void LoadAndShowFiles()
        {
            // user never did the config, so create one based on default values.
            if (!File.Exists(priceFileName))
            {
                MessageBox.Show("未发现人工配置价格文件，将使用默认值生成，请人工修改后保存！");
                var mySerializer = new XmlSerializer(typeof(FuelPriceList));
                // To write to a file, create a StreamWriter object.  
                using (var myWriter = new StreamWriter(priceFileName))
                {
                    mySerializer.Serialize(myWriter, LogicalPump.GetDefaultFuelPriceList());
                    myWriter.Close();
                }
            }

            this.textBoxPriceConfig.Text = File.ReadAllText(priceFileName, Encoding.UTF8);


            // user never did the config, so create one based on default values.
            if (!File.Exists(pumpConfigFileName))
            {
                MessageBox.Show("未发现人工配置过的油机配置文件，将使用默认值生成，请人工修改后保存！");
                var mySerializer = new XmlSerializer(typeof(PumpStationInfo));
                // To write to a file, create a StreamWriter object.  
                using (var myWriter = new StreamWriter(pumpConfigFileName))
                {
                    mySerializer.Serialize(myWriter, LogicalPump.GetDefaultPumpStationInfo());
                    myWriter.Close();
                }
            }

            this.textBoxPumpConfig.Text = File.ReadAllText(pumpConfigFileName, Encoding.UTF8);
        }

        private void btnSavePriceConfig_Click(object sender, EventArgs e)
        {
            XmlSerializer mySerializer = new
                XmlSerializer(typeof(FuelPriceList));
            using (TextReader reader = new StringReader(this.textBoxPriceConfig.Text))
            {
                var result = (FuelPriceList)mySerializer.Deserialize(reader);
                this.pump.FulePriceList = result;
                this.pump.FulePriceList.VER_版本 += 1;
            }


            File.Delete(this.priceFileName);
            var fs = File.OpenWrite(this.priceFileName);
            var bytes = Encoding.UTF8.GetBytes(this.textBoxPriceConfig.Text);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
        }

        private void btnSavePumpConfig_Click(object sender, EventArgs e)
        {
            XmlSerializer mySerializer = new
                XmlSerializer(typeof(PumpStationInfo));
            using (TextReader reader = new StringReader(this.textBoxPumpConfig.Text))
            {
                var result = (PumpStationInfo)mySerializer.Deserialize(reader);
                this.pump.PumpStationInfo = result;
                this.pump.PumpStationInfo.Ver += 1;
            }

            File.Delete(this.pumpConfigFileName);
            var fs = File.OpenWrite(this.pumpConfigFileName);
            var bytes = Encoding.UTF8.GetBytes(this.textBoxPumpConfig.Text);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
        }

        private void btn_EnablePumpComm_Click(object sender, EventArgs e)
        {
            this.pump.AllowCommWithRealPhsicalPump = !this.pump.AllowCommWithRealPhsicalPump;
            this.btn_EnablePumpComm.Text = this.pump.AllowCommWithRealPhsicalPump ? "关闭通信" : "开启通信";
            pumpIcon.ImageLocation = this.pump.AllowCommWithRealPhsicalPump ? @"Images/pump_group_newdesign.png" : @"Images/pump_group_disabled.png";
        }
    }
}
