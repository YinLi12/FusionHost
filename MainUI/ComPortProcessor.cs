using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using log4net;
using MessageParser;
using MessageParser.MessageEntity.Outgoing;

namespace MainUI
{
    public class ComPortProcessor : IDisposable
    {
        public enum CommActiveMode
        {
            PcActive,
            PumpActive,
        }
        static ILog logger = log4net.LogManager.GetLogger("Main");

        private readonly SerialPort port;
        private List<byte> buffer;
        private int expectingMsgLen = 0;

        /// <summary>
        /// fired right before bytes writ to a COM PORT, can interrupt the operation by EventArg.
        /// </summary>
        public event EventHandler<ComPortEventArg> OnComPortDataWriting;

        /// <summary>
        /// msg just received and deserialized from COM port raw bytes, and no further processing yet.
        /// </summary>
        public event EventHandler<ComPortEventArg> OnMessageReceiving;

        /// <summary>
        /// msg had been processed, and outgoing msg (if necessary) had been send out.
        /// </summary>
        public event EventHandler<ComPortEventArg> OnMessageProcessed;

        /// <summary>
        /// Only created when concreate KaJiLianDong Msg received via COM port, otherwise, this is null.
        /// </summary>
        public LogicalPump Pump { get; set; }
        public CommActiveMode ActiveMode { get; private set; }


        public SerialPort SerialPort => this.port;

        private ComPortProcessor() { }

        public ComPortProcessor(SerialPort port, CommActiveMode activeMode)
        {
            this.port = port;
            this.ActiveMode = activeMode;
        }

