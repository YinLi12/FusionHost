using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using MessageParser;
using Timer = System.Timers.Timer;

namespace MainUI
{
    public partial class Main : Form
    {
        static ILog logger = log4net.LogManager.GetLogger("Main");

        ComPortProcessor.CommActiveMode activeMode
            = ConfigurationManager.AppSettings["PcActiveMode"].ToLower() == "true" ? ComPortProcessor.CommActiveMode.PcActive : ComPortProcessor.CommActiveMode.PumpActive;

        public Main()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();
            this.Btn_Disconnect.Enabled = false;
            this.Shown += Main_Shown;
            this.Closing += (a, b) =>
             {
                 this.Btn_Disconnect_Click(null, null);
             };
        }


        private void Main_Shown(object sender, EventArgs e)
        {
            string[] portlist = System.IO.Ports.SerialPort.GetPortNames();
            this.CheckedListBox_availableComPorts.Items.AddRange(portlist);
            //this.CheckedListBox_availableComPorts.Items.Add("fake0");
            //this.CheckedListBox_availableComPorts.Items.Add("fake1");
            //this.CheckedListBox_availableComPorts.Items.Add("fake2");
            //this.CheckedListBox_availableComPorts.Items.Add("fake3");
            //this.CheckedListBox_availableComPorts.Items.Add("fake4");
            //this.CheckedListBox_availableComPorts.Items.Add("fake5");
            //this.CheckedListBox_availableComPorts.Items.Add("fake6");
            //this.CheckedListBox_availableComPorts.Items.Add("fake7");
            //this.CheckedListBox_availableComPorts.Items.Add("fake8");
            this.CheckedListBox_availableComPorts.CheckOnClick = true;
            logger.Info("Available Com Ports: " + portlist.Aggregate((acc, n) => acc + ", " + n));
        }

        List<ComPortProcessor> processors = new List<ComPortProcessor>();
        private void Btn_Connect_Click(object sender, EventArgs e)
        {
            var checkedComPorts = this.CheckedListBox_availableComPorts.CheckedItems.Cast<string>();
            if (!checkedComPorts.Any()) return;
            this.Btn_Connect.Enabled = false;
            this.Btn_Disconnect.Enabled = true;
            logger.Info("Connecting Com Ports: " + checkedComPorts.Aggregate((acc, n) => acc + ", " + n));

            int baud = 9600;
            Parity parity = Parity.None;
            int databit = 8;
            StopBits stopbit = StopBits.One;
            int iconHorizentalOffset = 0;
            foreach (var comPortName in checkedComPorts)
            {
                var processor = new ComPortProcessor(new SerialPort(comPortName, baud, parity, databit, stopbit), this.activeMode);
                this.processors.Add(processor);
                processor.OnMessageReceiving += (_, arg) =>
                {
                    var p = _ as ComPortProcessor;
                    this.LoggingOnUiForIncoming(p, arg.Message);
                    // OnMessageReceiving event fired indicates the COM Port have a real pump connected at other peer, 
                    // so create the LogicalPump object, and draw the ICON.
                    if (p.Pump == null)
                    {
                        var pumpIcon = new PictureBox { Location = new Point(iconHorizentalOffset, 0) };
                        pumpIcon.Paint += (c, d) =>
                        {
                            using (var myFont = new Font("Arial", 12))
                            {
                                d.Graphics.DrawString(p.SerialPort.PortName, myFont, Brushes.Black, new Point(0, 0));
                            }
                        };
                        pumpIcon.BackColor = Color.Transparent;
                        pumpIcon.SizeMode = PictureBoxSizeMode.AutoSize;
                        pumpIcon.ImageLocation = @"Images/pump_group_disabled.png";//@"Images/pump_group_newdesign.png";
                        pumpIcon.Click += (__, ___) => { new PumpSettings(p.Pump).Show(); };
                        this.panel2.Controls.Add(pumpIcon);
                        iconHorizentalOffset += 70;

                        p.Pump = new LogicalPump
                        {
                            ComPortName = p.SerialPort.PortName,
                            PumpIcon = pumpIcon
                        };
                        p.Pump.PropertyChanged += (a, b) =>
                        {
                            if (b.PropertyName == "PumpState")
                            {
                                this.textBox1.AppendText("Pump state changed to " + p.Pump.PumpState +
                                                         System.Environment.NewLine);
                                if (p.Pump.PumpState == LogicalPump.LogicalPumpState.CardInserted)
                                {
                                    p.Pump.PumpIcon.ImageLocation = @"Images/pump_group_card_insert.png";
                                }
                                else if (p.Pump.PumpState == LogicalPump.LogicalPumpState.Idle)
                                {
                                    p.Pump.PumpIcon.ImageLocation = @"Images/pump_group_newdesign.png";
                                }
                            }
                        };

                        System.Timers.Timer timelyCheck = new Timer(5000);
                        timelyCheck.Elapsed += (___, __) =>
                        {
                            var timelyQueryAcc = new PcAskReadPumpAccumulator();
                            timelyQueryAcc.SetMessageSequenceNumber(p.Pump.LastSendMsgSequenceNumber++);
                            p.Write(timelyQueryAcc);
                        };
                        timelyCheck.Start();
                    }
                };
                processor.Start();
                this.textBox1.AppendText("Connected to ComPort: " + comPortName + System.Environment.NewLine);
                logger.Info("Connected to ComPort: " + comPortName);
            }
        }

