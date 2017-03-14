using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser.MessageEntity.Outgoing
{
    public class PumpInquiryBlackAndWhiteListResponse : KaJiLianDongV11MessageTemplateBase
    {
        public enum PumpInquiryBlackAndWhiteListResult
        {
            匹配 = 0,
            不匹配 = 1,
        }
        /// <summary>
        ///  b0=0:匹配 / 其余:不匹配
        /// </summary>
        [Format(1, EncodingType.BIN, 1)]
        public PumpInquiryBlackAndWhiteListResult M_FLAG_匹配标志 { get; set; }

        [Format(10, EncodingType.BcdString, 2)]
        public string ASN卡应用号 { get; set; }
    }
}
