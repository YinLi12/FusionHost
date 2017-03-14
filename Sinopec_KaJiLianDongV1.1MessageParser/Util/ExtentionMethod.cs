using System;
using System.Collections.Generic;
using System.Linq;


namespace MessageParser
{
    public static class ExtentionMethod
    {
        /// <summary>
        /// string "123" convert to byte[0] 0000001, byte[1] 00100011
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToBCD(this string value)
        {
            if (value.Length % 2 != 0)
            {
                value = "0" + value;
            }

            List<byte> result = new List<byte>();
            for (int i = 0; i < value.Length; i += 2)
            {
                int parsed = -1;
                try
                {
                    parsed = int.Parse(value[i].ToString());
                }
                catch
                {
                    throw new ArgumentException(value[i].ToString() + " in string: " + value + ", can't convert to int.");
                }
                byte b = (byte)(parsed << 4);
                if (i + 1 <= value.Length - 1)
                {
                    try
                    {
                        parsed = int.Parse(value[i + 1].ToString());
                    }
                    catch
                    {
                        throw new ArgumentException(value[i + 1].ToString() + " in string: " + value + ", can't convert to int.");
                    }

                    b += (byte)parsed;
                }

                result.Add(b);
            }

            return result.ToArray();
        }

        /// <summary>
        /// convert bytes to a Hex format log String, each byte divided by a SPACE.
        /// byte[0] = 1, byte[1] = 15, byte[2] = 134, output 01 0F 86.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexLogString(this IEnumerable<byte> bytes)
        {
            return bytes.Select(s => s.ToString("X").PadLeft(2, '0')).Aggregate((acc, n) => acc + " " + n);
        }
        /// <summary>
        /// Convert bytes to a BCD int value, e.g.: input byte[0]=00000110, byte[1]=00010001, output 611
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int GetBCD(this IEnumerable<byte> bytes)
        {
            string final = bytes.Aggregate(string.Empty, (current, b) => current + b.GetBCDString());
            return int.Parse(final);
        }

        /// <summary>
        /// Convert one byte to a BCD string, e.g.: input 00020001, output 21
        /// </summary>
        /// <param name="oneByte"></param>
        /// <returns></returns>
        public static int GetBCD(this byte oneByte)
        {
            int low = oneByte - (oneByte >> 4) * 16;
            int high = oneByte >> 4;
            if (high > 9 || low > 9) throw new ArgumentException("invalid BCD number!");
            return low + high * 10;
        }

        /// <summary>
        /// Convert one byte to a BCD string, e.g.: input 00020001, output "21"
        /// </summary>
        /// <param name="oneByte">one byte need to be converted</param>
        /// <returns>string value stands for the BCD</returns>
        public static string GetBCDString(this byte oneByte)
        {
            return oneByte.GetBCD() < 10 ? oneByte.GetBCD().ToString().PadLeft(2, '0') : oneByte.GetBCD().ToString();
        }

        /// <summary>
        /// Convert one byte to a BCD string, e.g.: input 00110001 00010100 00020001, output "311421"
        /// </summary>
        /// <param name="oneByte">one byte need to be converted</param>
        /// <returns>string value stands for the BCD</returns>
        public static string GetBCDString(this byte[] targetBytes)
        {
            string result = "";
            for (int i = 0; i < targetBytes.Length; i++)
            {
                result += targetBytes[i].GetBCD() < 10 ? targetBytes[i].GetBCD().ToString().PadLeft(2, '0') : targetBytes[i].GetBCD().ToString();
            }

            return result;
        }

        /// <summary>
        /// Convert a bytes to a Int32 value, the byte element which has smaller index have the bigger power.
        /// like 2 length byte array: byte[0] = 1, byte[1] = 2, then the converted Int32 value is 0000000100000010 = 258  
        /// </summary>
        /// <param name="targetBytes">input bytes</param>
        /// <returns>int32 value</returns>
        public static int ToInt32(this byte[] targetBytes)
        {
            int accu = 0;
            for (int i = 0; i < targetBytes.Length; i++)
            {
                accu += targetBytes[i] << ((targetBytes.Length - i - 1) * 8);
            }

            return accu;
        }

