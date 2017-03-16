using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    /// <summary>
    /// 加油机对 PC 机的普通查询命令 
    /// 功能：在加油机主动方式下，加油机定时与 PC 机进行的握手通讯；在 PC 机主动方式下，
    ///此命令无实际作用。 
    /// </summary>
    public class PcGenericInquiryRequest : KaJiLianDongV11MessageTemplateBase
    {

        public enum GenericInquiryRequestType
        {
            加油机对PC机普通查询命令30H = 0x30,
            PC机对加油机普通查询命令30H = 0x30,
            //加油机发送实时信息命令31H = 0x31
        }

        public PcGenericInquiryRequest()
        {
            base.HANDLE = 0x30;
        }
    }
}