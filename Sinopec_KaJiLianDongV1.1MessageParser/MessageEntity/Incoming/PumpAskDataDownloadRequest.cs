using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    /// <summary>
    /// 加油机向 PC 机申请下载数据命令 
    /// 功能：加油机启动从 PC 机下载数据命令 
    /// </summary>
    public class PumpAskDataDownloadRequest : KaJiLianDongV11MessageTemplateBase
    {
        public enum PumpAskDataDownloadType
        {
            基础黑名单 = 0,
            新增黑名单 = 1,
            新删黑名单 = 2,
            白名单 = 3,
            油品油价表 = 4,
            油站通用信息 = 5,
            私有数据 = 6,
            下载程序 = 7,
        }
        /// <summary>
        /// 0：基础黑名单， 1：新增黑名单 2: 新删黑名单 3：白名单 4：油品油价表 5：油站通用信息 6：私有数据， 7：下载程序 
        /// </summary>
        [Format(1, EncodingType.BIN, 1)]
        public PumpAskDataDownloadType Content_内容 { get; set; }

        /// <summary>
        /// 当下载增删黑名单时有效，为内部基础黑名单的版本
        /// </summary>
        [Format(2, EncodingType.BIN, 2)]
        public int IBL_VER_基础黑名单版本 { get; set; }
    }
}