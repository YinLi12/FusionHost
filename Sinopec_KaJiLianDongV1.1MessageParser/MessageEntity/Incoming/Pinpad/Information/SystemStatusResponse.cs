using System;
using System.Collections.Generic;
using System.Linq;
namespace MessageParser
{
    public class SystemStatusResponse : MessageBase
    {
        public class Entry
        {
            /// <summary>
            ///  identifies the  specific  PCI  PTS  subsystem  modul e  to  which  the  status  info  is  referred.
            /// </summary>
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

            //Main system
            public enum SystemStatus
            {
                Startup = 0x01,
                Inoperative = 0x02,
                Offline = 0x03,
                Online = 0x04,
                Busy = 0x05,
                Service = 0x06   
            }
            public enum SystemStatusExt
            {
                 SecureDisplay = 0x00,
                 ExternalDisplay = 0x01,
                 NonSecureDisplay = 0x02,
                 Unknown = 0x03
            }
            public enum SystemError
            {
                NoError = 0x00,
                Error = 0x01
            }

            //card reader
            public enum CardReaderSystemStatus
            {
                SPOT_CR_DISABLED_STATE = 0x00,
                SPOT_CR_ENABLED_STATE = 0x01,
                SPOT_CR_BUSY_STATE = 0x02,
                SPOT_CR_ERROR_STATE = 0x03,
                SPOT_CR_INOPERATIVE_STATE = 0x04,
                SPOT_CR_STARTUP = 0x05,
                SPOT_CR_DALLASDOWNLOAD = 0x06,
                SPOT_CR_DAILYCHECK = 0x0c
            }
             public enum CardReaderStatusExt
             {
                 SPOT_CR_SENSORNOTARMED_EVENT = 0x00,
                 SensorNotArmedMagnetCardInReader = 0x01,
                 SensorNotArmedChipCardInReader = 0x03,
                 SensorArmed = 0x80,
                 MagnetCardInReader = 0x81,
                 ChipCardInReader = 0x83
             }
             public enum CardReaderError
             {
                 NoError = 0x00,
                 DriverInitError = 0x01,
                 DeviceConnectionTimeout = 0x02,
                 DeviceConnectionError = 0x03,
                 BadSoftwareVersion = 0x04,
                 RFU = 0x05,
                 NoMasterKey = 0x06,
                 EncryptionError = 0x07,
                 ReaderConnectionError = 0x08,
                 ReadError = 0x09,
                 ICCError = 0x0A,
                 MagneticWriteError = 0x0B,
                 DailyCheckStarted = 0x0C,
                 GenericError = 0x7F
             }
                            
            //keyPad
            public enum KeyPadSystemStatus
            {
                SPOT_KB_DISABLED_STATE = 0x00,
                SPOT_KB_ENABLED_STATE = 0x01,
                SPOT_KB_BUSY_STATE = 0x02,
                SPOT_KB_ERROR_STATE = 0x03,
                SPOT_KB_INOPERATIVE_STATE = 0x04,
                SPOT_KB_STARTUP = 0x05,
                SPOTKB_DALLASDOWNLOAD = 0x06
            }
            public enum KeyPadStatusExt
            {
                SPOT_KB_SENSORARMED_EVENT = 0x80,
                SPOT_KB_SENSORNOTARMED_EVENT = 0x00
            }
            public enum KeyPadError
            {
                SPOT_KB_NOERROR = 0x00,
                SPOT_KB_DRIVERINITERROR = 0x01,
                SPOT_KB_DEVICECONNECTIONTIMEOUT = 0x02,
                SPOT_KB_DEVICECONNECTIONERROR = 0x03,
                SPOT_KB_BADSOFTWAREVERSION = 0x04,
                RFU = 0x05
            }

            //Securitymodule
            public enum SecuritymoduleSystemStatus
            {
                Idle = 0x00,
                Busy = 0x01,
                Error = 0x02,
                Inoperative = 0x03,
                Startup = 0x04,
            }
            public enum SecuritymoduleStatusExt
            {
                SensorNotArmed = 0x00,
                SensorArmed = 0x80,
            }
            public enum SecuritymoduleError
            {
                NoError = 0x00,
                DriverInitError = 0x01,
                DeviceConnectionTimeout = 0x02,
                DeviceConnectionFailure = 0x03,
                BadSoftwareVersion = 0x04,
                RFU = 0x05,
                NoMasterKeyError = 0x06,
                EncryptionError = 0x07,
                SecurityModuleError = 0x09,
                KeyDatabaseError = 0x0A,
                GenericError = 0x7F, 
            }


            public SubSystemType CurrentSubSystemType { get; set; }            

            [Format(1, EncodingType.BIN, 0)]
            public SubSystemType DeviceCode { get; set; }