        /// <summary>
        /// Replace some bits in a byte.    like 10 in decimal, unfold it to bit(right most is the bit 0): 00001010, 
        /// now want to replace the from bit 1 to bit 3 to decimal 3 (unfold with 011), then the replaced value is 00000110 = 3.
        /// </summary>
        /// <param name="target">target byte</param>
        /// <param name="bitStartIndex">which bits to start, based on 0</param>
        /// <param name="bitEndIndex">replace to which bits, based on 0</param>
        /// <param name="replacedValue">value which replaced to</param>
        /// <returns>the replaced value</returns>
        public static byte SetBit(this byte target, int bitStartIndex, int bitEndIndex, int replacedValue)
        {
            if (bitStartIndex < 0 || bitEndIndex > 7 || bitEndIndex < bitStartIndex)
            {
                throw new ArgumentException("bitStartIndex or bitEndIndex value is not valid");
            }

            byte mask = 0;
            for (int i = 0; i < bitEndIndex - bitStartIndex + 1; i++)
            {
                mask += (byte)Math.Pow(2, i);
            }

            if (replacedValue > mask)
            {
                throw new ArgumentOutOfRangeException("Replaced value: " + replacedValue + " cannot fit the bits range");
            }

            byte maskedValue = (byte)(target & (255 - (mask << bitStartIndex)));
            return (byte)(maskedValue + (replacedValue << bitStartIndex));
        }

        /// <summary>
        /// Like int 28, interpreted as binary 00011100, so index 0 bit is 0, index 2 bit is 1, index 3 bit is 1,
        /// index 4 bit is 1, index 5 bit is 0.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="index">from low to high digits, based on 0.</param>
        /// <returns></returns>
        public static byte GetBit(this byte target, int index)
        {
            if (index < 0 || index > 7)
            {
                throw new ArgumentException("index value is not valid");
            }

            var turn = (target >> (index));
            var t = (turn << 7).GetBinBytes(4).Last() >> 7;
            return (byte)t;
        }

