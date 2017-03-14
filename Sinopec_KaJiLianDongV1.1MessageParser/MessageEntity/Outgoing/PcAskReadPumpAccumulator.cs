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
    public class PcAskReadPumpAccumulator : KaJiLianDongV11MessageTemplateBase
    {

        public PcAskReadPumpAccumulator()
        {
            base.HANDLE = 0x38;
            base.SetMessageCallerSide(MessageCallerSide.Host);
        }
    }
}