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
    /// </summary>
    public class PcGenericInquiryWithRichInfoResponse : KaJiLianDongV11MessageTemplateBase
    {
        public enum RequestType
        {
            //加油机对PC机普通查询命令30H = 0x30,
            PC机对加油机普通查询命令30H = 0x30,
            //加油机发送实时信息命令31H = 0x31
        }

        public PcGenericInquiryWithRichInfoResponse()
        {
            base.HANDLE = 0x30;
            base.SetMessageCallerSide(MessageCallerSide.Host);
        }

        [Format(7, EncodingType.BcdString, 1)]
        public long PC_TIME { get; private set; }

        public DateTime GetPcTime()
        {
            var dd = this.PC_TIME.ToString();
            var dt = Convert.ToDateTime(dd);
            return dt;
        }

        public void SetPcTime(DateTime dateTime)
        {
            var str = dateTime.ToString("yyyyMMddHHmmss");
            this.PC_TIME = long.Parse(str);
        }

        [Format(2, EncodingType.BIN, 2)]
        public int BL_VER { get; set; }

        [Format(1, EncodingType.BIN, 3)]
        public byte ADD_BL_VER { get; set; }

        [Format(1, EncodingType.BIN, 4)]
        public byte DEL_BL_VER { get; set; }

        [Format(1, EncodingType.BIN, 5)]
        public byte WH_VER { get; set; }

        [Format(1, EncodingType.BIN, 6)]
        public byte PRC_VER { get; set; }

        [Format(1, EncodingType.BIN, 7)]
        public byte Sta_VER { get; set; }

        [Format(1, EncodingType.BIN, 8)]
        public 更新标志 SELF_D_VER { get; set; }

        [Format(1, EncodingType.BIN, 9)]
        public 更新标志 SOFT_FLAG { get; set; }

        public enum 更新标志
        {
            无数据=0,
            有新的程序or私有数据需要下载or发送=1
        }
    }
}