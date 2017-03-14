using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser.MessageEntity.Incoming
{
    public class PumpRaiseInternalErrorRequest : KaJiLianDongV11MessageTemplateBase
    {
        /// <summary>
        /// 信息类别 , b0= 0：ASCII / 1：需要解释 
        /// </summary>
        [Format(1, EncodingType.BIN, 1)]
        public byte INFO_Type_信息类别 { get; set; }

        /// <summary>
        /// 数据 
        /// </summary>
        [EnumerableFormat("%cascade", 2, EncodingType = EncodingType.BIN)]
        public List<byte> DATA { get; set; }
    }
}
