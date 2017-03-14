using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    public class FuelPriceList : MessageTemplateBase
    {
        public FuelPriceList() { }
        public FuelPriceList(byte version)
        {
            this.VER_版本 = version;
        }

        private byte _VER_版本;
        /// <summary>
        /// 版本,范围0,1-255,  0: 数据无效 
        /// </summary>
        [Format(1, EncodingType.BIN, 1)]
        [Range(0, 255, "0,1-255")]
        public byte VER_版本
        {
            get { return this._VER_版本; }
            set
            {
                this._VER_版本 = value > 255 ? (byte)1 : value;
            }
        }

        /// <summary>
        /// 新油品油价生效时间 , YYYY MM DD hh mm 
        /// do not set this directly, use help function Set_V_D_andT_新油品油价生效时间(DateTime time)
        /// </summary>
        [Format(6, EncodingType.BcdString, 2)]
        public string V_D_andT_新油品油价生效时间 { get; set; }

        public void Set_V_D_andT_新油品油价生效时间(DateTime time)
        {
            this.V_D_andT_新油品油价生效时间 = time.ToString("yyyyMMddHHmm");
        }

        /// <summary>
        /// 版本, 0: 数据无效 
        /// </summary>
        [Format(1, EncodingType.BIN, 3)]
        public int FieldNum_记录数 { get; set; }

        [EnumerableFormat("FieldNum_记录数", 4, EncodingType = EncodingType.BIN)]
        public List<FuelPriceRecord> 当前油品油价记录List { get; set; }

        [EnumerableFormat("FieldNum_记录数", 5, EncodingType = EncodingType.BIN)]
        public List<FuelPriceRecord> 新油品油价记录List { get; set; }
    }

    public class FuelPriceRecord : MessageTemplateBase
    {
        public FuelPriceRecord() { }
        /// <summary>
        /// 枪号 
        /// </summary>
        [Format(1, EncodingType.BIN, 1)]
        public int NZN_枪号 { get; set; }

        /// <summary>
        /// 油品代码 
        /// </summary>
        [Format(2, EncodingType.BcdString, 2)]
        public string O_Type_油品代码 { get; set; }

        /// <summary>
        /// 密度 
        /// </summary>
        [Format(4, EncodingType.BIN, 3)]
        public int Den_密度 { get; set; }

        /// <summary>
        ///  价格数目  
        /// </summary>
        [Format(1, EncodingType.BIN, 4)]
        [Range(1, 8, "1-8")]
        public byte PRC_n_价格数目 { get; set; }

        /// <summary>
        /// 范围 0～9999 ,表示：0～99.99 
        /// </summary>
        [EnumerableFormat("PRC_n_价格数目", 5, EncodingType = EncodingType.BIN)]
        public List<Price> PrcList { get; set; }
    }

    public class Price : MessageTemplateBase
    {
        public Price() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="price">must be in range 0～9999 表示：0～99.99 </param>
        public Price(int price)
        {
            this.SetPrice(price);
        }
        /// <summary>
        ///  价格byte0, high digit
        /// do not set this directly, use help function SetPrice(int price)
        /// </summary>
        [Format(1, EncodingType.BIN, 1)]
        public byte PriceByte0Raw { get; set; }

        /// <summary>
        ///  价格byte1, low digit
        /// do not set this directly, use help function SetPrice(int price)
        /// </summary>
        [Format(1, EncodingType.BIN, 2)]
        public byte PriceByte1Raw { get; set; }

        /// <summary>
        ///  0～9999 表示：0～99.99 
        /// </summary>
        /// <param name="price"></param>
        public void SetPrice(int price)
        {
            if (price < 0 || price > 9999) throw new ArgumentOutOfRangeException("0～9999");
            var bytes = price.GetBinBytes(2);
            this.PriceByte0Raw = bytes[0];
            this.PriceByte1Raw = bytes[1];
        }

        public int GetPrice()
        {
            byte[] r = new[] { this.PriceByte0Raw, this.PriceByte1Raw };
            return r.ToInt32();
        }

        //public static Price Create(int newPrice)
        //{
        //    Price p = new Price(newPrice);
        //    p.SetPrice(newPrice);
        //    return p;
        //}
    }
}
