using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser.MessageEntity.Outgoing.BlackAndWhiteList
{
    public class WhiteList : BlackAndWhiteListBase
    {
        public WhiteList() { }
        public WhiteList(int version, DateTime availableStartDate, DateTime availableEndDate, BlackListEffectiveAreaEnum area, int provinceCode, int cityCode)
        {
            if (version < 0 || version > 0xFF) throw new ArgumentOutOfRangeException("WhiteList version number must between 0 and 0x00FF");
            base.Version = version;
            base.SetAvailableStartDate(availableStartDate);
            base.SetAvailableEndDate(availableEndDate);
            base.SetBlackListEffectiveArea(area, provinceCode, cityCode);
        }
    }
}