        /// <summary>
        /// Convert a hex string to a byte[]
        /// like targetHexString is 01030E, then the result bytes is: byte[0] = 1, byte[1] = 3, byte [2] = 14. 
        /// </summary>
        /// <param name="targetHexString">input hex string</param>
        /// <returns>byte[]</returns>
        public static byte[] ToBytes(this string targetHexString)
        {
            targetHexString = targetHexString.Trim().Replace(" ", "");
            if (targetHexString.Length % 2 != 0)
            {
                throw new ArgumentException("The targetHexString length is not even.");
            }

            List<byte> result = new List<byte>();
            for (int i = 0; i < targetHexString.Length; i += 2)
            {
                var middle = targetHexString.Substring(i, 2);
                result.Add(Convert.ToByte(middle, 16));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Append a bytes to the head of base bytes, like base bytes is byte[0] = 0, byte[1]=1, appending bytes is byte[0] = 2, then the result
        /// is byte[0] = 2, byte[1] = 0, byte[2] = 1.
        /// </summary>
        /// <param name="baseBytes"></param>
        /// <param name="bytesToAppending"></param>
        /// <returns></returns>
        public static byte[] AppendToHeader(this byte[] baseBytes, byte[] bytesToAppending)
        {
            return bytesToAppending.Concat(baseBytes).ToArray();
        }

        /// <summary>
        /// Convert a int to a bytes with BCD encoding, like 13(with outputtotalBytes set to 4) 
        /// convert to byte[0] = 00000000, byte[1] = 00010011
        /// </summary>
        /// <param name="targetInt">target int value</param>
        /// <param name="outputTotalBytes">the array length of the output, put <![CDATA[<=]>]]>0 as the auto detect length</param>
        /// <returns>bytes</returns>
        public static byte[] GetBCDBytes(this long targetInt, int outputTotalBytes)
        {
            // put each digit to a single byte, like int 134 put into byte[0] = 00000000, byte[1] = 00000011, byte[2] = 00000100
            var process = targetInt.ToString().Select(b => (byte)int.Parse(b.ToString())).ToList();
            // always extend to even
            if (process.Count() % 2 != 0)
            {
                process = process.ToArray().AppendToHeader(new byte[] { 0 }).ToList();
            }

            // left move 4 bit for odd index item, then plus the followed even index item, form one byte of BCD.
            // then remove that odd index item.
            for (int i = process.Count() - 1; i >= 0; i--)
            {
                process[i] += (byte)(process.ToArray()[i - 1] << 4);
                process.RemoveAt(i - 1);
                i = i - 1;
            }

            var diff = outputTotalBytes - process.Count;
            if (diff > 0)
            {
                var v = Enumerable.Repeat(0, diff).Select(p => (byte)p).Concat(process).ToArray();
                return v;
            }

            return process.ToArray();
        }

        public static byte[] GetBCDBytes(this int targetInt, int outputTotalBytes)
        {
            // put each digit to a single byte, like int 134 put into byte[0] = 00000000, byte[1] = 00000011, byte[2] = 00000100
            var process = targetInt.ToString().Select(b => (byte)int.Parse(b.ToString())).ToList();
            // always extend to even
            if (process.Count() % 2 != 0)
            {
                process = process.ToArray().AppendToHeader(new byte[] { 0 }).ToList();
            }

            // left move 4 bit for odd index item, then plus the followed even index item, form one byte of BCD.
            // then remove that odd index item.
            for (int i = process.Count() - 1; i >= 0; i--)
            {
                process[i] += (byte)(process.ToArray()[i - 1] << 4);
                process.RemoveAt(i - 1);
                i = i - 1;
            }

            var diff = outputTotalBytes - process.Count;
            if (diff > 0)
            {
                var v = Enumerable.Repeat(0, diff).Select(p => (byte)p).Concat(process).ToArray();
                return v;
            }

            return process.ToArray();
        }

        /// <summary>
        /// Convert a int value to a bytes, like 3 convert to byte[0] = 00000011; 256 convert to byte[0] = 00000001, byte[1] = 00000001;
        /// 257 conver to byte[0] = 00000001, byte[1] = 00000011;
        /// </summary>
        /// <param name="targetInt">target int value</param>
        /// <param name="outputTotalBytes">the array length of the output, put <![CDATA[<=]>]]>0 as the auto detect length</param>
        /// <returns>bytes</returns>
        public static byte[] GetBinBytes(this int targetInt, int outputTotalBytes)
        {
            if (outputTotalBytes > 4) throw new ArgumentOutOfRangeException("GetBinBytes support max 4 bytes output");

            var r = BitConverter.GetBytes(targetInt);

            if (outputTotalBytes < r.Length)
            {
                for (int index = r.Length - 1; index > outputTotalBytes - 1; index--)
                {
                    if (r[index] > 0)
                    {
                        throw new ArgumentException(string.Format("Target integer value {0} is too large to be converted into {1} bytes", targetInt, outputTotalBytes));
                    }
                }
            }

            if (outputTotalBytes > 0)
            {
                return r.Take(outputTotalBytes).Reverse().ToArray();
            }

            return r.Reverse().ToArray();
        }

        /// <summary>
        /// Convert a bytes array to a string with hex encoding, like a byte[] { 00, 00, 64, 04 }, then converted string will be 00004004
        /// </summary>
        /// <param name="target">target byte array</param>
        /// <returns>a string with hex encoding</returns>
        public static string GetHexString(this IEnumerable<byte> target)
        {
            return target.Select(b => b.ToString("X").Length == 1 ? b.ToString("X").PadLeft(2, '0') : b.ToString("X")).Aggregate((p, n) => { return p + n; });
        }

        /// <summary>
        /// Get the first or default element in a WinEps message which specified by the element type.
        /// Since the element is a reference type, so nothing find will return a Null as the default value.
        /// </summary>
        /// <typeparam name="T">the look up element type</typeparam>
        /// <param name="message">WinEps message</param>
        /// <returns>the element, if not found, return null</returns>
        //public static T FirstOrDefaultElement<T>(this WinEpsMessageBase message) where T : WinEpsElementBase
        //{
        //    return message.FirstOrDefault(e => e.GetType() == typeof(T)) as T;
        //}

        //private static ushort[] table = {
        //      0x0000, 0x1189, 0x2312, 0x329B, 0x4624, 0x57AD, 0x6536, 0x74BF,
        //      0x8C48, 0x9DC1, 0xAF5A, 0xBED3, 0xCA6C, 0xDBE5, 0xE97E, 0xF8F7,
        //      0x1081, 0x0108, 0x3393, 0x221A, 0x56A5, 0x472C, 0x75B7, 0x643E,
        //      0x9CC9, 0x8D40, 0xBFDB, 0xAE52, 0xDAED, 0xCB64, 0xF9FF, 0xE876,
        //      0x2102, 0x308B, 0x0210, 0x1399, 0x6726, 0x76AF, 0x4434, 0x55BD,
        //      0xAD4A, 0xBCC3, 0x8E58, 0x9FD1, 0xEB6E, 0xFAE7, 0xC87C, 0xD9F5,
        //      0x3183, 0x200A, 0x1291, 0x0318, 0x77A7, 0x662E, 0x54B5, 0x453C,
        //      0xBDCB, 0xAC42, 0x9ED9, 0x8F50, 0xFBEF, 0xEA66, 0xD8FD, 0xC974,
        //      0x4204, 0x538D, 0x6116, 0x709F, 0x0420, 0x15A9, 0x2732, 0x36BB,
        //      0xCE4C, 0xDFC5, 0xED5E, 0xFCD7, 0x8868, 0x99E1, 0xAB7A, 0xBAF3,
        //      0x5285, 0x430C, 0x7197, 0x601E, 0x14A1, 0x0528, 0x37B3, 0x263A,
        //      0xDECD, 0xCF44, 0xFDDF, 0xEC56, 0x98E9, 0x8960, 0xBBFB, 0xAA72,
        //      0x6306, 0x728F, 0x4014, 0x519D, 0x2522, 0x34AB, 0x0630, 0x17B9,
        //      0xEF4E, 0xFEC7, 0xCC5C, 0xDDD5, 0xA96A, 0xB8E3, 0x8A78, 0x9BF1,
        //      0x7387, 0x620E, 0x5095, 0x411C, 0x35A3, 0x242A, 0x16B1, 0x0738,
        //      0xFFCF, 0xEE46, 0xDCDD, 0xCD54, 0xB9EB, 0xA862, 0x9AF9, 0x8B70,
        //      0x8408, 0x9581, 0xA71A, 0xB693, 0xC22C, 0xD3A5, 0xE13E, 0xF0B7,
        //      0x0840, 0x19C9, 0x2B52, 0x3ADB, 0x4E64, 0x5FED, 0x6D76, 0x7CFF,
        //      0x9489, 0x8500, 0xB79B, 0xA612, 0xD2AD, 0xC324, 0xF1BF, 0xE036,
        //      0x18C1, 0x0948, 0x3BD3, 0x2A5A, 0x5EE5, 0x4F6C, 0x7DF7, 0x6C7E,
        //      0xA50A, 0xB483, 0x8618, 0x9791, 0xE32E, 0xF2A7, 0xC03C, 0xD1B5,
        //      0x2942, 0x38CB, 0x0A50, 0x1BD9, 0x6F66, 0x7EEF, 0x4C74, 0x5DFD,
        //      0xB58B, 0xA402, 0x9699, 0x8710, 0xF3AF, 0xE226, 0xD0BD, 0xC134,
        //      0x39C3, 0x284A, 0x1AD1, 0x0B58, 0x7FE7, 0x6E6E, 0x5CF5, 0x4D7C,
        //      0xC60C, 0xD785, 0xE51E, 0xF497, 0x8028, 0x91A1, 0xA33A, 0xB2B3,
        //      0x4A44, 0x5BCD, 0x6956, 0x78DF, 0x0C60, 0x1DE9, 0x2F72, 0x3EFB,
        //      0xD68D, 0xC704, 0xF59F, 0xE416, 0x90A9, 0x8120, 0xB3BB, 0xA232,
        //      0x5AC5, 0x4B4C, 0x79D7, 0x685E, 0x1CE1, 0x0D68, 0x3FF3, 0x2E7A,
        //      0xE70E, 0xF687, 0xC41C, 0xD595, 0xA12A, 0xB0A3, 0x8238, 0x93B1,
        //      0x6B46, 0x7ACF, 0x4854, 0x59DD, 0x2D62, 0x3CEB, 0x0E70, 0x1FF9,
        //      0xF78F, 0xE606, 0xD49D, 0xC514, 0xB1AB, 0xA022, 0x92B9, 0x8330,
        //      0x7BC7, 0x6A4E, 0x58D5, 0x495C, 0x3DE3, 0x2C6A, 0x1EF1, 0x0F78
        //    };
        static ushort[] table = new ushort[256];
        const ushort polynomial = 0xA001;
        private static ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        public static byte[] ComputeChecksumBytesCrc16(this byte[] bytes)
        {
            ushort crc = ComputeChecksum(bytes);
            var result = BitConverter.GetBytes(crc);
            return result.Reverse().ToArray();
        }

        static ExtentionMethod()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }
    }

