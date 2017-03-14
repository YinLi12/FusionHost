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
    public class PumpGenericInquiryRequest : KaJiLianDongV11MessageTemplateBase
    {
        public override string ToLogString()
        {
            return "PumpGenericInquiryRequest, caller: " + this.GetMessageCallerSide() + ", SeqNo.: " + this.GetMessageSequenceNumber() + ", " + base.ToLogString();
        }
    }
}