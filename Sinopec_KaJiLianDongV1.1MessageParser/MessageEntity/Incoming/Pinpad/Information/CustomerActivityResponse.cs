using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    public class CustomerActivityResponse : MessageBase
    {
        public enum CustomerActivity
        {
            Keypad = 0x01,
            CardReader = 0x02,
            Barcode = 0x03,
            SoftKeys = 0x04,
            TrindReader = 0x05,
            BNA = 0x06,
            Contactless = 0x07,
        }

        [Format(1, EncodingType.BIN, 0)]
        public CustomerActivity Activity { get; set; }
    }
}
