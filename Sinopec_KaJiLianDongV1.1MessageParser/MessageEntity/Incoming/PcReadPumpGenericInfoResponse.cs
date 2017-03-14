using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    /// <summary>
    ///  加油机上送加油机信息命令 
    /// 功能：加油机上送加油机信息 
    /// </summary>
    public class PcReadPumpGenericInfoResponse : KaJiLianDongV11MessageTemplateBase
    {
        /// <summary>
        /// 前对齐，后补空格 
        /// </summary>
        [Format(12, EncodingType.ASCII, 1)]
        public string M_INFO_厂家信息 { get; set; }

        /// <summary>
        /// 石化编码规范中地区编码的头两个数字
        /// </summary>
        [Format(1, EncodingType.BCD, 2)]
        public byte Prov_省代号 { get; set; }

        /// <summary>
        /// 石化编码规范中地区编码的中间两个数字
        /// </summary>
        [Format(1, EncodingType.BCD, 3)]
        public byte City_地市代码 { get; set; }

        /// <summary>
        /// 同石化编码
        /// </summary>
        [Format(4, EncodingType.BCD, 4)]
        public byte Superior_上级单位代号 { get; set; }

        /// <summary>
        /// 同石化编码
        /// </summary>
        [Format(4, EncodingType.BCD, 5)]
        public byte S_ID_加油站ID { get; set; }

        [Format(7, EncodingType.BCD, 6)]
        public long TIME_Raw { get; private set; }

        public DateTime GetPcTime()
        {
            var dd = this.TIME_Raw.ToString();
            var dt = Convert.ToDateTime(dd);
            return dt;
        }

        //public void SetPcTime(DateTime dateTime)
        //{
        //    var str = dateTime.ToString("yyyyMMddHHmmss");
        //    this.TIME_Raw = long.Parse(str);
        //}

        /// <summary>
        /// 
        /// </summary>
        [Format(1, EncodingType.BIN, 7)]
        public byte GUN_N_油枪数 { get; set; }

        [EnumerableFormat("GUN_N_油枪数", 8, EncodingType = EncodingType.BIN)]
        public List<byte> MZN_单个油枪 { get; set; }

        /// <summary>
        /// 基础黑名单版本号 
        /// </summary>
        [Format(2, EncodingType.BIN, 9)]
        public int BL_VER { get; set; }

        /// <summary>
        /// 新增黑名单版本 
        /// </summary>
        [Format(1, EncodingType.BIN, 10)]
        public byte ADD_BL_VER { get; set; }

        /// <summary>
        /// 新删黑名单版本 
        /// </summary>
        [Format(1, EncodingType.BIN, 11)]
        public byte DEL_BL_VER { get; set; }

        /// <summary>
        /// 白名单版本号 
        /// </summary>
        [Format(1, EncodingType.BIN, 12)]
        public byte WH_VER { get; set; }

        /// <summary>
        /// 油品油价版本 
        /// </summary>
        [Format(1, EncodingType.BIN, 13)]
        public byte PRC_VER { get; set; }

        /// <summary>
        /// 油站通用信息版本 
        /// </summary>
        [Format(1, EncodingType.BIN, 14)]
        public byte Sta_VER { get; set; }

        [EnumerableFormat("%cascade", 15, EncodingType = EncodingType.BIN)]
        public List<byte> M_DATA_厂家自定义数据 { get; set; }
    }
}