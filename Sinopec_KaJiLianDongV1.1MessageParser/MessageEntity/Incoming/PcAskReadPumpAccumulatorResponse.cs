using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    /// <summary>
    /// PC 机读取加油机信息命令 
    /// </summary>
    public class PcAskReadPumpAccumulatorResponse : KaJiLianDongV11MessageTemplateBase
    {
        /// <summary>
        /// 油枪数n 
        /// </summary>
        [Format(1, EncodingType.BIN, 1)]
        public byte GUN_N_油枪数 { get; set; }

        [EnumerableFormat("GUN_N_油枪数", 2, EncodingType = EncodingType.BIN)]
        public List<NozzleAndAccumulator> MZN_单个油枪 { get; set; }


        public class NozzleAndAccumulator
        {
            /// <summary>
            /// 枪号 
            /// </summary>
            [Format(1, EncodingType.BIN, 1)]
            public byte NZN_枪号 { get; set; }

            /// <summary>
            /// 升累计,单位：0.01 升 
            /// </summary>
            [Format(4, EncodingType.BIN, 2)]
            public int V_TOT_升累计 { get; set; }
        }
    }
}