using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessageParser;
using MessageParser.MessageEntity.Outgoing.BlackAndWhiteList;
using Timer = System.Timers.Timer;

namespace MainUI
{
    public class LogicalPump : INotifyPropertyChanged
    {
        public PictureBox PumpIcon { get; set; }
        public LogicalPump()
        {
            System.Timers.Timer timelyCheck = new Timer(5000);
            timelyCheck.Elapsed += (_, __) =>
            {
                if (DateTime.Now.Subtract(this.lastPulseTime).TotalMinutes >=
                    maxPulseDueMinutes)
                {
                    this.PumpState = LogicalPumpState.Disconnected;
                }
            };
            timelyCheck.Start();
        }
        public void ReloadFrom(PcReadPumpGenericInfoResponse loadFrom)
        {
            M_INFO_厂家信息 = loadFrom.M_INFO_厂家信息;
            Prov_省代号 = loadFrom.Prov_省代号;
            City_地市代码 = loadFrom.City_地市代码;
            Superior_上级单位代号 = loadFrom.Superior_上级单位代号;
            S_ID_加油站ID = loadFrom.S_ID_加油站ID;
            TIME_Raw = loadFrom.TIME_Raw;
            //GUN_N_油枪数 = loadFrom.GUN_N_油枪数;

            BL_VER = loadFrom.BL_VER;
            ADD_BL_VER = loadFrom.ADD_BL_VER;
            DEL_BL_VER = loadFrom.DEL_BL_VER;
            WH_VER = loadFrom.WH_VER;
            PRC_VER = loadFrom.PRC_VER;
            Sta_VER = loadFrom.Sta_VER;
            M_DATA_厂家自定义数据 = loadFrom.M_DATA_厂家自定义数据;
            //LastLoadTime = DateTime.Now;
            loadFrom.MZN_单个油枪.ForEach(_ => this.AddNozzle(new LogicalNozzle() { NozzleNumber = _ }));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ComPortName { get; set; }

        private byte lastSendMsgSequenceNumber = 0;

        /// <summary>
        /// Gets or sets if this Logical Pump confirmed by human(cashier), and allow to send misc configuration msg to real pump.
        /// this is set to false most likely in first time start the system, and don't want send the Default configuration to a unknown config pump 
        /// which could not match, for this case, cashier should check/modify the default configuration, and then set this to allow.
        /// </summary>
        public bool AllowCommWithRealPhsicalPump { get; set; }

        public byte LastSendMsgSequenceNumber
        {
            get { return this.lastSendMsgSequenceNumber; }
            set
            {
                this.lastSendMsgSequenceNumber = value > 63 ? (byte)0 : value;
            }
        }

        /// <summary>
        /// Gets the indicator if send a pump accumulator inquiry after a trx was done.
        /// </summary>
        public bool ReadPumpAccumualtorAfterTrxDone { get; } = true;

        //public DateTime LastLoadTime { get; set; }

        private string _M_INFO_厂家信息;
        /// <summary>
        /// 前对齐，后补空格 
        /// </summary>
        public string M_INFO_厂家信息
        {
            get { return this._M_INFO_厂家信息; }
            set
            {
                this._M_INFO_厂家信息 = value;
                var safe = this.PropertyChanged;
                safe?.Invoke(this, new PropertyChangedEventArgs("M_INFO_厂家信息"));
            }
        }

        private byte _Prov_省代号;
        /// <summary>
        /// 石化编码规范中地区编码的头两个数字
        /// </summary>
        public byte Prov_省代号
        {
            get { return this._Prov_省代号; }
            set
            {
                this._Prov_省代号 = value;
                var safe = this.PropertyChanged;
                safe?.Invoke(this, new PropertyChangedEventArgs("Prov_省代号"));
            }
        }

        /// <summary>
        /// 石化编码规范中地区编码的中间两个数字
        /// </summary>
        public byte City_地市代码 { get; set; }

        /// <summary>
        /// 同石化编码
        /// </summary>
        public byte Superior_上级单位代号 { get; set; }

        /// <summary>
        /// 同石化编码
        /// </summary>
        public byte S_ID_加油站ID { get; set; }

        public long TIME_Raw { get; private set; }

        public DateTime GetPcTime()
        {
            var dd = this.TIME_Raw.ToString();
            var dt = Convert.ToDateTime(dd);
            return dt;
        }

        //public void SetPcTime(DateTime dateTime)
        //{
        //    var str = dateTime.ToString("yyyyMMddHHmmss");
        //    this.TIME_Raw = long.Parse(str);
        //}

        /// <summary>
        /// 
        /// </summary>
        //public byte GUN_N_油枪数 { get; set; }

        private List<LogicalNozzle> _Nozzles_油枪组 = new List<LogicalNozzle>();
        public IEnumerable<LogicalNozzle> Nozzles_油枪组 { get { return this._Nozzles_油枪组; } }

        public void AddNozzle(LogicalNozzle newNozzle)
        {
            this._Nozzles_油枪组.Add(newNozzle);
            var safe = this.PropertyChanged;
            safe?.Invoke(this, new PropertyChangedEventArgs("Nozzle"));
        }

        private int _BL_VER;
        /// <summary>
        /// 基础黑名单版本号 
        /// </summary>
        public int BL_VER
        {
            get { return this._BL_VER; }
            set
            {
                this._BL_VER = value;
                var safe = this.PropertyChanged;
                safe?.Invoke(this, new PropertyChangedEventArgs("BL_VER"));
            }
        }

        private byte _ADD_BL_VER;
        /// <summary>
        /// 新增黑名单版本 
        /// </summary>
        public byte ADD_BL_VER
        {
            get { return this._ADD_BL_VER; }
            set
            {
                this._ADD_BL_VER = value;
                var safe = this.PropertyChanged;
                safe?.Invoke(this, new PropertyChangedEventArgs("ADD_BL_VER"));
            }
        }

        private byte _DEL_BL_VER;
        /// <summary>
        /// 新删黑名单版本 
        /// </summary>
        public byte DEL_BL_VER
        {
            get { return this._DEL_BL_VER; }
            set
            {
                this._DEL_BL_VER = value;
                var safe = this.PropertyChanged;
                safe?.Invoke(this, new PropertyChangedEventArgs("DEL_BL_VER"));
            }
        }

        private byte _WH_VER;
        /// <summary>
        /// 白名单版本号 
        /// </summary>
        public byte WH_VER
        {
            get { return this._WH_VER; }
            set
            {
                this._WH_VER = value;
                var safe = this.PropertyChanged;
                safe?.Invoke(this, new PropertyChangedEventArgs("WH_VER"));
            }
        }

        private byte _PRC_VER;
        /// <summary>
        /// 油品油价版本 
        /// </summary>
        public byte PRC_VER
        {
            get { return this._PRC_VER; }
            set
            {
                this._PRC_VER = value;
                var safe = this.PropertyChanged;
                safe?.Invoke(this, new PropertyChangedEventArgs("PRC_VER"));
            }
        }

        private byte _Sta_VER;
        /// <summary>
        /// 油站通用信息版本 
        /// </summary>
        public byte Sta_VER
        {
            get { return this._Sta_VER; }
            set
            {
                this._Sta_VER = value;
                var safe = this.PropertyChanged;
                safe?.Invoke(this, new PropertyChangedEventArgs("Sta_VER"));
            }
        }

        public List<byte> M_DATA_厂家自定义数据 { get; set; }

        public int KeepMaxTransactionNumber => 100;

        private readonly List<LogicalTransaction> transactions = new List<LogicalTransaction>();
        public IEnumerable<LogicalTransaction> Transactions => this.transactions;

        public void StackTransaction(LogicalTransaction trx)
        {
            this.transactions.Add(trx);
            var safe = this.PropertyChanged;
            safe?.Invoke(this, new PropertyChangedEventArgs("Transaction"));
        }
        public void ClearTransaction(int index)
        {
            this.transactions.RemoveAt(index);
            var safe = this.PropertyChanged;
            safe?.Invoke(this, new PropertyChangedEventArgs("Transaction"));
        }


        public enum LogicalPumpState
        {
            /// <summary>
            /// no any nozzle is on lifting, or card inserted.
            /// </summary>
            Idle,
            /// <summary>
            /// at least one nozzle is in CardInserted.
            /// </summary>
            CardInserted,
            /// <summary>
            /// at least one nozzle is in lifting state.
            /// </summary>
            NozzleLiftedOrFueling,
            /// <summary>
            /// have not received heart beat for a while.
            /// </summary>
            Disconnected,
        }

        /// <summary>
        /// if diff(DateTime.Now, lastPulseTime) >= ThisValue, considered as disconnected. 
        /// </summary>
        private const int maxPulseDueMinutes = 2;
        private DateTime lastPulseTime;

        /// <summary>
        /// notify the underlying comm is still on going.
        /// </summary>
        public void Pulse()
        {
            this.lastPulseTime = DateTime.Now;
        }

        private LogicalPumpState pumpState;

        public LogicalPumpState PumpState
        {
            get
            {
                return this.pumpState;
            }
            set
            {
                if (this.pumpState != value)
                {
                    this.pumpState = value;
                    var safe = this.PropertyChanged;
                    safe?.Invoke(this, new PropertyChangedEventArgs("PumpState"));
                }
            }
        }

        public void UpdatePumpStateByMessage(PumpStateChangeRequest msg)
        {
            PumpStateChangeRequest.ParseSubMessages(msg.SubMessageRaw, msg.SubMessageCount, msg);
            if (msg.StateNozzleOperatingSubMessages == null && msg.StateCardInsertedSubMessages == null)
            {
                if (this.Nozzles_油枪组.Any())
                    this.Nozzles_油枪组.ToList().ForEach(n => n.ResetState());
                this.PumpState = LogicalPump.LogicalPumpState.Idle;
            }
            else
            {
                if (msg.StateNozzleOperatingSubMessages != null && msg.StateNozzleOperatingSubMessages.Any())
                {
                    // further set state for specified Nozzle
                    foreach (var busyNozzle in msg.StateNozzleOperatingSubMessages)
                    {
                        var targetLogicalNozzle = this.Nozzles_油枪组.FirstOrDefault(n => n.NozzleNumber == busyNozzle.MZN枪号);
                        // the purpose adding nozzle here is just like a trying to recovery the most info for a pump, so some nozzles will be missed.
                        if (targetLogicalNozzle == null)
                        {
                            targetLogicalNozzle = new LogicalNozzle()
                            {
                                NozzleNumber = busyNozzle.MZN枪号,
                            };
                            this.AddNozzle(targetLogicalNozzle);
                        }
                        targetLogicalNozzle.ResetState();
                        targetLogicalNozzle.Amount = busyNozzle.AMN数额;
                        targetLogicalNozzle.Volumn = busyNozzle.VOL升数;
                        targetLogicalNozzle.Price = busyNozzle.PRC价格;
                        targetLogicalNozzle.NozzleState = LogicalNozzle.PumpNozzleState.BusyLiftedOrFueling;
                    }

                    this.PumpState = LogicalPump.LogicalPumpState.NozzleLiftedOrFueling;
                }
                else if (msg.StateCardInsertedSubMessages != null && msg.StateCardInsertedSubMessages.Any())
                {
                    foreach (var cardInsertedNozzle in msg.StateCardInsertedSubMessages)
                    {
                        var targetLogicalNozzle = this.Nozzles_油枪组.FirstOrDefault(n => n.NozzleNumber == cardInsertedNozzle.MZN枪号);
                        // the purpose adding nozzle here is just like a trying to recovery the most info for a pump, so some nozzles will be missed.
                        if (targetLogicalNozzle == null)
                        {
                            targetLogicalNozzle = new LogicalNozzle() { NozzleNumber = cardInsertedNozzle.MZN枪号 };
                            this.AddNozzle(targetLogicalNozzle);
                        }
                        targetLogicalNozzle.ResetState();
                        targetLogicalNozzle.InsertedCardNumber = cardInsertedNozzle.ASN卡应用号;
                        targetLogicalNozzle.InsertedCardStateCode = cardInsertedNozzle.CardSt卡状态;
                        targetLogicalNozzle.InsertedCardBalance = cardInsertedNozzle.BAL余额;
                        targetLogicalNozzle.NozzleState = LogicalNozzle.PumpNozzleState.BusyCardInserted;
                    }

                    this.PumpState = LogicalPump.LogicalPumpState.CardInserted;
                }

            }
        }

        public static PumpStationInfo GetDefaultPumpStationInfo()
        {
            var result = new PumpStationInfo();
            result.Ver = 0xDF;
            result.City_地市代码 = 11;
            result.Prov_省代号 = 11;
            result.Superior_上级单位代号 = "11111111";
            result.S_ID_加油站ID = "11111111";
            result.POS_P_通讯终端逻辑编号 = 0;
#warning where this configuration info come from?
            result.GUN_N_油枪数 = 4;
#warning where this configuration info number come from?
            result.MZN_单个油枪 = new List<byte>() { 1, 2, 3, 4 };
            return result;
        }

        public static FundamentalBlackList GetFundamentalBlackList()
        {
            var result = new FundamentalBlackList(0x00AB, DateTime.Now.Subtract(new TimeSpan(366, 1, 1, 1)), DateTime.Now.AddYears(1), BlackAndWhiteListBase.BlackListEffectiveAreaEnum.本省黑名单, 1, 1);
            result.名单数量 = 1;
            result.CardSerialNumbers = new List<CardSerialNumber>();
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01111111111111111111", });
            return result;
        }

        public static IncrementalBlackList GetIncrementalBlackList()
        {
            var result = new IncrementalBlackList(0x71, DateTime.Now.Subtract(new TimeSpan(366, 1, 1, 1)), DateTime.Now.AddYears(1), BlackAndWhiteListBase.BlackListEffectiveAreaEnum.本省黑名单, 1, 1);
            result.名单数量 = 1;
            result.CardSerialNumbers = new List<CardSerialNumber>();
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01111111111111111111", });
            return result;
        }

        public static DeletionBlackList GetDeletionBlackList()
        {
            var result = new DeletionBlackList(0x01, DateTime.Now.Subtract(new TimeSpan(366, 1, 1, 1)), DateTime.Now.AddYears(1), BlackAndWhiteListBase.BlackListEffectiveAreaEnum.全国黑名单, 1, 1);
            result.名单数量 = 0;
            //result.CardSerialNumbers = new List<CardSerialNumber>();
            //result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "09999911111111111111", });
            return result;
        }

        public static WhiteList GetWhiteList()
        {
            var result = new WhiteList(0x8E, DateTime.Now.Subtract(new TimeSpan(366, 1, 1, 1)), DateTime.Now.AddYears(1), BlackAndWhiteListBase.BlackListEffectiveAreaEnum.本省黑名单, 11, 1);

            #region
            result.CardSerialNumbers = new List<CardSerialNumber>();
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000419900800001084", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000001", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000002", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000003", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000004", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000005", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000006", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000007", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000008", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000009", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000010", });

            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000011", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000012", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000013", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000014", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000015", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000016", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000017", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000018", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000019", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000020", });

            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000021", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000022", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000023", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000024", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000025", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000026", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000027", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000028", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000029", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000030", });

            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000031", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000032", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000033", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000034", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000035", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000036", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000037", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000038", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000039", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000040", });

            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000041", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000042", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000043", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000044", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000045", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000046", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000047", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000048", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000049", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000050", });

            #endregion

            return result;
        }

        /// <summary>
        /// the latest configurated Price list.
        /// </summary>
        public FuelPriceList FulePriceList { get; set; }

        /// <summary>
        /// the latest configurated station info.
        /// </summary>
        public PumpStationInfo PumpStationInfo { get; set; }

        public static FuelPriceList GetDefaultFuelPriceList()
        {
            var result = new FuelPriceList(0xF8);
            result.Set_V_D_andT_新油品油价生效时间(DateTime.Now.Subtract(new TimeSpan(20, 1, 1, 1)));
            result.FieldNum_记录数 = 4;
            result.当前油品油价记录List = new List<FuelPriceRecord>();
            result.当前油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 1,
                O_Type_油品代码 = "1031",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x02B0) }
            });
            result.当前油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 4,
                O_Type_油品代码 = "1031",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x02B0) }
            });
            result.当前油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 2,
                O_Type_油品代码 = "1011",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x024C) }
            });
            result.当前油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 3,
                O_Type_油品代码 = "1011",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x024C) }
            });


            result.新油品油价记录List = new List<FuelPriceRecord>();
            result.新油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 1,
                O_Type_油品代码 = "1031",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x02B0) }
            });
            result.新油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 4,
                O_Type_油品代码 = "1031",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x02B0) }
            });
            result.新油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 2,
                O_Type_油品代码 = "1011",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x024C) }
            });
            result.新油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 3,
                O_Type_油品代码 = "1011",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x024C) }
            });
            return result;
        }
    }
}
