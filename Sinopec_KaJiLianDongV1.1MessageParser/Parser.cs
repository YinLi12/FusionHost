using MessageParser;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MessageParser
{
    public class Parser : ParserBase
    {
        public Parser() : base(new MessageTemplateLookup())
        {
        }

        public override byte[] Serialize(MessageTemplateBase message)
        {
            byte[] bytesWithoutCrc16 = base.Serialize(message);
            if (message.GetType().GetProperty("CRC16") != null)
            {
                var crc16 = bytesWithoutCrc16.Skip(1).ToArray().ComputeChecksumBytesCrc16();
                return bytesWithoutCrc16.Concat(crc16).ToArray();
            }

            return bytesWithoutCrc16;
        }
    }
}
