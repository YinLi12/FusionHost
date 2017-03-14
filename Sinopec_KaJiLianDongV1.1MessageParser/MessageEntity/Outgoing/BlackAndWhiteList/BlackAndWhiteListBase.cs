using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    public abstract class BlackAndWhiteListBase : MessageTemplateBase
    {
        [Format(2, EncodingType.BIN, 1)]
        public int Version { get; set; }

        [Format(4, EncodingType.BcdString, 2)]
        public string AvailableStartDate { get; set; }

        public void SetAvailableStartDate(DateTime dateTime)
        {
            this.AvailableStartDate = dateTime.ToString("yyyyMMdd");
        }

        public DateTime GetAvaialbleStartDate()
        {
            CultureInfo culture = new CultureInfo("zh-CN");
            var dt = DateTime.ParseExact(this.AvailableStartDate, "yyyyMMdd", culture);
            return dt;
        }


        [Format(4, EncodingType.BcdString, 3)]
        public string AvailableEndDate { get; set; }

        public void SetAvailableEndDate(DateTime dateTime)
        {
            this.AvailableEndDate = dateTime.ToString("yyyyMMdd");
        }

        public DateTime GetAvaialbleEndDate()
        {
            CultureInfo culture = new CultureInfo("zh-CN");
            var dt = DateTime.ParseExact(this.AvailableEndDate, "yyyyMMdd", culture);
            return dt;
        }

        public enum BlackListEffectiveAreaEnum
        {
            全国黑名单,
            本省黑名单,
            本市黑名单
        }

        [Format(2, EncodingType.BIN, 4)]
        public int BlackListEffectiveArea { get; set; }

        public void SetBlackListEffectiveArea(BlackListEffectiveAreaEnum area, int provinceCode, int cityCode)
        {
            if (area == BlackListEffectiveAreaEnum.全国黑名单)
            {
                this.BlackListEffectiveArea = 0xFFFF;
            }
            else if (area == BlackListEffectiveAreaEnum.本省黑名单)
            {
                this.BlackListEffectiveArea = (provinceCode.GetBCDBytes(2).ToInt32() << 8) + 0x00FF;
            }
            else if (area == BlackListEffectiveAreaEnum.本市黑名单)
            {
                this.BlackListEffectiveArea = (provinceCode.GetBCDBytes(2).ToInt32() << 8) + cityCode.GetBCDBytes(2).ToInt32();
            }
        }

        [Format(4, EncodingType.BIN, 5)]
        public int 名单数量 { get; set; }

        [EnumerableFormat("名单数量", 6, EncodingType = EncodingType.BIN)]
        public List<CardSerialNumber> CardSerialNumbers { get; set; }
    }

    public class CardSerialNumber
    {
        [Format(10, EncodingType.BcdString, 1)]
        public string SerialNumber { get; set; }
    }
}
