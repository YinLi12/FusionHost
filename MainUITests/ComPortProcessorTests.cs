using Microsoft.VisualStudio.TestTools.UnitTesting;
using MainUI;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using MessageParser;

namespace MainUI.Tests
{
    [TestClass()]
    public class ComPortProcessorTests
    {
        byte[] packageHeader = new byte[] { 0xFA };
        [TestMethod()]
        public void PumpGenericInquiryRequestProcess_PumpActive_MessageTest1()
        {
            List<Byte[]> pendingForSend = new List<byte[]>();
            pendingForSend.Add("FA E0 FF 00 00 01 30 F0 03".ToBytes());//加油机主动Inquiry
            //pendingForSend.Add("FA E0 FF 01 00 04 33 05 00 DF 1D 39".ToBytes());//请求允许下载：油站通用信息，
            //pendingForSend.Add("FA E0 FF 02 00 05 34 05 00 00 02 E7 F0".ToBytes());//请求下载数据：油站通用信息， OFFSET:0, SEG:2
            //pendingForSend.Add("FA E0 FF 03 00 01 30 B4 03".ToBytes());////加油机主动Inquiry

            //pendingForSend.Add("FA E0 FF 04 00 04 33 04 00 F8 C7 7D".ToBytes());//请求允许下载：油品油价表 ， 
            //pendingForSend.Add("FA E0 FF 05 00 05 34 04 00 00 06 3E B1".ToBytes());//求下载数据：油品油价表， OFFSET:0, SEG:6

            //pendingForSend.Add("FA 00 00 00 00 01 30 84 01".ToBytes());//加油机主动Inquiry
            //pendingForSend.Add("FA 00 00 01 00 04 33 03 00 8E 20 9E".ToBytes());//请求允许下载：白名单 ，
            //pendingForSend.Add("FA 00 00 02 00 05 34 03 00 00 01 CC F1".ToBytes());//求下载数据：白名单, S_OFFSET_段偏移: 0, SEG_段数_segs: 1
            //pendingForSend.Add("FA 00 00 03 00 05 34 03 00 01 0F 54 B0".ToBytes());//Content_内容: 白名单, S_OFFSET_段偏移: 1, SEG_段数_segs: 15
            //pendingForSend.Add("FA 00 00 04 00 05 34 03 00 10 0F E2 FD".ToBytes());//Content_内容: 白名单, S_OFFSET_段偏移: 16, SEG_段数_segs: 15
            //pendingForSend.Add("FA 00 00 05 00 05 34 03 00 1F 0F DE 39".ToBytes());//Content_内容: 白名单, S_OFFSET_段偏移: 31, SEG_段数_segs: 15
            //pendingForSend.Add("FA 00 00 06 00 05 34 03 00 2E 0F 5B 6C".ToBytes());//Content_内容: 白名单, S_OFFSET_段偏移: 46, SEG_段数_segs: 15
            //pendingForSend.Add("FA 00 00 07 00 05 34 03 00 3D 0F A7 A0".ToBytes());//Content_内容: 白名单, S_OFFSET_段偏移: 61, SEG_段数_segs: 15
            //pendingForSend.Add("FA 00 00 08 00 05 34 03 00 4C 0F B7 C4".ToBytes());//Content_内容: 白名单, S_OFFSET_段偏移: 76, SEG_段数_segs: 15
            //pendingForSend.Add("FA 00 00 09 00 05 34 03 00 5B 0F 8B 0A".ToBytes());//Content_内容: 白名单, S_OFFSET_段偏移: 91, SEG_段数_segs: 15
            //pendingForSend.Add("FA 00 00 0A 00 05 34 03 00 6A 0F 0E 5F".ToBytes());//Content_内容: 白名单, S_OFFSET_段偏移: 106, SEG_段数_segs: 15
            //pendingForSend.Add("FA 00 00 0B 00 05 34 03 00 79 05 F5 13".ToBytes());//Content_内容: 白名单, S_OFFSET_段偏移: 121, SEG_段数_segs: 5
            //pendingForSend.Add("FA 00 00 0A 00 05 34 03 00 6A 0F 0E 5F".ToBytes());//Content_内容: 白名单, S_OFFSET_段偏移: 106, SEG_段数_segs: 15

            //pendingForSend.Add("FA 00 00 0F 00 04 33 00 00 AB 3B 40".ToBytes());//请求下载数据：基础黑名单 IBL_VER_基础黑名单版本: 171
            //pendingForSend.Add("FA 00 00 10 00 05 34 00 00 00 01 5D 71".ToBytes());//Content_内容: 基础黑名单, S_OFFSET_段偏移: 0, SEG_段数_segs: 1
            //pendingForSend.Add("FA 00 00 11 00 05 34 00 00 01 01 01 B1".ToBytes());//Content_内容: 基础黑名单, S_OFFSET_段偏移: 1, SEG_段数_segs: 1

            //pendingForSend.Add("FA 00 00 12 00 02 310053 0C".ToBytes());//PumpStateChangeRequest31  命令中提供的信息n1   followingsubmsgEmpty??

            var processor = new ComPortProcessor(new SerialPort("COM14"), ComPortProcessor.CommActiveMode.PumpActive);
            processor.OnComPortDataWriting += (sender, arg) =>
            {
                arg.Continue = false;
                //Assert.AreEqual(typeof(PcGenericInquiryWithRichInfoResponse), arg.Message.GetType());
                //var debug = arg.WritingBytes.Select(s => s.ToString("X").PadLeft(2, '0')).Aggregate((acc, n) => acc + " " + n);
                //Console.WriteLine(debug);
            };
            processor.OnMessageReceiving += (_, arg) =>
            {
                var p = _ as ComPortProcessor;
                // OnMessageReceiving event fired indicates the COM Port have a real pump connected at other peer, 
                // so create the LogicalPump object, and draw the ICON.
                if (p.Pump == null)
                {
                    p.Pump = new LogicalPump
                    {
                        ComPortName = p.SerialPort.PortName,
                    };
                    // set all to default since this is just for unit test.
                    p.Pump.FulePriceList = LogicalPump.GetDefaultFuelPriceList();
                    p.Pump.PumpStationInfo = LogicalPump.GetDefaultPumpStationInfo();
                    p.Pump.AllowCommWithRealPhsicalPump = true;
                }
            };
            Assert.AreEqual(true, processor.Pump == null);
            Parser parser = new Parser();
            for (int i = 0; i < pendingForSend.Count; i++)
                processor.ProcessMessage(parser.Deserialize(pendingForSend[i]));
            Assert.AreEqual(true, processor.Pump != null);



            //Assert.AreEqual(LogicalPump.LogicalPumpState.Idle, processor.Pump.PumpState);
            //processor.ProcessMessage(parser.Deserialize("FA 00 00 17 00 40 31020101   1001 00 04 19 90 08 00 0010 8400 0077 46 23 7401031001 00 04 19 90 08 00 00 10 8400 0077 46 23 74FF 77".ToBytes()));//PumpStateChangeRequest31	信息条数		卡插入  NZN	LEN卡信息数据长度 	   ASN	
            //Assert.AreEqual(LogicalPump.LogicalPumpState.CardInserted, processor.Pump.PumpState);


            //Assert.AreEqual(true, processor.Pump.Nozzles_油枪组 != null);
            //processor.ProcessMessage(parser.Deserialize("FA 00 00 2B 00 22 38  04     01    00 B3 42 85     02     00 00 20 6B   03 00 00 93 DC 04 00 00 5C 2F D6 CA".ToBytes())); //加油机上送累计数命令 油枪数   枪号 升累计         枪号 升累计      ......
            //Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(p => p.NozzleNumber == 1).VolumnAccumulator == 0x00B34285);
            //Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(p => p.NozzleNumber == 2).VolumnAccumulator == 0x0000206B);
            //TRX DONE NOTIFICATION  
            //FA 00 00 04 00 96 32  00 00 00 014520 17 03 05 13 57 5701 00 04 19 90 08 00 00 10 8477 46 23 7400 00 0000 0000 00 00 0000 00 00 0000 00 00 0000 00 00 00 00 00 00 00 00 0000 00 00 00 00 0000 00 00 0000000000     01     10 31     00 00 00     00 00       94      00 B3 41 EC      00 00 00 00 00 00 00 00 00 00 00   69 A6 1B EE      B5 99

            //PumpStateChangeRequest, nozzle change
            //FA E2FF 18 0073 31 04     01 12 11 12345678901234567890 4455 99229654 32         02 02 01 223344 556677 8907        01 12 11 12345678901234567890 4455 9922 965432              01 12 11 12345678901234567890 4455 9922 965432 88 99
        }

