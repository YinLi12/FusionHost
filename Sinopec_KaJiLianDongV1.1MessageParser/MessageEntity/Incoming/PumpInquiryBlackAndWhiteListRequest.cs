using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    public class PumpInquiryBlackAndWhiteListRequest : KaJiLianDongV11MessageTemplateBase
    {
        [Format(10, EncodingType.BcdString, 1)]
        public string ASN卡应用号 { get; set; }
    }
}
