using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    public class PumpStationInfo : MessageTemplateBase
    {
        private byte _Ver;
        /// <summary>
        /// 版本 ,    0,1～255     0: 数据无效 
        /// </summary>
        [Format(1, EncodingType.BIN, 1)]
        public byte Ver
        {
            get { return this._Ver; }
            set
            {
                this._Ver = value > 255 ? (byte)1 : value;
            }
        }
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
        [Format(4, EncodingType.BcdString, 4)]
        public string Superior_上级单位代号 { get; set; }

        /// <summary>
        /// 同石化编码
        /// </summary>
        [Format(4, EncodingType.BcdString, 5)]
        public string S_ID_加油站ID { get; set; }

        /// <summary>
        /// 通讯终端逻辑编号 0-249 
        /// </summary>
        [Format(1, EncodingType.BCD, 6)]
        [Range(0, 249, "0-249 ")]
        public byte POS_P_通讯终端逻辑编号 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Format(1, EncodingType.BIN, 7)]
        public byte GUN_N_油枪数 { get; set; }

        [EnumerableFormat("GUN_N_油枪数", 8, EncodingType = EncodingType.BIN)]
        public List<byte> MZN_单个油枪 { get; set; }
    }
}
