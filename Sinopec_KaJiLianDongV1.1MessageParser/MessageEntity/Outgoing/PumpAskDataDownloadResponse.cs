using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    /// <summary>
    ///  PC 机发送需要下载的数据总长度命令 
    /// 功能：PC 机通知加油机需要下载的数据的总长度 
    /// </summary>
    public class PumpAskDataDownloadResponse : KaJiLianDongV11MessageTemplateBase
    {

        public PumpAskDataDownloadResponse()
        {
            base.HANDLE = 0x33;
            base.SetMessageCallerSide(MessageCallerSide.Host);
        }

        /// <summary>
        ///  数据文件长度； 长度为0表示数据合法，但内容为空
        /// </summary>
        [Format(4, EncodingType.BIN, 1)]
        public int BL_LEN_长度 { get; set; }

        /// <summary>
        /// 0：基础黑名单， 1：新增黑名单 2: 新删黑名单 3：白名单 4：油品油价表 5：油站通用信息 6：私有数据， 7：下载程序 
        /// </summary>
        [Format(1, EncodingType.BIN, 2)]
        public PumpAskDataDownloadRequest.PumpAskDataDownloadType Content_内容 { get; set; }
    }
}