        [TestMethod()]
        public void PumpStateChangeRequest_PumpActive_MessageTest1()
        {
            List<Byte[]> pendingForSend = new List<byte[]>();
            pendingForSend.Add("FA E0 FF 00 00 01 30 F0 03".ToBytes());//加油机主动Inquiry

            //PumpStateChangeRequest, SubMessageCount= 2.
            //CardInserted: cardNo->01000419900800001084, nozzleNo->1, cardBalance->2001085300CardInserted: cardNo->01000419900800001084, nozzleNo->3, cardBalance->2001085300
            pendingForSend.Add("FA 00 00 17 00 40 31020101   1001 00 04 19 90 08 00 0010 8400 0077 46 23 7401031001 00 04 19 90 08 00 00 10 8400 0077 46 23 74FF 77".ToBytes());//加油机主动Inquiry

            //PumpStateChangeRequest, set to idle
            pendingForSend.Add("FA 00 00 12 00 02 310053 0C".ToBytes());//PumpStateChangeRequest31  命令中提供的信息n1   followingsubmsgEmpty??

            var processor = new ComPortProcessor(new SerialPort("COM14"), ComPortProcessor.CommActiveMode.PumpActive);
            processor.OnComPortDataWriting += (sender, arg) =>
            {
                arg.Continue = false;
            };
            processor.OnMessageReceiving += (_, arg) =>
            {
                var p = _ as ComPortProcessor;
                // OnMessageReceiving event fired indicates the COM Port have a real pump connected at other peer, 
                // so create the LogicalPump object, and draw the ICON.
                if (p.Pump == null)
                {
                    p.Pump = new LogicalPump
                    {
                        ComPortName = p.SerialPort.PortName,
                    };
                    // set all to default since this is just for unit test.
                    p.Pump.FulePriceList = LogicalPump.GetDefaultFuelPriceList();
                    p.Pump.PumpStationInfo = LogicalPump.GetDefaultPumpStationInfo();
                    p.Pump.AllowCommWithRealPhsicalPump = true;
                }
            };
            Assert.AreEqual(true, processor.Pump == null);
            Parser parser = new Parser();
            for (int i = 0; i < pendingForSend.Count; i++)
            {
                processor.ProcessMessage(parser.Deserialize(pendingForSend[i]));
                if (i == 0)
                {
                    Assert.AreEqual(true, processor.Pump.PumpState == LogicalPump.LogicalPumpState.Idle);
                    Assert.AreEqual(true, processor.Pump != null);
                    Assert.AreEqual(false, processor.Pump.Nozzles_油枪组.Any());
                }
                else if (i == 1)
                {
                    Assert.AreEqual(true, processor.Pump.PumpState == LogicalPump.LogicalPumpState.CardInserted);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.Any());
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 1).NozzleState == LogicalNozzle.PumpNozzleState.BusyCardInserted);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 1).InsertedCardNumber == "01000419900800001084");
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 1).InsertedCardBalance == 2001085300);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 3).NozzleState == LogicalNozzle.PumpNozzleState.BusyCardInserted);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 3).InsertedCardNumber == "01000419900800001084");
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 3).InsertedCardBalance == 2001085300);
                }
                else if (i == 2)
                {
                    Assert.AreEqual(true, processor.Pump.PumpState == LogicalPump.LogicalPumpState.Idle);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.Any());
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.All(n => n.NozzleState == LogicalNozzle.PumpNozzleState.Idle));
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.All(n => string.IsNullOrEmpty(n.InsertedCardNumber)));
                }
            }












            //Assert.AreEqual(LogicalPump.LogicalPumpState.Idle, processor.Pump.PumpState);
            //processor.ProcessMessage(parser.Deserialize("FA 00 00 17 00 40 31020101   1001 00 04 19 90 08 00 0010 8400 0077 46 23 7401031001 00 04 19 90 08 00 00 10 8400 0077 46 23 74FF 77".ToBytes()));//PumpStateChangeRequest31	信息条数		卡插入  NZN	LEN卡信息数据长度 	   ASN	
            //Assert.AreEqual(LogicalPump.LogicalPumpState.CardInserted, processor.Pump.PumpState);


            //Assert.AreEqual(true, processor.Pump.Nozzles_油枪组 != null);
            //processor.ProcessMessage(parser.Deserialize("FA 00 00 2B 00 22 38  04     01    00 B3 42 85     02     00 00 20 6B   03 00 00 93 DC 04 00 00 5C 2F D6 CA".ToBytes())); //加油机上送累计数命令 油枪数   枪号 升累计         枪号 升累计      ......
            //Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(p => p.NozzleNumber == 1).VolumnAccumulator == 0x00B34285);
            //Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(p => p.NozzleNumber == 2).VolumnAccumulator == 0x0000206B);
            //TRX DONE NOTIFICATION  
            //FA 00 00 04 00 96 32  00 00 00 014520 17 03 05 13 57 5701 00 04 19 90 08 00 00 10 8477 46 23 7400 00 0000 0000 00 00 0000 00 00 0000 00 00 0000 00 00 00 00 00 00 00 00 0000 00 00 00 00 0000 00 00 0000000000     01     10 31     00 00 00     00 00       94      00 B3 41 EC      00 00 00 00 00 00 00 00 00 00 00   69 A6 1B EE      B5 99

            //PumpStateChangeRequest, nozzle change
            //FA E2FF 18 0073 31 04     01 12 11 12345678901234567890 4455 99229654 32         02 02 01 223344 556677 8907        01 12 11 12345678901234567890 4455 9922 965432              01 12 11 12345678901234567890 4455 9922 965432 88 99
        }

        [TestMethod()]
        public void PumpStateChangeRequest_PumpActive_MessageTest2()
        {
            List<Byte[]> pendingForSend = new List<byte[]>();
            pendingForSend.Add("FA E0 FF 00 00 01 30 F0 03".ToBytes());//加油机主动Inquiry

            //PumpStateChangeRequest, SubMessageCount = 1.
            //NozzleChanged: nozzleNo->2, vol->5596791, amount->2241348, price->35079
            pendingForSend.Add("FA E2FF 18 0013 31 01          02 02 01 223344 556677 8907   8899".ToBytes());//加油机主动Inquiry

            //PumpStateChangeRequest, set to idle
            pendingForSend.Add("FA 00 00 12 00 02 310053 0C".ToBytes());//PumpStateChangeRequest31  命令中提供的信息n1   followingsubmsgEmpty??

            var processor = new ComPortProcessor(new SerialPort("COM14"), ComPortProcessor.CommActiveMode.PumpActive);
            processor.OnComPortDataWriting += (sender, arg) =>
            {
                arg.Continue = false;
            };
            processor.OnMessageReceiving += (_, arg) =>
            {
                var p = _ as ComPortProcessor;
                // OnMessageReceiving event fired indicates the COM Port have a real pump connected at other peer, 
                // so create the LogicalPump object, and draw the ICON.
                if (p.Pump == null)
                {
                    p.Pump = new LogicalPump
                    {
                        ComPortName = p.SerialPort.PortName,
                    };
                    // set all to default since this is just for unit test.
                    p.Pump.FulePriceList = LogicalPump.GetDefaultFuelPriceList();
                    p.Pump.PumpStationInfo = LogicalPump.GetDefaultPumpStationInfo();
                    p.Pump.AllowCommWithRealPhsicalPump = true;
                }
            };
            Assert.AreEqual(true, processor.Pump == null);
            Parser parser = new Parser();
            for (int i = 0; i < pendingForSend.Count; i++)
            {
                processor.ProcessMessage(parser.Deserialize(pendingForSend[i]));
                if (i == 0)
                {
                    Assert.AreEqual(true, processor.Pump.PumpState == LogicalPump.LogicalPumpState.Idle);
                    Assert.AreEqual(true, processor.Pump != null);
                    Assert.AreEqual(false, processor.Pump.Nozzles_油枪组.Any());
                }
                else if (i == 1)
                {
                    Assert.AreEqual(true, processor.Pump.PumpState == LogicalPump.LogicalPumpState.NozzleLiftedOrFueling);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.Any());
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 2).NozzleState == LogicalNozzle.PumpNozzleState.BusyLiftedOrFueling);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 2).Volumn == 5596791);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 2).Amount == 2241348);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 2).Price == 35079);
                }
                else if (i == 2)
                {
                    Assert.AreEqual(true, processor.Pump.PumpState == LogicalPump.LogicalPumpState.Idle);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.Any());
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.All(n => n.Volumn == 0));
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.All(n => n.Amount == 0));
                }
            }












            //Assert.AreEqual(LogicalPump.LogicalPumpState.Idle, processor.Pump.PumpState);
            //processor.ProcessMessage(parser.Deserialize("FA 00 00 17 00 40 31020101   1001 00 04 19 90 08 00 0010 8400 0077 46 23 7401031001 00 04 19 90 08 00 00 10 8400 0077 46 23 74FF 77".ToBytes()));//PumpStateChangeRequest31	信息条数		卡插入  NZN	LEN卡信息数据长度 	   ASN	
            //Assert.AreEqual(LogicalPump.LogicalPumpState.CardInserted, processor.Pump.PumpState);


            //Assert.AreEqual(true, processor.Pump.Nozzles_油枪组 != null);
            //processor.ProcessMessage(parser.Deserialize("FA 00 00 2B 00 22 38  04     01    00 B3 42 85     02     00 00 20 6B   03 00 00 93 DC 04 00 00 5C 2F D6 CA".ToBytes())); //加油机上送累计数命令 油枪数   枪号 升累计         枪号 升累计      ......
            //Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(p => p.NozzleNumber == 1).VolumnAccumulator == 0x00B34285);
            //Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(p => p.NozzleNumber == 2).VolumnAccumulator == 0x0000206B);
            //TRX DONE NOTIFICATION  
            //FA 00 00 04 00 96 32  00 00 00 014520 17 03 05 13 57 5701 00 04 19 90 08 00 00 10 8477 46 23 7400 00 0000 0000 00 00 0000 00 00 0000 00 00 0000 00 00 00 00 00 00 00 00 0000 00 00 00 00 0000 00 00 0000000000     01     10 31     00 00 00     00 00       94      00 B3 41 EC      00 00 00 00 00 00 00 00 00 00 00   69 A6 1B EE      B5 99

            //PumpStateChangeRequest, nozzle change
            //FA E2FF 18 0013 31 01          02 02 01 223344 556677 8907    8899 
        }

        [TestMethod()]
        public void PcAskReadPumpAccumulator_PumpActive_MessageTest1()
        {
            List<Byte[]> pendingForSend = new List<byte[]>();
            pendingForSend.Add("FA E0 FF 00 00 01 30 F0 03".ToBytes());//加油机主动Inquiry

            //PumpStateChangeRequest, SubMessageCount = 1.
            //NozzleChanged: nozzleNo->2, vol->5596791, amount->2241348, price->35079
            pendingForSend.Add("FA E2FF 18 0013 31 01          02 02 01 223344 556677 8907   8899".ToBytes());//加油机主动Inquiry

            //PumpStateChangeRequest, set to idle
            pendingForSend.Add("FA 00 00 12 00 02 310053 0C".ToBytes());//PumpStateChangeRequest31  命令中提供的信息n1   followingsubmsgEmpty??

            // GUN_N_油枪数: 4
            //   [0]NZN_枪号: 1, V_TOT_升累计: 11747973
            //   [1]NZN_枪号: 2, V_TOT_升累计: 8299
            //   [2]NZN_枪号: 3, V_TOT_升累计: 37852
            //   [3]NZN_枪号: 4, V_TOT_升累计: 23599
            pendingForSend.Add(" FA 00 00 2B 00 22 38  04     01    00 B3 42 85     02     00 00 20 6B   03 00 00 93 DC 04 00 00 5C 2F D6 CA".ToBytes());

            var processor = new ComPortProcessor(new SerialPort("COM14"), ComPortProcessor.CommActiveMode.PumpActive);
            processor.OnComPortDataWriting += (sender, arg) =>
            {
                arg.Continue = false;
            };
            processor.OnMessageReceiving += (_, arg) =>
            {
                var p = _ as ComPortProcessor;
                // OnMessageReceiving event fired indicates the COM Port have a real pump connected at other peer, 
                // so create the LogicalPump object, and draw the ICON.
                if (p.Pump == null)
                {
                    p.Pump = new LogicalPump
                    {
                        ComPortName = p.SerialPort.PortName,
                    };
                    // set all to default since this is just for unit test.
                    p.Pump.FulePriceList = LogicalPump.GetDefaultFuelPriceList();
                    p.Pump.PumpStationInfo = LogicalPump.GetDefaultPumpStationInfo();
                    p.Pump.AllowCommWithRealPhsicalPump = true;
                }
            };
            Assert.AreEqual(true, processor.Pump == null);
            Parser parser = new Parser();
            for (int i = 0; i < pendingForSend.Count; i++)
            {
                processor.ProcessMessage(parser.Deserialize(pendingForSend[i]));
                if (i == 0)
                {
                    Assert.AreEqual(true, processor.Pump.PumpState == LogicalPump.LogicalPumpState.Idle);
                    Assert.AreEqual(true, processor.Pump != null);
                    Assert.AreEqual(false, processor.Pump.Nozzles_油枪组.Any());
                }
                else if (i == 1)
                {
                    Assert.AreEqual(true, processor.Pump.PumpState == LogicalPump.LogicalPumpState.NozzleLiftedOrFueling);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.Any());
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 2).NozzleState == LogicalNozzle.PumpNozzleState.BusyLiftedOrFueling);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 2).Volumn == 5596791);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 2).Amount == 2241348);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 2).Price == 35079);
                }
                else if (i == 2)
                {
                    Assert.AreEqual(true, processor.Pump.PumpState == LogicalPump.LogicalPumpState.Idle);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.Any());
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.All(n => n.Volumn == 0));
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.All(n => n.Amount == 0));
                }
                else if (i == 3)
                {
                    Assert.AreEqual(true, processor.Pump.PumpState == LogicalPump.LogicalPumpState.Idle);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.Count() == 4);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.All(n => n.Volumn == 0));
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.All(n => n.Amount == 0));
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.All(n => string.IsNullOrEmpty(n.InsertedCardNumber)));

                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 1).VolumnAccumulator == 11747973);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 2).VolumnAccumulator == 8299);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 3).VolumnAccumulator == 37852);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 4).VolumnAccumulator == 23599);
                }
            }












            //Assert.AreEqual(LogicalPump.LogicalPumpState.Idle, processor.Pump.PumpState);
            //processor.ProcessMessage(parser.Deserialize("FA 00 00 17 00 40 31020101   1001 00 04 19 90 08 00 0010 8400 0077 46 23 7401031001 00 04 19 90 08 00 00 10 8400 0077 46 23 74FF 77".ToBytes()));//PumpStateChangeRequest31	信息条数		卡插入  NZN	LEN卡信息数据长度 	   ASN	
            //Assert.AreEqual(LogicalPump.LogicalPumpState.CardInserted, processor.Pump.PumpState);


            //Assert.AreEqual(true, processor.Pump.Nozzles_油枪组 != null);
            //processor.ProcessMessage(parser.Deserialize("FA 00 00 2B 00 22 38  04     01    00 B3 42 85     02     00 00 20 6B   03 00 00 93 DC 04 00 00 5C 2F D6 CA".ToBytes())); //加油机上送累计数命令 油枪数   枪号 升累计         枪号 升累计      ......
            //Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(p => p.NozzleNumber == 1).VolumnAccumulator == 0x00B34285);
            //Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(p => p.NozzleNumber == 2).VolumnAccumulator == 0x0000206B);
            //TRX DONE NOTIFICATION  
            //FA 00 00 04 00 96 32  00 00 00 014520 17 03 05 13 57 5701 00 04 19 90 08 00 00 10 8477 46 23 7400 00 0000 0000 00 00 0000 00 00 0000 00 00 0000 00 00 00 00 00 00 00 00 0000 00 00 00 00 0000 00 00 0000000000     01     10 31     00 00 00     00 00       94      00 B3 41 EC      00 00 00 00 00 00 00 00 00 00 00   69 A6 1B EE      B5 99

            //PumpStateChangeRequest, nozzle change
            //FA E2FF 18 0013 31 01          02 02 01 223344 556677 8907    8899 
        }

        [TestMethod()]
        public void PumpNotifyTransactionDone_PumpActive_MessageTest1()
        {
            List<Byte[]> pendingForSend = new List<byte[]>();
            pendingForSend.Add("FA E0 FF 00 00 01 30 F0 03".ToBytes());//加油机主动Inquiry

            //PumpStateChangeRequest, SubMessageCount = 1.
            //NozzleChanged: nozzleNo->2, vol->5596791, amount->2241348, price->35079
            pendingForSend.Add("FA E2FF 18 0013 31 01          02 02 01 223344 556677 8907   8899".ToBytes());//加油机主动Inquiry

            //PumpStateChangeRequest, set to idle
            pendingForSend.Add("FA 00 00 12 00 02 310053 0C".ToBytes());//PumpStateChangeRequest31  命令中提供的信息n1   followingsubmsgEmpty??

            // GUN_N_油枪数: 4
            //   [0]NZN_枪号: 1, V_TOT_升累计: 11747973
            //   [1]NZN_枪号: 2, V_TOT_升累计: 8299
            //   [2]NZN_枪号: 3, V_TOT_升累计: 37852
            //   [3]NZN_枪号: 4, V_TOT_升累计: 23599
            pendingForSend.Add(" FA 00 00 2B 00 22 38  04     01    00 B3 42 85     02     00 00 20 6B   03 00 00 93 DC 04 00 00 5C 2F D6 CA".ToBytes());

            //POS_TTC: 1, T_Type: 69, TIME_Raw: 20170305135757, ASN卡应用号: 01000419900800001084, 
            //BAL余额: 2001085300, AMN数额: 0, CTC: 0, TAC电子签名: 0, GMAC电子签名: 0, 
            //PSAM_TAC灰锁签名: 0, PSAM_ASN_PSAM应用号: 00000000000000000000, PSAM_TTC: 0, 
            //DS_扣款来源: 0, UNIT_结算单位_方式: 0, C_TYPE_卡类: 0, VER_版本: 0, NZN_枪号: 1, 
            //G_CODE_油品代码: 1031, VOL_升数: 0, PRC_成交价格: 0, EMP_员工号: 148, 
            //V_TOT_升累计: 11747820, T_MAC_终端数据认证码: 1772493806
            pendingForSend.Add("FA 00 00 04 00 96 32  00 00 00 014520 17 03 05 13 57 5701 00 04 19 90 08 00 00 10 8477 46 23 7400 00 0000 0000 00 00 0000 00 00 0000 00 00 0000 00 00 00 00 00 00 00 00 0000 00 00 00 00 0000 00 00 0000000000     01     10 31     00 00 00     00 00       94      00 B3 41 EC      00 00 00 00 00 00 00 00 00 00 00   69 A6 1B EE      B5 99".ToBytes());

            //POS_TTC: 1, T_Type: 69, TIME_Raw: 20170318135757, ASN卡应用号: 01008719900800001084, 
            //BAL余额: 2001085300, AMN数额: 0, CTC: 0, TAC电子签名: 0, GMAC电子签名: 0, 
            //PSAM_TAC灰锁签名: 0, PSAM_ASN_PSAM应用号: 00000000000000000000, PSAM_TTC: 0, 
            //DS_扣款来源: 0, UNIT_结算单位_方式: 0, C_TYPE_卡类: 0, VER_版本: 0, NZN_枪号: 1, 
            //G_CODE_油品代码: 1031, VOL_升数: 0, PRC_成交价格: 0, EMP_员工号: 148, 
            //V_TOT_升累计: 11747820, T_MAC_终端数据认证码: 1772493806
            pendingForSend.Add("FA 00 00 04 00 96 32  00 00 00 014520 17 03 18 13 57 5701 00 87 19 90 08 00 00 10 8477 46 23 7400 00 0000 0000 00 00 0000 00 00 0000 00 00 0000 00 00 00 00 00 00 00 00 0000 00 00 00 00 0000 00 00 0000000000     01     10 31     00 00 00     00 00       94      00 B3 41 EC      00 00 00 00 00 00 00 00 00 00 00   69 A6 1B EE      B5 99".ToBytes());

            var processor = new ComPortProcessor(new SerialPort("COM14"), ComPortProcessor.CommActiveMode.PumpActive);
            processor.OnComPortDataWriting += (sender, arg) =>
            {
                arg.Continue = false;
            };
            processor.OnMessageReceiving += (_, arg) =>
            {
                var p = _ as ComPortProcessor;
                // OnMessageReceiving event fired indicates the COM Port have a real pump connected at other peer, 
                // so create the LogicalPump object, and draw the ICON.
                if (p.Pump == null)
                {
                    p.Pump = new LogicalPump
                    {
                        ComPortName = p.SerialPort.PortName,
                    };
                    // set all to default since this is just for unit test.
                    p.Pump.FulePriceList = LogicalPump.GetDefaultFuelPriceList();
                    p.Pump.PumpStationInfo = LogicalPump.GetDefaultPumpStationInfo();
                    p.Pump.AllowCommWithRealPhsicalPump = true;
                }
            };
            Assert.AreEqual(true, processor.Pump == null);
            Parser parser = new Parser();
            for (int i = 0; i < pendingForSend.Count; i++)
            {
                processor.ProcessMessage(parser.Deserialize(pendingForSend[i]));
                if (i == 0)
                {
                    Assert.AreEqual(true, processor.Pump != null);
                    Assert.AreEqual(false, processor.Pump.Transactions.Any());
                    Assert.AreEqual(true, processor.Pump.PumpState == LogicalPump.LogicalPumpState.Idle);
                    Assert.AreEqual(false, processor.Pump.Nozzles_油枪组.Any());
                }
                else if (i == 1)
                {
                    Assert.AreEqual(true, processor.Pump.PumpState == LogicalPump.LogicalPumpState.NozzleLiftedOrFueling);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.Any());
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 2).NozzleState == LogicalNozzle.PumpNozzleState.BusyLiftedOrFueling);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 2).Volumn == 5596791);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 2).Amount == 2241348);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 2).Price == 35079);
                }
                else if (i == 2)
                {
                    Assert.AreEqual(true, processor.Pump.PumpState == LogicalPump.LogicalPumpState.Idle);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.Any());
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.All(n => n.Volumn == 0));
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.All(n => n.Amount == 0));
                }
                else if (i == 3)
                {
                    Assert.AreEqual(true, processor.Pump.PumpState == LogicalPump.LogicalPumpState.Idle);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.Count() == 4);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.All(n => n.Volumn == 0));
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.All(n => n.Amount == 0));
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.All(n => string.IsNullOrEmpty(n.InsertedCardNumber)));

                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 1).VolumnAccumulator == 11747973);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 2).VolumnAccumulator == 8299);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 3).VolumnAccumulator == 37852);
                    Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(n => n.NozzleNumber == 4).VolumnAccumulator == 23599);
                }
                else if (i == 4)
                {
                    Assert.AreEqual(true, processor.Pump.Transactions.Any());
                    Assert.AreEqual(true, processor.Pump.Transactions.Count() == 1);
                    Assert.AreEqual(true, processor.Pump.Transactions.First().ASN卡应用号 == "01000419900800001084");
                    Assert.AreEqual(true, processor.Pump.Transactions.First().BAL余额 == 2001085300);
                    Assert.AreEqual(true, processor.Pump.Transactions.First().NZN_枪号 == 1);
                    Assert.AreEqual(true, processor.Pump.Transactions.First().G_CODE_油品代码 == "1031");
                }
                else if (i == 5)
                {
                    Assert.AreEqual(true, processor.Pump.Transactions.Any());
                    Assert.AreEqual(true, processor.Pump.Transactions.Count() == 2);
                    Assert.AreEqual(true, processor.Pump.Transactions.Skip(1).First().ASN卡应用号 == "01008719900800001084");
                }
            }












            //Assert.AreEqual(LogicalPump.LogicalPumpState.Idle, processor.Pump.PumpState);
            //processor.ProcessMessage(parser.Deserialize("FA 00 00 17 00 40 31020101   1001 00 04 19 90 08 00 0010 8400 0077 46 23 7401031001 00 04 19 90 08 00 00 10 8400 0077 46 23 74FF 77".ToBytes()));//PumpStateChangeRequest31	信息条数		卡插入  NZN	LEN卡信息数据长度 	   ASN	
            //Assert.AreEqual(LogicalPump.LogicalPumpState.CardInserted, processor.Pump.PumpState);


            //Assert.AreEqual(true, processor.Pump.Nozzles_油枪组 != null);
            //processor.ProcessMessage(parser.Deserialize("FA 00 00 2B 00 22 38  04     01    00 B3 42 85     02     00 00 20 6B   03 00 00 93 DC 04 00 00 5C 2F D6 CA".ToBytes())); //加油机上送累计数命令 油枪数   枪号 升累计         枪号 升累计      ......
            //Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(p => p.NozzleNumber == 1).VolumnAccumulator == 0x00B34285);
            //Assert.AreEqual(true, processor.Pump.Nozzles_油枪组.First(p => p.NozzleNumber == 2).VolumnAccumulator == 0x0000206B);
            //TRX DONE NOTIFICATION  
            //FA 00 00 04 00 96 32  00 00 00 014520 17 03 05 13 57 5701 00 04 19 90 08 00 00 10 8477 46 23 7400 00 0000 0000 00 00 0000 00 00 0000 00 00 0000 00 00 00 00 00 00 00 00 0000 00 00 00 00 0000 00 00 0000000000     01     10 31     00 00 00     00 00       94      00 B3 41 EC      00 00 00 00 00 00 00 00 00 00 00   69 A6 1B EE      B5 99

            //PumpStateChangeRequest, nozzle change
            //FA E2FF 18 0013 31 01          02 02 01 223344 556677 8907    8899 
        }
    }
}