        private System.Timers.Timer timelyCheck;
        public void Start()
        {
            port.DataReceived += Port_DataReceived;
            this.buffer = new List<byte>();
            port.Open();
            logger.Debug(port.PortName + " is Opened? : " + port.IsOpen);
            if (this.ActiveMode == CommActiveMode.PcActive)
            {
                timelyCheck = new Timer(10000);
                timelyCheck.Elapsed += (_, __) =>
                {
                    // first time to attempt connect to a pump which may not existed in other peer
                    if (this.Pump == null)
                    {
                        // here all send with 0 for avoid downloading the incorrect data to an unknown configuration pump. does it work?
                        var outgoing = new PcGenericInquiryWithRichInfoResponse();
                        outgoing.SetMessageSequenceNumber(0);
                        outgoing.SetPcTime(DateTime.Now);
                        outgoing.BL_VER = 0; //LogicalPump.GetFundamentalBlackList().Version;
                        outgoing.ADD_BL_VER = 0; //(byte)LogicalPump.GetIncrementalBlackList().Version;
                        outgoing.DEL_BL_VER = 0; //(byte)LogicalPump.GetDeletionBlackList().Version;
                        outgoing.WH_VER = 0; //(byte)LogicalPump.GetWhiteList().Version;
                        outgoing.PRC_VER = 0; //LogicalPump.GetDefaultFuelPriceList().VER_版本;
                        outgoing.Sta_VER = 0; //LogicalPump.GetDefaultPumpStationInfo().Ver;
                        outgoing.SELF_D_VER = PcGenericInquiryWithRichInfoResponse.更新标志.无数据;
                        outgoing.SOFT_FLAG = PcGenericInquiryWithRichInfoResponse.更新标志.无数据;
                    }
                    else
                    {
                        // wait for manual confirm the download files content.
                        if (!this.Pump.AllowCommWithRealPhsicalPump) return;
                        var outgoing = new PcGenericInquiryWithRichInfoResponse();
                        outgoing.SetMessageSequenceNumber(Pump.LastSendMsgSequenceNumber++);
                        outgoing.SetPcTime(DateTime.Now);
                        outgoing.BL_VER = LogicalPump.GetFundamentalBlackList().Version;
                        outgoing.ADD_BL_VER = (byte)LogicalPump.GetIncrementalBlackList().Version;
                        outgoing.DEL_BL_VER = (byte)LogicalPump.GetDeletionBlackList().Version;
                        outgoing.WH_VER = (byte)LogicalPump.GetWhiteList().Version;
                        outgoing.PRC_VER = this.Pump.FulePriceList.VER_版本;
                        outgoing.Sta_VER = this.Pump.PumpStationInfo.Ver;
                        outgoing.SELF_D_VER = PcGenericInquiryWithRichInfoResponse.更新标志.无数据;
                        outgoing.SOFT_FLAG = PcGenericInquiryWithRichInfoResponse.更新标志.无数据;
                    }
                };
                timelyCheck.Start();
            }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var port = sender as SerialPort;
                while (port.BytesToRead > 0)
                {
                    var oneByte = (byte)port.ReadByte();
                    //logger.Debug(port.PortName + " incoming a byte raw(Hex): " + oneByte.ToString("X").PadLeft(2, '0'));

                    if (!buffer.Any() && oneByte != 0xFA)
                        continue;
                    //throw new FormatException("Data header must be 0xFA, while now is 0x" + oneByte.ToString("X").PadLeft(2, '0'));

                    this.buffer.Add(oneByte);
                    if (this.expectingMsgLen == 0 && this.buffer.Count >= 8)
                    {
                        this.expectingMsgLen =
                            ((new List<byte>() { this.buffer[4], this.buffer[5] }).ToArray()).GetBCD() + 4 + 2 + 2;
                        //logger.Debug(port.PortName + " expectingMsgLen: " + expectingMsgLen);
                    }

                    if (this.buffer.Count == this.expectingMsgLen)
                    {
                        byte[] copy = new byte[this.expectingMsgLen];
                        this.buffer.CopyTo(copy);
                        this.buffer.Clear();
                        this.expectingMsgLen = 0;

                        logger.Debug(port.PortName + " incoming a msg raw(Hex): " + copy.Select(b => b.ToString("X").PadLeft(2, '0')).Aggregate((acc, n) => acc + " " + n));

                        var parser = new Parser();
                        var incomingMsg = parser.Deserialize(copy);

                        this.ProcessMessage(incomingMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception in Port_DataReceived(..., ...), buffer: "
                    + this.buffer.Select(b => b.ToString("X").PadLeft(2, '0')).Aggregate((acc, n) => acc + " " + n) + ", expectingLen: " + this.expectingMsgLen
                    + System.Environment.NewLine + ex);
                this.buffer.Clear();
            }
        }

        /// <summary>
        /// when everything initializing, read the pump files version first rather than send local file versions number which may trigger the downloading process.
        /// </summary>
        private bool tryReadPumpGenericInfoAtBeginning = false;

        public void ProcessMessage(MessageTemplateBase incomingMsg)
        {
            var safe = this.OnMessageReceiving;
            var arg = new ComPortEventArg() { Continue = true, Message = incomingMsg };
            safe?.Invoke(this, arg);

            var parser = new Parser();
            // skip the logging for PumpGenericInquiryRequest msg since it's too much.
            if (!(incomingMsg is PumpGenericInquiryRequest))
            {
                logger.Debug(port.PortName + "      " + incomingMsg.ToLogString());
            }

            // Null LogicalPump object indicates comm is not ready yet.
            if (this.Pump == null)
            {
                return;
            }

            this.Pump.Pulse();
            if (!this.Pump.AllowCommWithRealPhsicalPump) return;
            if (incomingMsg is PumpGenericInquiryRequest)
            {
                if (this.tryReadPumpGenericInfoAtBeginning)
                {
                    /* here we send the same versions for all downloadable content for simplify */
                    var outgoing = new PcGenericInquiryWithRichInfoResponse();
                    outgoing.SetPcTime(DateTime.Now);
                    outgoing.BL_VER = this.Pump.BL_VER;
                    outgoing.ADD_BL_VER = this.Pump.ADD_BL_VER;
                    outgoing.DEL_BL_VER = this.Pump.DEL_BL_VER;
                    outgoing.WH_VER = this.Pump.WH_VER;
                    outgoing.PRC_VER = this.Pump.PRC_VER;
                    outgoing.Sta_VER = this.Pump.Sta_VER;
                    outgoing.SELF_D_VER = PcGenericInquiryWithRichInfoResponse.更新标志.无数据;
                    outgoing.SOFT_FLAG = PcGenericInquiryWithRichInfoResponse.更新标志.无数据;
                    outgoing.SetMessageSequenceNumber(Pump.LastSendMsgSequenceNumber++);
                    this.Write(outgoing);
                }
                else
                {
                    /* here we send the confirmed (by cashier, since `AllowCommWithRealPhsicalPump` is true now) versions for all downloadable content*/
                    var outgoing = new PcGenericInquiryWithRichInfoResponse();
                    outgoing.SetMessageSequenceNumber(Pump.LastSendMsgSequenceNumber++);
                    outgoing.SetPcTime(DateTime.Now);
                    outgoing.BL_VER = LogicalPump.GetFundamentalBlackList().Version;
                    outgoing.ADD_BL_VER = (byte)LogicalPump.GetIncrementalBlackList().Version;
                    outgoing.DEL_BL_VER = (byte)LogicalPump.GetDeletionBlackList().Version;
                    outgoing.WH_VER = (byte)LogicalPump.GetWhiteList().Version;
                    // below use the latest one.
                    outgoing.PRC_VER = this.Pump.FulePriceList.VER_版本;
                    outgoing.Sta_VER = this.Pump.PumpStationInfo.Ver;
                    outgoing.SELF_D_VER = PcGenericInquiryWithRichInfoResponse.更新标志.无数据;
                    outgoing.SOFT_FLAG = PcGenericInquiryWithRichInfoResponse.更新标志.无数据;
                    this.Write(outgoing);
                }
            }
            else if (incomingMsg is PcReadPumpGenericInfoResponse)
            {
                var incoming = incomingMsg as PcReadPumpGenericInfoResponse;
                this.Pump.ReloadFrom(incoming);
            }
            else if (incomingMsg is PumpNotifyTransactionDoneRequest)
            {
                var incoming = incomingMsg as PumpNotifyTransactionDoneRequest;

                this.Pump.StackTransaction(LogicalTransaction.LoadFrom(incoming));
                if (this.Pump.Transactions.Count() > this.Pump.KeepMaxTransactionNumber)
                    this.Pump.ClearTransaction(0);
                var outgoing = new PumpNotifyTransactionDoneResponse();
                outgoing.SetMessageSequenceNumber(Pump.LastSendMsgSequenceNumber++);
                outgoing.Result = PumpNotifyTransactionDoneResponse.PumpNotifyTransactionDoneResponseResult.正确;
                this.Write(outgoing);

                if (this.Pump.ReadPumpAccumualtorAfterTrxDone)
                {
                    var request = new PcAskReadPumpAccumulator();
                    request.SetMessageSequenceNumber(Pump.LastSendMsgSequenceNumber++);
                    this.Write(request);
                }
            }
            else if (incomingMsg is PumpAskDataDownloadRequest)
            {
                var incoming = incomingMsg as PumpAskDataDownloadRequest;
                var outgoing = new PumpAskDataDownloadResponse();
                outgoing.SetMessageSequenceNumber(Pump.LastSendMsgSequenceNumber++);
                int downloadingContentLength = 0;
                switch (incoming.Content_内容)
                {
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.基础黑名单:
                        downloadingContentLength = parser.Serialize(LogicalPump.GetWhiteList()).Length;
                        break;
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.新增黑名单:
                        break;
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.新删黑名单:
                        break;
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.白名单:
                        downloadingContentLength = parser.Serialize(LogicalPump.GetWhiteList()).Length;
                        break;
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.油品油价表:
                        downloadingContentLength = parser.Serialize(LogicalPump.GetDefaultFuelPriceList()).Length;
                        break;
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.油站通用信息:
                        downloadingContentLength = parser.Serialize(LogicalPump.GetDefaultPumpStationInfo()).Length;
                        break;
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.私有数据:
                        break;
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.下载程序:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // tell pump this length of content will start downloading
                outgoing.BL_LEN_长度 = downloadingContentLength;
                outgoing.Content_内容 = incoming.Content_内容;
                this.Write(outgoing);
            }
            else if (incomingMsg is PumpDataDownloadRequest)
            {
                var incoming = incomingMsg as PumpDataDownloadRequest;
                var outgoing = new PumpDataDownloadResponse();
                outgoing.SetMessageSequenceNumber(Pump.LastSendMsgSequenceNumber++);
                byte[] fullDownloadingBytes = null;
                switch (incoming.Content_内容)
                {
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.基础黑名单:
                        fullDownloadingBytes = parser.Serialize(LogicalPump.GetWhiteList());
                        break;
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.新增黑名单:
                        fullDownloadingBytes = parser.Serialize(LogicalPump.GetIncrementalBlackList());
                        break;
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.新删黑名单:
                        fullDownloadingBytes = parser.Serialize(LogicalPump.GetDeletionBlackList());
                        break;
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.白名单:
                        fullDownloadingBytes = parser.Serialize(LogicalPump.GetWhiteList());
                        break;
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.油品油价表:
                        fullDownloadingBytes = parser.Serialize(LogicalPump.GetDefaultFuelPriceList());
                        break;
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.油站通用信息:
                        fullDownloadingBytes = parser.Serialize(LogicalPump.GetDefaultPumpStationInfo());
                        break;
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.私有数据:
                        break;
                    case PumpAskDataDownloadRequest.PumpAskDataDownloadType.下载程序:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                outgoing.Content_内容 = incoming.Content_内容;
                outgoing.S_OFFSET_段偏移 = incoming.S_OFFSET_段偏移;
                outgoing.SEG_段数_segs = incoming.SEG_段数_segs;
                outgoing.DATA_Content_数据内容 =
                    fullDownloadingBytes.Skip(16 * incoming.S_OFFSET_段偏移).Take(incoming.SEG_段数_segs * 16).ToList();
                this.Write(outgoing);
            }
            else if (incomingMsg is PumpStateChangeRequest)
            {
                //var previousState = Pump.PumpState;
                var incoming = incomingMsg as PumpStateChangeRequest;
                Pump.UpdatePumpStateByMessage(incoming);
            }
            else if (incomingMsg is PcAskReadPumpAccumulatorResponse)
            {
                var incoming = incomingMsg as PcAskReadPumpAccumulatorResponse;
                foreach (var incomingNozzleInfo in incoming.MZN_单个油枪)
                {
                    if (Pump.Nozzles_油枪组.Any(p => p.NozzleNumber == incomingNozzleInfo.NZN_枪号))
                    {
                        Pump.Nozzles_油枪组.First(p => p.NozzleNumber == incomingNozzleInfo.NZN_枪号).VolumnAccumulator =
                            incomingNozzleInfo.V_TOT_升累计;
                    }
                    else
                    {
                        Pump.AddNozzle(new LogicalNozzle() { NozzleNumber = incomingNozzleInfo.NZN_枪号, VolumnAccumulator = incomingNozzleInfo.V_TOT_升累计 });
                    }
                }
            }
            else if (incomingMsg is PumpInquiryBlackAndWhiteListRequest)
            {
                var incoming = incomingMsg as PumpInquiryBlackAndWhiteListRequest;
                var outgoing = new PumpInquiryBlackAndWhiteListResponse();
                outgoing.SetMessageSequenceNumber(Pump.LastSendMsgSequenceNumber++);
                outgoing.ASN卡应用号 = incoming.ASN卡应用号;
                outgoing.M_FLAG_匹配标志 = PumpInquiryBlackAndWhiteListResponse.PumpInquiryBlackAndWhiteListResult.匹配;
                this.Write(outgoing);
            }

            var safe1 = this.OnMessageProcessed;
            var arg1 = new ComPortEventArg() { Continue = true, Message = incomingMsg };
            safe1?.Invoke(this, arg1);
        }

        public virtual void Write(MessageTemplateBase outgoing)
        {
            var safe = this.OnComPortDataWriting;
            var arg = new ComPortEventArg() { Message = outgoing, Continue = true };
            safe?.Invoke(this, arg);
            if (arg.Continue)
            {
                var parser = new Parser();
                var bytes = parser.Serialize(outgoing);
                logger.Debug(port.PortName + " outgoing a msg raw(Hex): " + bytes.Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n));
                this.port.Write(bytes, 0, bytes.Length);
            }
        }

        public void Dispose()
        {
            this.port.Close();
            timelyCheck.Stop();
            timelyCheck.Dispose();
            this.Pump = null;
        }
    }

    public class ComPortEventArg : EventArgs
    {
        public MessageTemplateBase Message { get; set; }
        public byte[] RawBytes { get; set; }

        /// <summary>
        /// give outside a chance to interrupt the operation, for now the unit test need this.
        /// </summary>
        public bool Continue { get; set; }
    }
}
