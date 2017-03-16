using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    /// <summary>
    /// 加油机对 PC 机普通查询命令 30H
    /// PC 机对加油机普通查询命令 30H
    /// </summary>
    public class PumpDataDownloadResponse : KaJiLianDongV11MessageTemplateBase
    {

        public PumpDataDownloadResponse()
        {
            base.HANDLE = 0x34;
            base.SetMessageCallerSide(MessageCallerSide.Host);
        }

        /// <summary>
        /// 0：基础黑名单， 1：新增黑名单 2: 新删黑名单 3：白名单 4：油品油价表 5：油站通用信息 6：私有数据， 7：下载程序 
        /// </summary>
        [Format(1, EncodingType.BIN, 1)]
        public PumpAskDataDownloadRequest.PumpAskDataDownloadType Content_内容 { get; set; }

        /// <summary>
        ///  同加油机的申请
        /// </summary>
        [Format(2, EncodingType.BIN, 2)]
        [Range(0, 65535, "段偏移max 65535")]
        public int S_OFFSET_段偏移 { get; set; }

        /// <summary>
        /// 实际 送出的段数
        /// </summary>
        [Format(1, EncodingType.BIN, 3)]
        public byte SEG_段数_segs { get; set; }

        /// <summary>
        /// 如果是最后的数据，则长 度应是实际长度，可能小 于segs*16 
        /// </summary>
        [EnumerableFormat("%cascade", 4, EncodingType = EncodingType.BIN)]
        public List<byte> DATA_Content_数据内容 { get; set; }
    }
}