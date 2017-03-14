using System;
using System.Collections.Generic;

namespace MessageParser
{
    public class LoginResponse : MessageBase
    {
        [Format(1, EncodingType.BIN, 0)]
        public PinpadResultCode AckCode { get; private set; }

        // need decorate, so put to private, the format from documentation: hh:mm:ss, dd/mm/yyyy
        [Format(14, EncodingType.ASCII, 1)]
        private string RawDateTime { get; set; }

        /// <summary>
        /// decorator property
        /// Gets the datetime
        /// </summary>
        public DateTime DateTime
        {
            get
            {
                var hour = int.Parse(this.RawDateTime.Substring(0, 2));
                var minute = int.Parse(this.RawDateTime.Substring(2, 2));
                var second = int.Parse(this.RawDateTime.Substring(4, 2));
                var day = int.Parse(this.RawDateTime.Substring(6, 2));
                var month = int.Parse(this.RawDateTime.Substring(8, 2));
                var year = int.Parse(this.RawDateTime.Substring(10, 4));
                return new DateTime(year, month, day, hour, minute, second);
            }

            set
            {
                //var hour = value.Hour.ToString();
                //string.Format("{0:s}", hour);
                //var minute = value.Minute.ToString();
                //string.Format("{0:s}", minute);
                //var second = value.Second.ToString();
                //string.Format("{0:s}", second);
                //var day = value.Day.ToString();
                //var month = value.Month.ToString();
                //var year = value.Year.ToString();
                //this.RawDateTime = hour + minute + second + day + month + year;
                RawDateTime = value.ToString("HHmmssddMMyyyy");
               
            }
        }

        [Range(0, 255, "Valid values for NumApps is from {1} to {2}, but current value is: {0}")]
        [Format(1, EncodingType.BIN, 2)]
        public int NumApps { get; set; }

        [EnumerableFormat("NumApps", 3)]
        public List<SpotLocalApplication> Entries { get; set; }
    }

    public class SpotLocalApplication
    {
        [Range(0, 255, "Valid values for SpotLocalApplication's AppId is from {1} to {2}, but current value is: {0}")]
        [Format(1, EncodingType.BIN, 0)]
        public int AppId { get; set; }

        [Range(0, 2147483647, "Valid values for SpotLocalApplication's Sign is from {1} to {2}, but current value is: {0}")]
        [Format(4, EncodingType.BIN, 1)]
        public int Sign { get; set; }

        [Format(40, EncodingType.ASCII, 2)]
        public string AppAscii { get; set; }
    }

    public class RSAKey
    {
        [Range(0, 32767, "Value {0} is out of ragne for RSAKey")]
        [Format(2, EncodingType.BIN, 0)]
        public int ModuloLen { get; private set; }

        [Range(0, 32767, "Value {0} is out of ragne for ExponentLen")]
        [Format(2, EncodingType.BIN, 1)]
        public int ExponentLen { get; private set; }

        [EnumerableFormat("ModuloLen", 2, EncodingType = EncodingType.BIN)]
        public List<byte> Modulo { get; private set; }

        [EnumerableFormat("ExponentLen", 3, EncodingType = EncodingType.BIN)]
        public List<byte> Exponent { get; private set; }
    }
}