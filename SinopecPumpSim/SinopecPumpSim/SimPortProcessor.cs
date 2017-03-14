using System.IO.Ports;
using MainUI;

namespace SinopecPumpSim
{
    public class SimPortProcessor : ComPortProcessor
    {
        public SimPortProcessor(SerialPort port, CommActiveMode activeMode) : base(port, activeMode)
        {
        }
    }
}