            [Format(1, EncodingType.BIN, 1)]
            private byte Status { get; set; }
            public void SetStatus(SubSystemType s, SystemStatus? systemStutus, CardReaderSystemStatus? cardReaderStutus, KeyPadSystemStatus? keypadStutus, SecuritymoduleSystemStatus? scuritymodule)
            {
                this.CurrentSubSystemType = s;
                switch(s)
                {
                    case SubSystemType.SPOT_SYS_MAIN:
                        if (systemStutus.HasValue)
                            this.Status = (byte)(systemStutus.Value);
                        else
                            throw new ArgumentException("expecting systemStatus enum!");
                        break;
                    case  SubSystemType.SPOT_SYS_CARDREADER:
                        if (cardReaderStutus.HasValue)
                            this.Status = (byte)(cardReaderStutus.Value);
                        else
                            throw new ArgumentException("expecting cardReaderStatus enum!");
                        break;
                    case SubSystemType.SPOT_SYS_KEYBOARD:
                        if (keypadStutus.HasValue)
                            this.Status = (byte)(keypadStutus.Value);
                        else
                            throw new ArgumentException("expecting keypadStatus enum!");
                        break;
                    case SubSystemType.SPOT_SYS_SECURITYMODULE:
                        if (scuritymodule.HasValue)
                            this.Status = (byte)(scuritymodule.Value);
                        else
                            throw new ArgumentException("expecting scuritymodule Status enum!");
                        break;

                    default:
                        break;
                }               
            }
            
            [Format(1, EncodingType.BIN, 2)]
            private byte StatusExt { get; set; }
            public void SetExtStatus(SubSystemType s, SystemStatusExt? system, CardReaderStatusExt? cardReader, KeyPadStatusExt? keypad, SecuritymoduleStatusExt? scuritymodule)
            {
                this.CurrentSubSystemType = s;
                switch (s)
                {
                    case SubSystemType.SPOT_SYS_MAIN:
                        if (system.HasValue)
                            this.StatusExt = (byte)(system.Value);
                        else
                            throw new ArgumentException("expecting systemExtStatus enum!");
                        break;
                    case SubSystemType.SPOT_SYS_CARDREADER:
                        if (cardReader.HasValue)
                            this.StatusExt = (byte)(cardReader.Value);
                        else
                            throw new ArgumentException("expecting cardReaderExtStatus enum!");
                        break;
                    case SubSystemType.SPOT_SYS_KEYBOARD:
                        if (keypad.HasValue)
                            this.StatusExt = (byte)(keypad.Value);
                        else
                            throw new ArgumentException("expecting keypadExtStatus enum!");
                        break;
                    case SubSystemType.SPOT_SYS_SECURITYMODULE:
                        if (scuritymodule.HasValue)
                            this.StatusExt = (byte)(scuritymodule.Value);
                        else
                            throw new ArgumentException("expecting scuritymodule ExtStatus enum!");
                        break;
                    default:
                        break;
                }
            }                       

            [Format(1, EncodingType.BIN, 3)]
            private byte ErrorCode { get; set; }
            public void SetErrorCode(SubSystemType s, SystemError? system, CardReaderError? cardReader, KeyPadError? keypad, SecuritymoduleError? scuritymodule)
            {
                this.CurrentSubSystemType = s;
                switch (s)
                {
                    case SubSystemType.SPOT_SYS_MAIN:
                        if (system.HasValue)
                            this.ErrorCode = (byte)(system.Value);
                        else
                            throw new ArgumentException("expecting SystemError enum!");
                        break;
                    case SubSystemType.SPOT_SYS_CARDREADER:
                        if (cardReader.HasValue)
                            this.ErrorCode = (byte)(cardReader.Value);
                        else
                            throw new ArgumentException("expecting CardReaderError enum!");
                        break;
                    case SubSystemType.SPOT_SYS_KEYBOARD:
                        if (keypad.HasValue)
                            this.ErrorCode = (byte)(keypad.Value);
                        else
                            throw new ArgumentException("expecting KeyPadError enum!");
                        break;
                    case SubSystemType.SPOT_SYS_SECURITYMODULE:
                        if (scuritymodule.HasValue)
                            this.ErrorCode = (byte)(scuritymodule.Value);
                        else
                            throw new ArgumentException("expecting scuritymodule Error enum!");
                        break;
                    default:
                        break;
                }
            }
        }

        [Format(1, EncodingType.BIN, 0)]
        public PinpadResultCode AckCode { get; set; }

        /// <summary>
        /// Gets the total NumEntries count
        /// </summary>
        [Format(1, EncodingType.BIN, 1)]
        public int NumEntries { get; set; }

        [EnumerableFormat("NumEntries", 2)]
        public List<Entry> Entries { get; set; }

        public override string ToLogString()
        {
            return base.ToLogString() + ", Now I'm in mode: " + this.Entries.Select(s => s.CurrentSubSystemType.ToString()).Aggregate((p, acc) => p + "," + acc);
        }
    }
}


