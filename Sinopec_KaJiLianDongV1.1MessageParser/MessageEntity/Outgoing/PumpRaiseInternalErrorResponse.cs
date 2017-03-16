using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser.MessageEntity.Outgoing
{
    public class PumpRaiseInternalErrorResponse : KaJiLianDongV11MessageTemplateBase
    {
        [Format(1, EncodingType.BIN, 1)]
        public byte COMMAND_通讯的命令字 { get; set; }

        /// <summary>
        ///  b0= 0:接受无误/1:通讯错 
        /// </summary>
        [Format(1, EncodingType.BIN, 2)]
        public byte FLAG_成功标志 { get; set; }
    }
}
