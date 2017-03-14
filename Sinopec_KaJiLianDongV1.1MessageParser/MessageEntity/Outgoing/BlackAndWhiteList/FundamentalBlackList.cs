using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser.MessageEntity.Outgoing.BlackAndWhiteList
{
    public class FundamentalBlackList : BlackAndWhiteListBase
    {
        public FundamentalBlackList(int version, DateTime availableStartDate, DateTime availableEndDate, BlackListEffectiveAreaEnum area, int provinceCode, int cityCode)
        {
            if (version < 0 || version > 0xFFFF) throw new ArgumentOutOfRangeException("Blacklist version number must between 0 and 0xFFFF");
            base.Version = version;
            base.SetAvailableStartDate(availableStartDate);
            base.SetAvailableEndDate(availableEndDate);
            base.SetBlackListEffectiveArea(area, provinceCode, cityCode);
        }
    }
}