//using System;
//using System.Collections.Generic;

//namespace SpotMessageParser
//{
//    public class SystemStatusResponse : MessageBase
//    {
//        public enum SubSystemType
//        {
//            Main = 0x01,
//            SecurityModule = 0x02,
//            Display = 0x04,
//            Keyboard = 0x08,
//            CardReader = 0x10,
//            SAM = 0x20,
//        }
//        public abstract class Entry
//        { }

//        public class SystemEntry : Entry
//        {
//            public enum Status
//            {
//                Startup = 0x01,
//                Inoperative = 0x02,
//                Offline = 0x03,
//                Online = 0x04,
//                Busy = 0x05,
//                Service = 0x06, 
//            }
//            public enum ExtStatus
//            {
//                SecureDisplay = 0x00,
//                ExternalDisplay = 0x01,
//                NonSecureDisplay = 0x02,
//                Unknown = 0x03,
//            }
//             public enum Error
//             {
//                 NoError = 0x00,
//                 Error = 0x01,
//             }
//            [Format(1, EncodingType.BIN, 0)]
//            public SubSystemType DeviceCode { get; set; }

//            [Format(1, EncodingType.BIN, 1)]
//             public Status statusCode { get; set; }

//            [Format(1, EncodingType.BIN, 2)]
//            public ExtStatus statusExt { get; set; }

//            [Format(1, EncodingType.BIN, 3)]
//            public Error errorCode { get; set; }
//        }

//        public class CardReadEntry : Entry
//        {
//            public enum Status
//            {
//                Disabled = 0x00,
//                Enabled = 0x01,
//                Busy = 0x02,
//                Error = 0x03,
//                Inoperative = 0x04,
//                Startup = 0x05,
//                DallasDownload = 0x06,
//                DailyCheck = 0x0C,
//            }
//            public enum ExtStatus
//            {
//                SensorNotArmed = 0x00,
//                SensorNotArmedMagnetCardInReader = 0x01,
//                SensorNotArmedChipCardInReader = 0x03,
//                SensorArmed = 0x80,
//                MagnetCardInReader = 0x81,
//                ChipCardInReader = 0x83,
//            }
//            public enum Error
//            {
//                NoError = 0x00,
//                DriverInitError = 0x01,
//                DeviceConnectionTimeout = 0x02,
//                DeviceConnectionError = 0x03,
//                BadSoftwareVersion = 0x04,
//                RFU = 0x05,
//                NoMasterKey = 0x06,
//                EncryptionError = 0x07,
//                ReaderConnectionError = 0x08,
//                ReadError = 0x09,
//                ICCError = 0x0A,
//                MagneticWriteError = 0x0B,
//                DailyCheckStarted = 0x0C,
//                GenericError = 0x7F,
//            }
//            [Format(1, EncodingType.BIN, 0)]
//            public SubSystemType DeviceCode { get; set; }

//            [Format(1, EncodingType.BIN, 1)]
//            public Status statusCode { get; set; }

//            [Format(1, EncodingType.BIN, 2)]
//            public ExtStatus statusExt { get; set; }

//            [Format(1, EncodingType.BIN, 3)]
//            public Error errorCode { get; set; }
//        }
//        public class KeyboardEntry : Entry
//        {
//            public enum Status
//            {
//                Disabled = 0x00,
//                Enabled = 0x01,
//                Busy = 0x02,
//                Error = 0x03,
//                Inoperative = 0x04,
//                Startup = 0x05,
//                DallasDownload = 0x06,
//            }
//            public enum ExtStatus
//            {
//                SensorArmed = 0x80,
//                SensorNotArmed = 0x00,
//            }
//            public enum Error
//            {
//                NoError = 0x00,
//                DriverInitError = 0x01,
//                DeviceConnectionTimeout = 0x02,
//                DeviceConnectionError = 0x03,
//                BadSoftwareVersion = 0x04,
//                RFU = 0x05,
//                NoMasterKeyError = 0x06,
//                EncryptionError = 0x07,
//                KeyboardError = 0x09,
//                GenericError = 0x7F, 
//            }
//            [Format(1, EncodingType.BIN, 0)]
//            public SubSystemType DeviceCode { get; set; }

//            [Format(1, EncodingType.BIN, 1)]
//            public Status statusCode { get; set; }

//            [Format(1, EncodingType.BIN, 2)]
//            public ExtStatus statusExt { get; set; }

//            [Format(1, EncodingType.BIN, 3)]
//            public Error errorCode { get; set; }
//        }

//         [Format(1, EncodingType.BIN, 0)]
//         public PinpadResultCode AckCode { get; set; }

//         [Format(1, EncodingType.BIN, 1)]
//         public int NumEntries { get; set; }

//         [EnumerableFormat("NumEntries", 2)]
//         public List<Entry> Entries { get; set; }
//    }

//}














