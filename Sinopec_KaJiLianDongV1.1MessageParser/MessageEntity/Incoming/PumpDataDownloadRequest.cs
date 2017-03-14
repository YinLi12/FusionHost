using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    /// <summary>
    /// 加油机对 PC 机的普通查询命令 
    /// 功能：在加油机主动方式下，加油机定时与 PC 机进行的握手通讯；在 PC 机主动方式下，
    ///此命令无实际作用。 
    /// </summary>
    public class PumpDataDownloadRequest : KaJiLianDongV11MessageTemplateBase
    {
        /// <summary>
        /// 0：基础黑名单， 1：新增黑名单 2: 新删黑名单 3：白名单 4：油品油价表 5：油站通用信息 6：私有数据， 7：下载程序 
        /// </summary>
        [Format(1, EncodingType.BIN, 1)]
        public PumpAskDataDownloadRequest.PumpAskDataDownloadType Content_内容 { get; set; }

        /// <summary>
        ///  每段长16字节 
        /// </summary>
        [Format(2, EncodingType.BIN, 2)]
        [Range(0,65535, "段偏移max 65535")]
        public int S_OFFSET_段偏移 { get; set; }

        /// <summary>
        /// 1到255 ,数据长度   segs*16
        /// </summary>
        [Format(1, EncodingType.BIN, 3)]
        public byte SEG_段数_segs  { get; set; }
    }
}