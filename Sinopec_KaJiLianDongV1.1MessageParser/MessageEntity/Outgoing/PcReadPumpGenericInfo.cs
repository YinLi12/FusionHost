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
    public class PcReadPumpGenericInfo : KaJiLianDongV11MessageTemplateBase
    {
        public PcReadPumpGenericInfo()
        {
            base.HANDLE = 0x3A;
            base.SetMessageCallerSide(MessageCallerSide.Host);
        }
    }
}