        public void LoggingOnUiForIncoming(ComPortProcessor p, MessageTemplateBase incomingMsg)
        {
            this.Invoke(new Action(() =>
            {
                if (incomingMsg is PumpNotifyTransactionDoneRequest)
                {
                    var incoming = incomingMsg as PumpNotifyTransactionDoneRequest;
                    this.textBox1.AppendText("A Trx is done, vol: " + incoming.VOL_升数 + ", amnt:" + incoming.AMN数额 +
                                          System.Environment.NewLine);
                }
                else if (incomingMsg is PumpAskDataDownloadRequest)
                {
                    var incoming = incomingMsg as PumpAskDataDownloadRequest;
                    this.textBox1.AppendText("Pump asking for download: " + incoming.Content_内容 +
                                          System.Environment.NewLine);
                }
                else if (incomingMsg is PumpDataDownloadRequest)
                {
                    var incoming = incomingMsg as PumpDataDownloadRequest;
                    this.textBox1.AppendText("Pump start for downloading: " + incoming.Content_内容 +
                                          System.Environment.NewLine);
                }
                else if (incomingMsg is PcAskReadPumpAccumulatorResponse)
                {
                    var incoming = incomingMsg as PcAskReadPumpAccumulatorResponse;
                    foreach (var incomingNozzleInfo in incoming.MZN_单个油枪)
                    {
                        this.textBox1.AppendText("Pump returned the accumualtor data, nozzle: "
                            + incomingNozzleInfo.NZN_枪号 + ", acc(vol): "
                            + incomingNozzleInfo.V_TOT_升累计
                            + System.Environment.NewLine);
                    }
                }
                else if (incomingMsg is PumpInquiryBlackAndWhiteListRequest)
                {
                    var incoming = incomingMsg as PumpInquiryBlackAndWhiteListRequest;
                    this.textBox1.AppendText("Pump ask for BlackAndWhiteList check for card: " + incoming.ASN卡应用号 + System.Environment.NewLine);
                }
            }));
        }

        private void Btn_Disconnect_Click(object sender, EventArgs e)
        {
            processors.ForEach(p =>
            {
                p.Dispose();
                this.textBox1.AppendText("Disconnected to ComPort: " + p.SerialPort.PortName + System.Environment.NewLine);
            });
            this.Btn_Disconnect.Enabled = false;
            this.Btn_Connect.Enabled = true;
        }

        List<string> command = new List<string>()
        {
            //PumpStateChangeRequest31    信息条数        卡插入  NZN    LEN卡信息数据长度     ASN
            "FA 00 00 17 00 40 31020101   1001 00 04 19 90 08 00 0010 8400 0077 46 23 7401031001 00 04 19 90 08 00 00 10 8400 0077 46 23 74FF 77",
            //加油机上送累计数命令 油枪数   枪号 升累计         枪号 升累计      ......
            "FA 00 00 2B 00 22 38  04     01    00 B3 42 85     02     00 00 20 6B   03 00 00 93 DC 04 00 00 5C 2F D6 CA",
        };
        private void BtnTest_Click(object sender, EventArgs e)
        {
            Parser parser = new Parser();
            MessageTemplateBase sending = null;
            if (!string.IsNullOrEmpty(this.textBoxTestCommand.Text))
            {
                sending = parser.Deserialize(this.textBoxTestCommand.Text.ToBytes());
            }
            else
            {
                sending = new PumpGenericInquiryRequest();
            }

            foreach (var processor in this.processors)
            {
                processor.ProcessMessage(sending);
            }

        }
    }
}
