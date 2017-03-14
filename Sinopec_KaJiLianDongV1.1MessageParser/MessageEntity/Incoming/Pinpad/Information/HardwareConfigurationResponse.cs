using System.Collections.Generic;

namespace MessageParser
{
    public class HardwareConfigurationResponse : MessageBase
    {
        [Format(1, EncodingType.BIN, 0)]
        public PinpadResultCode AckCode { get; set; }
        
        [Format(1, EncodingType.BIN, 1)]
        public int NumDevices { get; set; }

        [EnumerableFormat("NumDevices", 2)]
        public List<HardwareDevice> Devices { get; set; }

        public class HardwareDevice
        {
            public enum SubSystemType
            {
                /// <summary>
                /// The whole system
                /// </summary>
                SPOT_SYS_MAIN = 0x01,

                /// <summary>
                /// Security module subsystem
                /// </summary>
                SPOT_SYS_SECURITYMODULE = 0x02,

                /// <summary>
                /// Display subsystem
                /// </summary>
                SPOT_SYS_DISPLAY = 0x04,

                /// <summary>
                /// Keyboard subsystem
                /// </summary>
                SPOT_SYS_KEYBOARD = 0x08,

                /// <summary>
                /// Card reader subsystem
                /// </summary>
                SPOT_SYS_CARDREADER = 0x10
            }

            [Format(1, EncodingType.BIN, 0)]
            public SubSystemType DeviceCode { get; set; }

            [Format(1, EncodingType.BIN, 1)]
            public byte TagLength { get; set; }

            [Format(10, EncodingType.ASCII, 2)]
            public string TagData { get; set; }
        }
    }
}