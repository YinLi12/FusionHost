using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainUI;

namespace MainUITests
{
    public class ComPortProcessorMock : ComPortProcessor
    {
        public ComPortProcessorMock() : base(null, CommActiveMode.PumpActive)
        { }
    }
}
