using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageParser;
using MessageParser.MessageEntity.Outgoing.BlackAndWhiteList;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageParserTest
{
    [TestClass]
    public class MessageUnitTest
    {
        Parser p = new Parser();
        //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)
        byte[] packageHeader = new byte[] { 0xFA };

        [TestMethod]
        public void PumpGenericInquiryRequest_Deserialize_TestMethod1()
        {
            string raw_without_crc16 = "E0FF40000130";
            var crc16 = raw_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullRaw = packageHeader.Concat(raw_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            var result = p.Deserialize(fullRaw.ToArray()) as PumpGenericInquiryRequest;
            Assert.AreEqual(true, result.HANDLE == 0x30);
            Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageTemplateBase.MessageCallerSide.Host);
        }

        [TestMethod]
        public void PumpGenericInquiryRequest_Deserialize_TestMethod2()
        {
            string raw_without_crc16 = "E0FF00000130";
            var crc16 = raw_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullRaw = packageHeader.Concat(raw_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            var result = p.Deserialize(fullRaw.ToArray()) as PumpGenericInquiryRequest;
            Assert.AreEqual(true, result.HANDLE == 0x30);
            Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageTemplateBase.MessageCallerSide.Pump);
        }

        [TestMethod]
        public void PumpGenericInquiryRequest_Serialize_TestMethod3()
        {
            PumpGenericInquiryRequest request = new PumpGenericInquiryRequest();
            request.SetMessageCallerSide(KaJiLianDongV11MessageTemplateBase.MessageCallerSide.Pump);
            request.SetMessageSequenceNumber(0);
            request.TargetAddress = 224;
            request.SourceAddress = 255;
            request.HANDLE = 0x30;
            var expected_raw_with_crc16 = "FA E0 FF 00 00 01 30 F0 03".ToBytes();
            //var p = new Parser();
            var result = p.Serialize(request);
            Assert.AreEqual(true, result.Length == expected_raw_with_crc16.Length);
            for (int i = 0; i < expected_raw_with_crc16.Length; i++)
            {
                Assert.AreEqual(true, expected_raw_with_crc16[i] == result[i], "actual: " + result.Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n) + System.Environment.NewLine + ", expecting: " + expected_raw_with_crc16.Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n)
                    );
            }
        }
        //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)


        /// <summary>
        /// only PC can init this msg and send to pump
        /// </summary>
        [TestMethod]
        public void PcGenericInquiryWithRichInfoResponse_Serialize_TestMethod1()
        {
            var request =
                new PcGenericInquiryWithRichInfoResponse();
            request.TargetAddress = 0xE0;
            request.SourceAddress = 0xFF;
            request.SetMessageSequenceNumber(1);
            request.SetPcTime(DateTime.Today);
            request.BL_VER = 500;
            request.ADD_BL_VER = 88;
            request.DEL_BL_VER = 77;
            request.WH_VER = 66;
            request.PRC_VER = 55;
            request.Sta_VER = 44;
            request.SELF_D_VER = PcGenericInquiryWithRichInfoResponse.更新标志.有新的程序or私有数据需要下载or发送;
            request.SOFT_FLAG = PcGenericInquiryWithRichInfoResponse.更新标志.有新的程序or私有数据需要下载or发送;

            string expectedSerializedBytes_without_crc16 = "E0FF 41 0016 30 " +
                                                           DateTime.Today.ToString("yyyyMMddHHmmss") +
                                                           " 01F4 58 4D 42 37 2C 01 01";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var dd =
                expectedSerializedBytes_without_crc16.ToBytes()
                    .Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            var crc16 = expectedSerializedBytes_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullExpectedRaw = packageHeader.Concat(expectedSerializedBytes_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            var result = p.Serialize(request);
            var debug = result.ToHexLogString();
            Assert.AreEqual(true, result.Length == fullExpectedRaw.Count(),
                "actual: " + debug + System.Environment.NewLine + ", expected: " +
                fullExpectedRaw.ToHexLogString());
            //Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageBase.MessageCallerSide.Pump);
        }

        /// <summary>
        /// only PC can init this msg and send to pump
        /// </summary>
        [TestMethod]
        public void PcGenericInquiryWithRichInfoResponse_Serialize_TestMethod2()
        {
            var request =
                new PcGenericInquiryWithRichInfoResponse();
            request.TargetAddress = 0xE2;
            request.SourceAddress = 0xFF;
            request.SetMessageSequenceNumber(24);
            request.SetPcTime(DateTime.Today);
            request.BL_VER = 99;
            request.ADD_BL_VER = 88;
            request.DEL_BL_VER = 77;
            request.WH_VER = 66;
            request.PRC_VER = 55;
            request.Sta_VER = 44;
            request.SELF_D_VER = PcGenericInquiryWithRichInfoResponse.更新标志.有新的程序or私有数据需要下载or发送;
            request.SOFT_FLAG = PcGenericInquiryWithRichInfoResponse.更新标志.无数据;

            string expectedSerializedBytes_without_crc16 = "E2FF 58 0016 30 " +
                                                           DateTime.Today.ToString("yyyyMMddHHmmss") +
                                                           " 0063 58 4D 42 37 2C 0F 00";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var dd =
                expectedSerializedBytes_without_crc16.ToBytes()
                    .Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            var crc16 = expectedSerializedBytes_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullExpectedRaw = packageHeader.Concat(expectedSerializedBytes_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            var result = p.Serialize(request);
            var debug = result.ToHexLogString();
            Assert.AreEqual(true, result.Length == fullExpectedRaw.Count(),
                "actual: " + debug + System.Environment.NewLine + ", expected: " +
                fullExpectedRaw.ToHexLogString());
            //Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageBase.MessageCallerSide.Pump);
        }

        /// <summary>
        /// only PC can init this msg and send to pump
        /// </summary>
        [TestMethod]
        public void PcGenericInquiryWithRichInfoResponse_Serialize_TestMethod3()
        {
            var request =
                new PcGenericInquiryWithRichInfoResponse();
            request.TargetAddress = 0xE2;
            request.SourceAddress = 0xFF;
            request.SetMessageSequenceNumber(24);
            request.SetPcTime(DateTime.Now);
            request.BL_VER = 99;
            request.ADD_BL_VER = 88;
            request.DEL_BL_VER = 77;
            request.WH_VER = 66;
            request.PRC_VER = 55;
            request.Sta_VER = 44;
            request.SELF_D_VER = PcGenericInquiryWithRichInfoResponse.更新标志.有新的程序or私有数据需要下载or发送;
            request.SOFT_FLAG = PcGenericInquiryWithRichInfoResponse.更新标志.无数据;

            string expectedSerializedBytes_without_crc16 = "E2FF 58 0016 30 " +
                                                           DateTime.Now.ToString("yyyyMMddHHmmss") +
                                                           " 0063 58 4D 42 37 2C 0F 00";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var dd =
                expectedSerializedBytes_without_crc16.ToBytes()
                    .Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            var crc16 = expectedSerializedBytes_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullExpectedRaw = packageHeader.Concat(expectedSerializedBytes_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            var result = p.Serialize(request);
            var debug = result.ToHexLogString();
            Assert.AreEqual(true, result.Length == fullExpectedRaw.Count(),
                "actual: " + debug + System.Environment.NewLine + ", expected: " +
                fullExpectedRaw.ToHexLogString());
            //Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageBase.MessageCallerSide.Pump);
        }

        /// <summary>
        /// only Pump can init this msg and send to PC.
        /// </summary>
        [TestMethod]
        public void PumpStateChangeRequest_Deserialize_TestMethod1()
        {
            //sub msg total len 15
            string raw_without_crc16 = "E2FF 18 0017 31 01     01 12 16 1234567890 4455 9922 965432";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var crc16 = raw_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullRaw = packageHeader.Concat(raw_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            // exclude the last 2 bytes CRC16
            var result = p.Deserialize(fullRaw.ToArray()) as PumpStateChangeRequest;
            var debug =
                (result as PumpStateChangeRequest).SubMessageRaw.Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            //var debug = result.ToHexLogString();
            Assert.AreEqual(true, result.HANDLE == 0x31);
            Assert.AreEqual(true, result.SourceAddress == 0xFF);
            Assert.AreEqual(true, result.TargetAddress == 0xE2);
            Assert.AreEqual(true, result.DataLength == 17);
            Assert.AreEqual(true, result.GetMessageSequenceNumber() == 24);
            Assert.AreEqual(true, result.SubMessageCount == 1);
            Assert.AreEqual(true, result.CRC16[0] == crc16[0] && result.CRC16[1] == crc16[1]);
            Assert.AreEqual(true, result.SubMessageRaw.Count == 15);
            Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageTemplateBase.MessageCallerSide.Pump);
        }

        [TestMethod]
        public void PumpStateChangeRequest_Deserialize_TestMethod2()
        {
            //                                                                          15                       11
            string raw_without_crc16 =
                "E2FF 18 0028 31 02     01 12 16 1234567890 4455 9922 965432         02 08 01 223344 556677 8907";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var crc16 = raw_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullRaw = packageHeader.Concat(raw_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            // exclude the last 2 bytes CRC16
            var result = p.Deserialize(fullRaw.ToArray()) as PumpStateChangeRequest;
            //(result as PumpStateChangeMainMessage).CRC16 = crc16.ToList();
            var debug =
                (result as PumpStateChangeRequest).SubMessageRaw.Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            Assert.AreEqual(true, result.HANDLE == 0x31);
            Assert.AreEqual(true, result.SourceAddress == 0xFF);
            Assert.AreEqual(true, result.TargetAddress == 0xE2);
            Assert.AreEqual(true, result.DataLength == 28);
            Assert.AreEqual(true, result.GetMessageSequenceNumber() == 24);
            Assert.AreEqual(true, result.SubMessageCount == 2);
            Assert.AreEqual(true, result.CRC16[0] == crc16[0] && result.CRC16[1] == crc16[1]);
            Assert.AreEqual(true, result.SubMessageRaw.Count == 26);
            Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageTemplateBase.MessageCallerSide.Pump);
        }

        [TestMethod]
        public void PumpStateChangeRequest_Deserialize_TestMethod3()
        {
            //                                            20                                          11                                     20                                                     20
            string raw_without_crc16 =
                "E2FF 18 0073 31 04     01 12 11 12345678901234567890 4455 99229654 32         02 08 01 223344 556677 8907        01 12 11 12345678901234567890 4455 9922 965432              01 12 11 12345678901234567890 4455 9922 965432  ";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var crc16 = raw_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullRaw = packageHeader.Concat(raw_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            // exclude the last 2 bytes CRC16
            var result = p.Deserialize(fullRaw.ToArray()) as PumpStateChangeRequest;
            //(result as PumpStateChangeMainMessage).CRC16 = crc16.ToList();
            var debug =
                (result as PumpStateChangeRequest).SubMessageRaw.Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            Assert.AreEqual(true, result.HANDLE == 0x31);
            Assert.AreEqual(true, result.SourceAddress == 0xFF);
            Assert.AreEqual(true, result.TargetAddress == 0xE2);
            Assert.AreEqual(true, result.DataLength == 73);
            Assert.AreEqual(true, result.GetMessageSequenceNumber() == 24);
            Assert.AreEqual(true, result.SubMessageCount == 4);
            Assert.AreEqual(true, result.CRC16[0] == crc16[0] && result.CRC16[1] == crc16[1]);
            Assert.AreEqual(true, result.SubMessageRaw.Count == 71);
            Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageTemplateBase.MessageCallerSide.Pump);
        }

        [TestMethod]
        public void PumpStateChangeRequest_Deserialize_TestMethod4()
        {
            //                                                             25                                      11                                     25                                                        25
            string raw_without_crc16 =
                "E2FF 18 0088 31 04     01 12 16 12345678901234567890 4455 99221133 965432112233         02 08 01 223344 556677 8907        01 12 16 12345678901234567890 4455 99221133 965432112233        01 12 16 12345678901234567890 4455 99221133 965432112233  ";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var crc16 = raw_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullRaw = packageHeader.Concat(raw_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            // exclude the last 2 bytes CRC16
            var result = p.Deserialize(fullRaw.ToArray()) as PumpStateChangeRequest;
            //(result as PumpStateChangeMainMessage).CRC16 = crc16.ToList();
            var debug =
                (result as PumpStateChangeRequest).SubMessageRaw.Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            Assert.AreEqual(true, result.HANDLE == 0x31);
            Assert.AreEqual(true, result.SourceAddress == 0xFF);
            Assert.AreEqual(true, result.TargetAddress == 0xE2);
            Assert.AreEqual(true, result.DataLength == 88);
            Assert.AreEqual(true, result.GetMessageSequenceNumber() == 24);
            Assert.AreEqual(true, result.SubMessageCount == 4);
            Assert.AreEqual(true, result.CRC16[0] == crc16[0] && result.CRC16[1] == crc16[1]);
            Assert.AreEqual(true, result.SubMessageRaw.Count == 86);
            Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageTemplateBase.MessageCallerSide.Pump);

            PumpStateChangeRequest.ParseSubMessages(result.SubMessageRaw, result.SubMessageCount, result);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages.Count == 3);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[0].St状态字 == 1);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[0].MZN枪号 == 18);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[0].LEN卡信息数据长度 == 22);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[0].ASN卡应用号 == "12345678901234567890");
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[0].CardSt卡状态 == "4455");
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[0].BAL余额 == Convert.ToInt32("99221133", 16),
                result.StateCardInsertedSubMessages[0].BAL余额.ToString());
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[0].IC_DATA卡片信息[5] == 0x33,
                result.StateCardInsertedSubMessages[0].IC_DATA卡片信息[0].ToString());
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[0].IC_DATA卡片信息[4] == 0x22);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[0].IC_DATA卡片信息[3] == 0x11);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[0].IC_DATA卡片信息[2] == 0x32);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[0].IC_DATA卡片信息[1] == 0x54);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[0].IC_DATA卡片信息[0] == 0x96);

            Assert.AreEqual(true, result.StateCardInsertedSubMessages[1].St状态字 == 1);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[1].MZN枪号 == 18);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[1].LEN卡信息数据长度 == 22);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[1].ASN卡应用号 == "12345678901234567890");
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[1].CardSt卡状态 == "4455");
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[1].BAL余额 == Convert.ToInt32("99221133", 16));
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[1].IC_DATA卡片信息[5] == 0x33);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[1].IC_DATA卡片信息[4] == 0x22);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[1].IC_DATA卡片信息[3] == 0x11);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[1].IC_DATA卡片信息[2] == 0x32);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[1].IC_DATA卡片信息[1] == 0x54);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[1].IC_DATA卡片信息[0] == 0x96);

            Assert.AreEqual(true, result.StateCardInsertedSubMessages[2].St状态字 == 1);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[2].MZN枪号 == 18);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[2].LEN卡信息数据长度 == 22);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[2].ASN卡应用号 == "12345678901234567890");
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[2].CardSt卡状态 == "4455");
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[2].BAL余额 == Convert.ToInt32("99221133", 16));
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[2].IC_DATA卡片信息[5] == 0x33);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[2].IC_DATA卡片信息[4] == 0x22);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[2].IC_DATA卡片信息[3] == 0x11);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[2].IC_DATA卡片信息[2] == 0x32);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[2].IC_DATA卡片信息[1] == 0x54);
            Assert.AreEqual(true, result.StateCardInsertedSubMessages[2].IC_DATA卡片信息[0] == 0x96);


            Assert.AreEqual(true, result.StateNozzleOperatingSubMessages.Count == 1);
            Assert.AreEqual(true, result.StateNozzleOperatingSubMessages[0].St状态字 == PumpStateChangeNozzleOperatingSubState.PumpStateChangeCode.抬枪或加油中);
            Assert.AreEqual(true, result.StateNozzleOperatingSubMessages[0].MZN枪号 == 8);
            Assert.AreEqual(true, result.StateNozzleOperatingSubMessages[0].P_UNIT结算单位_方式 == 1);
            Assert.AreEqual(true, result.StateNozzleOperatingSubMessages[0].AMN数额 == Convert.ToInt32("223344", 16));
            Assert.AreEqual(true, result.StateNozzleOperatingSubMessages[0].VOL升数 == Convert.ToInt32("556677", 16));
            Assert.AreEqual(true, result.StateNozzleOperatingSubMessages[0].PRC价格 == Convert.ToInt32("8907", 16));
        }

        [TestMethod]
        public void PumpAskDataDownloadRequest_Deserialize_TestMethod1()
        {
            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            string raw_without_crc16 = "E2FF 18 0004 33 04   1234 ";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var crc16 = raw_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullRaw = packageHeader.Concat(raw_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            // exclude the last 2 bytes CRC16
            var result = p.Deserialize(fullRaw.ToArray()) as PumpAskDataDownloadRequest;

            Assert.AreEqual(true, result.HANDLE == 0x33);
            Assert.AreEqual(true, result.SourceAddress == 0xFF);
            Assert.AreEqual(true, result.TargetAddress == 0xE2);
            Assert.AreEqual(true, result.DataLength == 4);
            Assert.AreEqual(true, result.GetMessageSequenceNumber() == 24);
            Assert.AreEqual(true, result.CRC16[0] == crc16[0] && result.CRC16[1] == crc16[1]);
            Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageTemplateBase.MessageCallerSide.Pump);
            Assert.AreEqual(true, result.Content_内容 == PumpAskDataDownloadRequest.PumpAskDataDownloadType.油品油价表);
            Assert.AreEqual(true, result.IBL_VER_基础黑名单版本 == 4660, result.IBL_VER_基础黑名单版本.ToString());
        }

        [TestMethod]
        public void PumpAskDataDownloadRequest_Deserialize_TestMethod2()
        {
            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            string raw_without_crc16 = "E2FF 18 0004 33 05   6688 ";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var crc16 = raw_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullRaw = packageHeader.Concat(raw_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            // exclude the last 2 bytes CRC16
            var result = p.Deserialize(fullRaw.ToArray()) as PumpAskDataDownloadRequest;

            Assert.AreEqual(true, result.HANDLE == 0x33);
            Assert.AreEqual(true, result.SourceAddress == 0xFF);
            Assert.AreEqual(true, result.TargetAddress == 0xE2);
            Assert.AreEqual(true, result.DataLength == 4);
            Assert.AreEqual(true, result.GetMessageSequenceNumber() == 24);
            Assert.AreEqual(true, result.CRC16[0] == crc16[0] && result.CRC16[1] == crc16[1]);
            Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageTemplateBase.MessageCallerSide.Pump);
            Assert.AreEqual(true, result.Content_内容 == PumpAskDataDownloadRequest.PumpAskDataDownloadType.油站通用信息);
            Assert.AreEqual(true, result.IBL_VER_基础黑名单版本 == 26248, result.IBL_VER_基础黑名单版本.ToString());
        }

        [TestMethod]
        public void PumpAskDataDownloadResponse_Serialize_TestMethod1()
        {
            var response = new PumpAskDataDownloadResponse();
            response.Content_内容 = PumpAskDataDownloadRequest.PumpAskDataDownloadType.油品油价表;
            response.BL_LEN_长度 = 0;
            response.SetMessageSequenceNumber(11);
            response.TargetAddress = 0xE2;
            response.SourceAddress = 0xFF;
            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            string expectedSerializedBytes_without_crc16 = "E2FF 4B 0006 33 00000000 04";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var dd =
                expectedSerializedBytes_without_crc16.ToBytes()
                    .Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            var crc16 = expectedSerializedBytes_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullExpectedRaw = packageHeader.Concat(expectedSerializedBytes_without_crc16.ToBytes().Concat(crc16));

            //var p = new Parser();
            var acutalBytes = p.Serialize(response);
            Assert.AreEqual(true, fullExpectedRaw.Count() == acutalBytes.Length, "length not match, actualyBytes: " + acutalBytes.ToHexLogString());
            for (int i = 0; i < fullExpectedRaw.Count(); i++)
            {
                Assert.AreEqual(true, fullExpectedRaw.ToList()[i] == acutalBytes[i], "byte[" + i.ToString() + "] not match, acutal Bytes: " + acutalBytes.ToHexLogString());
            }
        }

        [TestMethod]
        public void PumpAskDataDownloadResponse_Serialize_TestMethod2()
        {
            var response = new PumpAskDataDownloadResponse();
            response.Content_内容 = PumpAskDataDownloadRequest.PumpAskDataDownloadType.油站通用信息;
            response.BL_LEN_长度 = 65535;
            response.SetMessageSequenceNumber(34);
            response.TargetAddress = 0xE2;
            response.SourceAddress = 0xFF;
            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            string expectedSerializedBytes_without_crc16 = "E2FF 62 0006 33 0000FFFF 05";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var dd =
                expectedSerializedBytes_without_crc16.ToBytes()
                    .Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            var crc16 = expectedSerializedBytes_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullExpectedRaw = packageHeader.Concat(expectedSerializedBytes_without_crc16.ToBytes().Concat(crc16));

            //var p = new Parser();
            var acutalBytes = p.Serialize(response);
            Assert.AreEqual(true, fullExpectedRaw.Count() == acutalBytes.Length, "length not match, actualyBytes: " + acutalBytes.ToHexLogString());
            for (int i = 0; i < fullExpectedRaw.Count(); i++)
            {
                Assert.AreEqual(true, fullExpectedRaw.ToList()[i] == acutalBytes[i], "byte[" + i.ToString() + "] not match, acutal Bytes: " + acutalBytes.ToHexLogString());
            }
        }

        [TestMethod]
        public void PumpDataDownloadRequest_Deserialize_TestMethod1()
        {
            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            string raw_without_crc16 = "E2FF 18 0005 34 04   1234 24";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var crc16 = raw_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullRaw = packageHeader.Concat(raw_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            // exclude the last 2 bytes CRC16
            var result = p.Deserialize(fullRaw.ToArray()) as PumpDataDownloadRequest;

            Assert.AreEqual(true, result.HANDLE == 0x34);
            Assert.AreEqual(true, result.SourceAddress == 0xFF);
            Assert.AreEqual(true, result.TargetAddress == 0xE2);
            Assert.AreEqual(true, result.DataLength == 5);
            Assert.AreEqual(true, result.GetMessageSequenceNumber() == 24);
            Assert.AreEqual(true, result.CRC16[0] == crc16[0] && result.CRC16[1] == crc16[1]);
            Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageTemplateBase.MessageCallerSide.Pump);
            Assert.AreEqual(true, result.Content_内容 == PumpAskDataDownloadRequest.PumpAskDataDownloadType.油品油价表);
            Assert.AreEqual(true, result.S_OFFSET_段偏移 == 4660);
            Assert.AreEqual(true, result.SEG_段数_segs == 36);
        }

        [TestMethod]
        public void PumpDataDownloadRequest_Deserialize_TestMethod2()
        {
            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            string raw_without_crc16 = "E2FF 1A 0005 34 05   4321 77";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var crc16 = raw_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullRaw = packageHeader.Concat(raw_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            // exclude the last 2 bytes CRC16
            var result = p.Deserialize(fullRaw.ToArray()) as PumpDataDownloadRequest;

            Assert.AreEqual(true, result.HANDLE == 0x34);
            Assert.AreEqual(true, result.SourceAddress == 0xFF);
            Assert.AreEqual(true, result.TargetAddress == 0xE2);
            Assert.AreEqual(true, result.DataLength == 5);
            Assert.AreEqual(true, result.GetMessageSequenceNumber() == 26);
            Assert.AreEqual(true, result.CRC16[0] == crc16[0] && result.CRC16[1] == crc16[1]);
            Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageTemplateBase.MessageCallerSide.Pump);
            Assert.AreEqual(true, result.Content_内容 == PumpAskDataDownloadRequest.PumpAskDataDownloadType.油站通用信息);
            Assert.AreEqual(true, result.S_OFFSET_段偏移 == 17185);
            Assert.AreEqual(true, result.SEG_段数_segs == 119);


            // ==============repeat for test Parser share instance case==============
            raw_without_crc16 = "E2FF 1A 0005 34 05   4321 77";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            crc16 = raw_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            fullRaw = packageHeader.Concat(raw_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            // exclude the last 2 bytes CRC16
            result = p.Deserialize(fullRaw.ToArray()) as PumpDataDownloadRequest;

            Assert.AreEqual(true, result.HANDLE == 0x34);
            Assert.AreEqual(true, result.SourceAddress == 0xFF);
            Assert.AreEqual(true, result.TargetAddress == 0xE2);
            Assert.AreEqual(true, result.DataLength == 5);
            Assert.AreEqual(true, result.GetMessageSequenceNumber() == 26);
            Assert.AreEqual(true, result.CRC16[0] == crc16[0] && result.CRC16[1] == crc16[1]);
            Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageTemplateBase.MessageCallerSide.Pump);
            Assert.AreEqual(true, result.Content_内容 == PumpAskDataDownloadRequest.PumpAskDataDownloadType.油站通用信息);
            Assert.AreEqual(true, result.S_OFFSET_段偏移 == 17185);
            Assert.AreEqual(true, result.SEG_段数_segs == 119);

            // ==============repeat for test Parser share instance case==============
            raw_without_crc16 = "E2FF 1A 0005 34 05   4321 77";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            crc16 = raw_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            fullRaw = packageHeader.Concat(raw_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            // exclude the last 2 bytes CRC16
            result = p.Deserialize(fullRaw.ToArray()) as PumpDataDownloadRequest;

            Assert.AreEqual(true, result.HANDLE == 0x34);
            Assert.AreEqual(true, result.SourceAddress == 0xFF);
            Assert.AreEqual(true, result.TargetAddress == 0xE2);
            Assert.AreEqual(true, result.DataLength == 5);
            Assert.AreEqual(true, result.GetMessageSequenceNumber() == 26);
            Assert.AreEqual(true, result.CRC16[0] == crc16[0] && result.CRC16[1] == crc16[1]);
            Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageTemplateBase.MessageCallerSide.Pump);
            Assert.AreEqual(true, result.Content_内容 == PumpAskDataDownloadRequest.PumpAskDataDownloadType.油站通用信息);
            Assert.AreEqual(true, result.S_OFFSET_段偏移 == 17185);
            Assert.AreEqual(true, result.SEG_段数_segs == 119);

            // ==============repeat for test Parser share instance case==============
            raw_without_crc16 = "E2FF 1A 0005 34 05   4321 77";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            crc16 = raw_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            fullRaw = packageHeader.Concat(raw_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            // exclude the last 2 bytes CRC16
            result = p.Deserialize(fullRaw.ToArray()) as PumpDataDownloadRequest;

            Assert.AreEqual(true, result.HANDLE == 0x34);
            Assert.AreEqual(true, result.SourceAddress == 0xFF);
            Assert.AreEqual(true, result.TargetAddress == 0xE2);
            Assert.AreEqual(true, result.DataLength == 5);
            Assert.AreEqual(true, result.GetMessageSequenceNumber() == 26);
            Assert.AreEqual(true, result.CRC16[0] == crc16[0] && result.CRC16[1] == crc16[1]);
            Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageTemplateBase.MessageCallerSide.Pump);
            Assert.AreEqual(true, result.Content_内容 == PumpAskDataDownloadRequest.PumpAskDataDownloadType.油站通用信息);
            Assert.AreEqual(true, result.S_OFFSET_段偏移 == 17185);
            Assert.AreEqual(true, result.SEG_段数_segs == 119);
        }

        [TestMethod]
        public void PumpDataDownloadResponse_Serialize_TestMethod1()
        {
            var response = new PumpDataDownloadResponse();
            response.Content_内容 = PumpAskDataDownloadRequest.PumpAskDataDownloadType.油品油价表;
            response.S_OFFSET_段偏移 = 65533;
            response.SEG_段数_segs = 99;
            response.DATA_Content_数据内容 = new List<byte>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            response.SetMessageSequenceNumber(11);
            response.TargetAddress = 0xE2;
            response.SourceAddress = 0xFF;
            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            string expectedSerializedBytes_without_crc16 = "E2FF 4B 0017 34 04 FFFD 63 0102030405060708090A0B0C";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var dd =
                expectedSerializedBytes_without_crc16.ToBytes()
                    .Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            var crc16 = expectedSerializedBytes_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullExpectedRaw = packageHeader.Concat(expectedSerializedBytes_without_crc16.ToBytes().Concat(crc16));

            //var p = new Parser();
            var acutalBytes = p.Serialize(response);
            Assert.AreEqual(true, fullExpectedRaw.Count() == acutalBytes.Length, "length not match, actualyBytes: " + acutalBytes.ToHexLogString());
            for (int i = 0; i < fullExpectedRaw.Count(); i++)
            {
                Assert.AreEqual(true, fullExpectedRaw.ToList()[i] == acutalBytes[i], "byte[" + i.ToString() + "] not match, acutal Bytes: " + acutalBytes.ToHexLogString());
            }
        }

        [TestMethod]
        public void PumpDataDownloadResponse_Serialize_TestMethod2()
        {
            var response = new PumpDataDownloadResponse();
            response.Content_内容 = PumpAskDataDownloadRequest.PumpAskDataDownloadType.油站通用信息;
            response.S_OFFSET_段偏移 = 7543;
            response.SEG_段数_segs = 234;
            response.DATA_Content_数据内容 = new List<byte>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            response.SetMessageSequenceNumber(11);
            response.TargetAddress = 0xE2;
            response.SourceAddress = 0xFF;
            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            string expectedSerializedBytes_without_crc16 = "E2FF 4B 0041 34 05 1D77 EA 0102030405060708090A0B0C0102030405060708090A0B0C0102030405060708090A0B0C";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var dd =
                expectedSerializedBytes_without_crc16.ToBytes()
                    .Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            var crc16 = expectedSerializedBytes_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullExpectedRaw = packageHeader.Concat(expectedSerializedBytes_without_crc16.ToBytes().Concat(crc16));

            //var p = new Parser();
            var acutalBytes = p.Serialize(response);
            Assert.AreEqual(true, fullExpectedRaw.Count() == acutalBytes.Length, "length not match, actualyBytes: " + acutalBytes.ToHexLogString());
            for (int i = 0; i < fullExpectedRaw.Count(); i++)
            {
                Assert.AreEqual(true, fullExpectedRaw.ToList()[i] == acutalBytes[i], "byte[" + i.ToString() + "] not match, acutal Bytes: " + acutalBytes.ToHexLogString());
            }

            // ==============repeat for test Parser share instance case==============
            response = new PumpDataDownloadResponse();
            response.Content_内容 = PumpAskDataDownloadRequest.PumpAskDataDownloadType.油站通用信息;
            response.S_OFFSET_段偏移 = 7543;
            response.SEG_段数_segs = 3;
            response.DATA_Content_数据内容 = new List<byte>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            response.SetMessageSequenceNumber(11);
            response.TargetAddress = 0xE2;
            response.SourceAddress = 0xFF;
            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            expectedSerializedBytes_without_crc16 = "E2FF 4B 0053 34 05 1D77 03 0102030405060708090A0B0C0102030405060708090A0B0C0102030405060708090A0B0C0102030405060708090A0B0C";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            dd =
                expectedSerializedBytes_without_crc16.ToBytes()
                    .Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            crc16 = expectedSerializedBytes_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            fullExpectedRaw = packageHeader.Concat(expectedSerializedBytes_without_crc16.ToBytes().Concat(crc16));

            //var p = new Parser();
            acutalBytes = p.Serialize(response);
            Assert.AreEqual(true, fullExpectedRaw.Count() == acutalBytes.Length, "length not match, actualyBytes: " + acutalBytes.ToHexLogString());
            for (int i = 0; i < fullExpectedRaw.Count(); i++)
            {
                Assert.AreEqual(true, fullExpectedRaw.ToList()[i] == acutalBytes[i], "byte[" + i.ToString() + "] not match, acutal Bytes: " + acutalBytes.ToHexLogString());
            }



            // ==============repeat for test Parser share instance case==============
            response = new PumpDataDownloadResponse();
            response.Content_内容 = PumpAskDataDownloadRequest.PumpAskDataDownloadType.油站通用信息;
            response.S_OFFSET_段偏移 = 7543;
            response.SEG_段数_segs = 3;
            response.DATA_Content_数据内容 = new List<byte>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            response.SetMessageSequenceNumber(11);
            response.TargetAddress = 0xE2;
            response.SourceAddress = 0xFF;
            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            expectedSerializedBytes_without_crc16 = "E2FF 4B 0053 34 05 1D77 03 0102030405060708090A0B0C0102030405060708090A0B0C0102030405060708090A0B0C0102030405060708090A0B0C";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            dd =
                expectedSerializedBytes_without_crc16.ToBytes()
                    .Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            crc16 = expectedSerializedBytes_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            fullExpectedRaw = packageHeader.Concat(expectedSerializedBytes_without_crc16.ToBytes().Concat(crc16));

            //var p = new Parser();
            acutalBytes = p.Serialize(response);
            Assert.AreEqual(true, fullExpectedRaw.Count() == acutalBytes.Length, "length not match, actualyBytes: " + acutalBytes.ToHexLogString());
            for (int i = 0; i < fullExpectedRaw.Count(); i++)
            {
                Assert.AreEqual(true, fullExpectedRaw.ToList()[i] == acutalBytes[i], "byte[" + i.ToString() + "] not match, acutal Bytes: " + acutalBytes.ToHexLogString());
            }



            // ==============repeat for test Parser share instance case==============
            response = new PumpDataDownloadResponse();
            response.Content_内容 = PumpAskDataDownloadRequest.PumpAskDataDownloadType.油站通用信息;
            response.S_OFFSET_段偏移 = 7543;
            response.SEG_段数_segs = 3;
            response.DATA_Content_数据内容 = new List<byte>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            response.SetMessageSequenceNumber(11);
            response.TargetAddress = 0xE2;
            response.SourceAddress = 0xFF;
            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            expectedSerializedBytes_without_crc16 = "E2FF 4B 0053 34 05 1D77 03 0102030405060708090A0B0C0102030405060708090A0B0C0102030405060708090A0B0C0102030405060708090A0B0C";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            dd =
                expectedSerializedBytes_without_crc16.ToBytes()
                    .Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            crc16 = expectedSerializedBytes_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            fullExpectedRaw = packageHeader.Concat(expectedSerializedBytes_without_crc16.ToBytes().Concat(crc16));

            //var p = new Parser();
            acutalBytes = p.Serialize(response);
            Assert.AreEqual(true, fullExpectedRaw.Count() == acutalBytes.Length, "length not match, actualyBytes: " + acutalBytes.ToHexLogString());
            for (int i = 0; i < fullExpectedRaw.Count(); i++)
            {
                Assert.AreEqual(true, fullExpectedRaw.ToList()[i] == acutalBytes[i], "byte[" + i.ToString() + "] not match, acutal Bytes: " + acutalBytes.ToHexLogString());
            }
        }

        [TestMethod]
        public void PumpDataDownloadResponse_Serialize_TestMethod3()
        {
            var response = new PumpDataDownloadResponse();
            response.Content_内容 = PumpAskDataDownloadRequest.PumpAskDataDownloadType.油站通用信息;
            response.S_OFFSET_段偏移 = 7543;
            response.SEG_段数_segs = 3;
            response.DATA_Content_数据内容 = new List<byte>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            response.SetMessageSequenceNumber(11);
            response.TargetAddress = 0xE2;
            response.SourceAddress = 0xFF;
            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            string expectedSerializedBytes_without_crc16 = "E2FF 4B 0053 34 05 1D77 03 0102030405060708090A0B0C0102030405060708090A0B0C0102030405060708090A0B0C0102030405060708090A0B0C";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var dd =
                expectedSerializedBytes_without_crc16.ToBytes()
                    .Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            var crc16 = expectedSerializedBytes_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullExpectedRaw = packageHeader.Concat(expectedSerializedBytes_without_crc16.ToBytes().Concat(crc16));

            //var p = new Parser();
            var acutalBytes = p.Serialize(response);
            Assert.AreEqual(true, fullExpectedRaw.Count() == acutalBytes.Length, "length not match, actualyBytes: " + acutalBytes.ToHexLogString());
            for (int i = 0; i < fullExpectedRaw.Count(); i++)
            {
                Assert.AreEqual(true, fullExpectedRaw.ToList()[i] == acutalBytes[i], "byte[" + i.ToString() + "] not match, acutal Bytes: " + acutalBytes.ToHexLogString());
            }
        }

        [TestMethod]
        public void PumpNotifyTransactionDoneRequest_Deserialize_TestMethod1()
        {
            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)
            //                                          POS-TTC   T-TYPE                                                   ASN卡应用号
            string raw_without_crc16 = "E2FF 1A 0096 32 12345678    00   " + DateTime.Today.ToString("yyyyMMddHHmmss") + " 98765432109876543210"
            //     BAL余额     AMN数额    CTC    TAC    GMAC     PSAM-TAC   PSAM-ASN              PSAM-TID        PSAM-TTC   DS
                + "56778890    012233    9587 44448888 55551111 66662222  01234567890123456789  123456123456     22333311    00 " +
            // UNIT    c-type  VER   NZN   G-CODE   vol      prc    EMP    V-TOT     RFU                    T-MAC
                "00     00     12     09   0102    010203    0202    09   12348899 0000000000000000000000  12345678";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var crc16 = raw_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullRaw = packageHeader.Concat(raw_without_crc16.ToBytes().Concat(crc16));
            //var p = new Parser();
            // exclude the last 2 bytes CRC16
            var result = p.Deserialize(fullRaw.ToArray()) as PumpNotifyTransactionDoneRequest;

            Assert.AreEqual(true, result.HANDLE == 0x32);
            Assert.AreEqual(true, result.SourceAddress == 0xFF);
            Assert.AreEqual(true, result.TargetAddress == 0xE2);
            Assert.AreEqual(true, result.DataLength == 96);
            Assert.AreEqual(true, result.GetMessageSequenceNumber() == 26);
            Assert.AreEqual(true, result.CRC16[0] == crc16[0] && result.CRC16[1] == crc16[1]);
            Assert.AreEqual(true, result.GetMessageCallerSide() == KaJiLianDongV11MessageTemplateBase.MessageCallerSide.Pump);
            Assert.AreEqual(true, result.POS_TTC == 305419896);
            Assert.AreEqual(true, result.T_Type == 0);
            Assert.AreEqual(true, result.GetTime() == DateTime.Today);
            Assert.AreEqual(true, result.ASN卡应用号 == "98765432109876543210");
            Assert.AreEqual(true, result.BAL余额 == 1450674320);
            Assert.AreEqual(true, result.AMN数额 == 74291);
            Assert.AreEqual(true, result.CTC == 38279);
            Assert.AreEqual(true, result.TAC电子签名 == 0x44448888);
            Assert.AreEqual(true, result.GMAC电子签名 == 0x55551111);
            Assert.AreEqual(true, result.PSAM_TAC灰锁签名 == 0x66662222);
            Assert.AreEqual(true, result.PSAM_ASN_PSAM应用号 == "01234567890123456789");
            Assert.AreEqual(true, result.PSAM_TID_PSAM编号.Count == 6);
            Assert.AreEqual(true, result.PSAM_TID_PSAM编号[0] == 0x12);
            Assert.AreEqual(true, result.PSAM_TID_PSAM编号[1] == 0x34);
            Assert.AreEqual(true, result.PSAM_TID_PSAM编号[2] == 0x56);
            Assert.AreEqual(true, result.PSAM_TID_PSAM编号[3] == 0x12);
            Assert.AreEqual(true, result.PSAM_TID_PSAM编号[4] == 0x34);
            Assert.AreEqual(true, result.PSAM_TID_PSAM编号[5] == 0x56);
            Assert.AreEqual(true, result.PSAM_TTC == 0x22333311);
            Assert.AreEqual(true, result.DS_扣款来源 == 0);
            Assert.AreEqual(true, result.UNIT_结算单位_方式 == 00);
            Assert.AreEqual(true, result.C_TYPE_卡类 == 0);
            Assert.AreEqual(true, result.VER_版本 == 0X12);
            Assert.AreEqual(true, result.NZN_枪号 == 9);
            Assert.AreEqual(true, result.G_CODE_油品代码 == "0102");
            Assert.AreEqual(true, result.VOL_升数 == 0x010203);
            Assert.AreEqual(true, result.PRC_成交价格 == 0x0202);
            Assert.AreEqual(true, result.EMP_员工号 == 0x09);
            Assert.AreEqual(true, result.V_TOT_升累计 == 0x12348899);
            Assert.AreEqual(true, result.RFU_备用.All(ppp => ppp == 0));
            Assert.AreEqual(true, result.RFU_备用.Count == 11);
            Assert.AreEqual(true, result.T_MAC_终端数据认证码 == 0x12345678);
        }

        [TestMethod]
        public void FuelPriceRecord_Serialize_TestMethod1()
        {
            var msg = new FuelPriceList(0x99);
            msg.Set_V_D_andT_新油品油价生效时间(DateTime.Now);
            msg.FieldNum_记录数 = 2;
            msg.当前油品油价记录List = new List<FuelPriceRecord>()
            {
                new FuelPriceRecord(){NZN_枪号 = 1,O_Type_油品代码 = "0101",Den_密度 = 0x12131415,PRC_n_价格数目 = 4,
                    PrcList =new List<Price>(){ new Price(0x55), new Price(0x66), new Price(0x77), new Price(0x88)}},
                new FuelPriceRecord()
                {
                    NZN_枪号 = 2,
                    O_Type_油品代码 = "0102",
                    Den_密度 = 0x12131415,
                    PRC_n_价格数目 = 4,
                    PrcList = new List<Price>()
                    {
                       new Price(0x55),
                       new Price(0x66),
                       new Price(0x77),
                       new Price(0x99),
                    }
                }
            };
            msg.新油品油价记录List = new List<FuelPriceRecord>()
            {
                new FuelPriceRecord(){NZN_枪号 = 1,O_Type_油品代码 = "0101",Den_密度 = 0x12131415,PRC_n_价格数目 = 4,
                    PrcList = new List<Price>(){ new Price(0x11), new Price(0x22), new Price(0x33), new Price(0x44)}},
                new FuelPriceRecord()
                {
                    NZN_枪号 = 2,
                    O_Type_油品代码 = "0102",
                    Den_密度 = 0x12131415,
                    PRC_n_价格数目 = 4,
                    PrcList = new List<Price>()
                    {
                       new Price(0x11),
                       new Price(0x22),
                       new Price(0x33),
                       new Price(0x43),
                    }
                }
            };
            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            string expectedSerializedBytes_without_crc16 = "99 " + DateTime.Now.ToString("yyyyMMddHHmm") + "02      01 0101 12131415 04 0055 0066 0077 0088      02 0102 12131415 04 0055 0066 0077 0099  " +
                                                           "   01 0101 12131415 04 0011 0022 0033 0044      02 0102 12131415 04 0011 0022 0033 0043  ";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var dd =
                expectedSerializedBytes_without_crc16.ToBytes()
                    .Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            var crc16 = expectedSerializedBytes_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullExpectedRaw = expectedSerializedBytes_without_crc16.ToBytes();

            //var p = new Parser();
            var acutalBytes = p.Serialize(msg);
            Assert.AreEqual(true, fullExpectedRaw.Count() == acutalBytes.Length, "length not match, actualyBytes: " + acutalBytes.ToHexLogString());
            for (int i = 0; i < fullExpectedRaw.Count(); i++)
            {
                Assert.AreEqual(true, fullExpectedRaw.ToList()[i] == acutalBytes[i], "byte[" + i.ToString() + "] not match, acutal Bytes: " + acutalBytes.ToHexLogString());
            }
        }

        [TestMethod]
        public void FuelPriceRecord_Serialize_TestMethod2()
        {
            var msg = new FuelPriceList(0x03);
            msg.Set_V_D_andT_新油品油价生效时间(DateTime.Now);
            msg.FieldNum_记录数 = 1;
            msg.当前油品油价记录List = new List<FuelPriceRecord>()
            {
                new FuelPriceRecord(){NZN_枪号 = 0x12,O_Type_油品代码 = "1031",Den_密度 = 0x12131415,PRC_n_价格数目 = 2,
                    PrcList =new List<Price>(){ new Price(0x55), new Price(0x66),}},
            };
            msg.新油品油价记录List = new List<FuelPriceRecord>()
            {
                new FuelPriceRecord(){NZN_枪号 =0x14,O_Type_油品代码 = "1041",Den_密度 = 0x12131415,PRC_n_价格数目 = 3,
                    PrcList = new List<Price>(){  new Price(0x11), new Price(0x22), new Price(0x33)}},
            };
            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            string expectedSerializedBytes_without_crc16 = "03 " + DateTime.Now.ToString("yyyyMMddHHmm") + "01      12 1031 12131415 02 0055 0066   " +
                                                           "   14 1041 12131415 03 0011 0022 0033    ";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var dd =
                expectedSerializedBytes_without_crc16.ToBytes()
                    .Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            var crc16 = expectedSerializedBytes_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullExpectedRaw = expectedSerializedBytes_without_crc16.ToBytes();

            //var p = new Parser();
            var acutalBytes = p.Serialize(msg);
            Assert.AreEqual(true, fullExpectedRaw.Count() == acutalBytes.Length, "length not match, actualyBytes: " + acutalBytes.ToHexLogString());
            for (int i = 0; i < fullExpectedRaw.Count(); i++)
            {
                Assert.AreEqual(true, fullExpectedRaw.ToList()[i] == acutalBytes[i], "byte[" + i.ToString() + "] not match, acutal Bytes: " + acutalBytes.ToHexLogString());
            }
        }

        [TestMethod]
        public void StationInfo_Serialize_TestMethod1()
        {
            var result = new PumpStationInfo();
            result.Ver = 0xDF;
            result.City_地市代码 = 11;
            result.Prov_省代号 = 11;
            result.Superior_上级单位代号 = "11111111";
            result.S_ID_加油站ID = "11111111";
            result.POS_P_通讯终端逻辑编号 = 0;
#warning where this configuration info come from?
            //result.GUN_N_油枪数 = 4;
#warning where this configuration info number come from?
            result.MZN_单个油枪 = new List<byte>() { 1, 2, 3, 4 };

            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            string expectedSerializedBytes_without_crc16 = "DF 11 11 11111111 11111111 00 04 01020304";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var dd =
                expectedSerializedBytes_without_crc16.ToBytes()
                    .Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            var crc16 = expectedSerializedBytes_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullExpectedRaw = expectedSerializedBytes_without_crc16.ToBytes();

            //var p = new Parser();
            var acutalBytes = p.Serialize(result);
            Assert.AreEqual(true, fullExpectedRaw.Count() == acutalBytes.Length, "length not match, actualyBytes: " + acutalBytes.ToHexLogString());
            for (int i = 0; i < fullExpectedRaw.Count(); i++)
            {
                Assert.AreEqual(true, fullExpectedRaw.ToList()[i] == acutalBytes[i], "byte[" + i.ToString() + "] not match, acutal Bytes: " + acutalBytes.ToHexLogString());
            }
        }

        [TestMethod]
        public void WhiteList_Serialize_TestMethod1()
        {
            var result = new WhiteList(0x8E, DateTime.Now.Subtract(new TimeSpan(366, 1, 1, 1)), DateTime.Now.AddYears(1), BlackAndWhiteListBase.BlackListEffectiveAreaEnum.本省黑名单, 11, 1);
            //result.名单数量 = 1;
            result.CardSerialNumbers = new List<CardSerialNumber>();
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000001", });


            //数据包头(FA) 目标地址(1Byte)  源地址(1Byte) 帧号/控制(1Byte) 有效数据长度(2Byte BCD) 有效数据(n) 数据校验(2Byte)

            string expectedSerializedBytes_without_crc16 = "008E " + DateTime.Now.Subtract(new TimeSpan(366, 1, 1, 1)).ToString("yyyyMMdd")
                + DateTime.Now.AddYears(1).ToString("yyyyMMdd") + "11FF 00000001 01000411100100000001";
            //FA E0 FF 41 00 16 30 20 17 02 23 00 00 00 01 F4 58 4D 42 37 2C 01 01
            var dd =
                expectedSerializedBytes_without_crc16.ToBytes()
                    .Select(s => s.ToString("X").PadLeft(2, '0'))
                    .Aggregate((acc, n) => acc + " " + n);
            var crc16 = expectedSerializedBytes_without_crc16.ToBytes().ComputeChecksumBytesCrc16();
            var fullExpectedRaw = expectedSerializedBytes_without_crc16.ToBytes();

            //var p = new Parser();
            var acutalBytes = p.Serialize(result);
            Assert.AreEqual(true, fullExpectedRaw.Count() == acutalBytes.Length, "length not match, actualyBytes: " + acutalBytes.ToHexLogString() + System.Environment.NewLine + ", expecting: " + expectedSerializedBytes_without_crc16);
            for (int i = 0; i < fullExpectedRaw.Count(); i++)
            {
                Assert.AreEqual(true, fullExpectedRaw.ToList()[i] == acutalBytes[i], "byte[" + i.ToString() + "] not match, acutal Bytes: " + acutalBytes.ToHexLogString() + System.Environment.NewLine + ", expecting: " + expectedSerializedBytes_without_crc16);
            }
        }

        [TestMethod]
        public void CRCTest1()
        {
            var _ = "00 00 44 00 06 33 00 00 00 58 04".ToBytes();
            var crc = _.ComputeChecksumBytesCrc16();
            Assert.AreEqual(0xC3, crc[0], "actual: " + crc.ToHexLogString());
            Assert.AreEqual(0x5A, crc[1], "actual: " + crc.ToHexLogString());
        }

        [TestMethod]
        public void CRCTest2()
        {
            var _ = "00 00 47 02 45 340300 3D0F01 00 04 11 10 01 00 00 01 13 01 00 04 11 10 01 00 00 01 14 01 00 04 11 10 01 00 00 01 15 01 00 04 11 10 01 00 00 01 16 01 00 04 11 10 01 00 00 01 17 01 00 04 11 10 01 00 00 01 18 01 00 04 11 10 01 00 00 01 19 01 00 04 11 10 01 00 00 01 20 01 00 04 11 10 01 00 00 01 21 01 00 04 11 10 01 00 00 01 22 01 00 04 11 10 01 00 00 01 23 01 00 04 11 10 01 00 00 01 24 01 00 04 11 10 01 00 00 01 25 01 00 04 11 10 01 00 00 01 26 01 00 04 11 10 01 00 00 01 27 01 00 04 11 10 01 00 00 01 28 01 00 04 11 10 01 00 00 01 29 01 00 04 11 10 01 00 00 01 30 01 00 04 11 10 01 00 00 01 31 01 00 04 11 10 01 00 00 01 32 01 00 04 11 10 01 00 00 01 33 01 00 04 11 10 01 00 00 01 34 01 00 04 11 10 01 00 00 01 35 01 00 04 11 10 01 00 00 01 36 ".ToBytes();
            var crc = _.ComputeChecksumBytesCrc16();
            Assert.AreEqual(0xC3, crc[0], "actual: " + crc.ToHexLogString());
            Assert.AreEqual(0xD1, crc[1], "actual: " + crc.ToHexLogString());
        }

        [TestMethod]
        public void ToHexStringTest1()
        {
            string raw =
                "E2 FF 18 00 28 31 02 01 12 16 12 34 56 78 90 44 55 99 22";
            var bytes = raw.ToBytes();
            var back = bytes.ToHexLogString();
            Assert.AreEqual(true, back == raw);
        }
    }
}
