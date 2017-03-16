using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    /// <summary>
    /// 加油机对 PC 机普通查询命令 30H
    /// PC 机对加油机普通查询命令 30H
    /// 加油机发送实时信息命令 31H
    /// </summary>
    public class PumpStateChangeCardInsertedSubState : MessageTemplateBase
    {
        //public enum GenericInquiryRequestType
        //{
        //    //加油机对PC机普通查询命令30H = 0x30,
        //    //PC机对加油机普通查询命令30H = 0x30,
        //    加油机发送实时信息命令31H = 0x31
        //}

        //public RealTimeInquiryRequest(GenericInquiryRequestType type)
        //{
        //    base.HANDLE = (byte)type;
        //}

        [Format(1, EncodingType.BIN, 0)]
        public byte St状态字 { get; set; }

        [Format(1, EncodingType.BIN, 1)]
        public byte MZN枪号 { get; set; }

        [Format(1, EncodingType.BIN, 2)]
        public byte LEN卡信息数据长度 { get; set; }

        [Format(10, EncodingType.BcdString, 3)]
        public string ASN卡应用号 { get; set; }

        [Format(2, EncodingType.BcdString, 4)]
        public string CardSt卡状态 { get; set; }

        [Format(4, EncodingType.BIN, 5)]
        public int BAL余额 { get; set; }

        [EnumerableFormat("LEN卡信息数据长度", "-16", 6, EncodingType = EncodingType.BIN)]
        public List<byte> IC_DATA卡片信息 { get; set; }

    }
}