using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Host
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] portlist = System.IO.Ports.SerialPort.GetPortNames();
            Console.WriteLine("Available COM ports are: " + portlist.Aggregate((acc, n) => acc + ", " + n));

            var isPcActiveMode = ConfigurationManager.AppSettings["PcActiveMode"].ToLower() == "true";
            if (!isPcActiveMode)
            {

            }
            else { }
        }
    }
}
