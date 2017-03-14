using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser.MessageEntity.Outgoing
{
    public class PumpNotifyTransactionDoneResponse : KaJiLianDongV11MessageTemplateBase
    {
        public PumpNotifyTransactionDoneResponse()
        {
            base.HANDLE = 0x32;
        }

        public enum PumpNotifyTransactionDoneResponseResult
        {
            正确 = 0,
            T_MAC_错 = 1,
        }

        [Format(1, EncodingType.BIN, 1)]
        public PumpNotifyTransactionDoneResponseResult Result { get; set; }
    }
}
