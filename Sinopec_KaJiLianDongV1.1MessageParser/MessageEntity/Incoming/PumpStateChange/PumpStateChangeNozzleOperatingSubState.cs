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
    public class PumpStateChangeNozzleOperatingSubState : MessageTemplateBase
    {
        public enum PumpStateChangeCode
        {
            卡插入 = 1,
            抬枪或加油中 = 2
        }

        //public RealTimeInquiryRequest(GenericInquiryRequestType type)
        //{
        //    base.HANDLE = (byte)type;
        //}

        [Format(1, EncodingType.BIN, 0)]
        public PumpStateChangeCode St状态字 { get; set; }

        [Format(1, EncodingType.BIN, 1)]
        public byte MZN枪号 { get; set; }

        [Format(1, EncodingType.BIN, 2)]
        public byte P_UNIT结算单位_方式 { get; set; }

        [Format(3, EncodingType.BIN, 3)]
        public int AMN数额 { get; set; }

        [Format(3, EncodingType.BIN, 4)]
        public int VOL升数 { get; set; }

        [Format(2, EncodingType.BIN, 5)]
        public int PRC价格 { get; set; }

    }
}