    //public class Crc16
    //{
    //    const ushort polynomial = 0xA001;
    //    ushort[] table = new ushort[256];

    //    public ushort ComputeChecksum(byte[] bytes)
    //    {
    //        ushort crc = 0;
    //        for (int i = 0; i < bytes.Length; ++i)
    //        {
    //            byte index = (byte)(crc ^ bytes[i]);
    //            crc = (ushort)((crc >> 8) ^ table[index]);
    //        }
    //        return crc;
    //    }

    //    public byte[] ComputeChecksumBytesCrc16(byte[] bytes)
    //    {
    //        ushort crc = ComputeChecksum(bytes);
    //        return BitConverter.GetBytes(crc);
    //    }

    //    public Crc16()
    //    {
    //        ushort value;
    //        ushort temp;
    //        for (ushort i = 0; i < table.Length; ++i)
    //        {
    //            value = 0;
    //            temp = i;
    //            for (byte j = 0; j < 8; ++j)
    //            {
    //                if (((value ^ temp) & 0x0001) != 0)
    //                {
    //                    value = (ushort)((value >> 1) ^ polynomial);
    //                }
    //                else
    //                {
    //                    value >>= 1;
    //                }
    //                temp >>= 1;
    //            }
    //            table[i] = value;
    //        }
    //    }
    //}

}
