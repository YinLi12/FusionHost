using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MessageParser
{
    [Serializable]
    public abstract class KaJiLianDongV11MessageTemplateBase : MessageTemplateBase
    {
        /// <summary>
        /// which side send the message, the value is parsed from `FrameSequenceAndControlRaw`.
        /// </summary>
        public enum MessageCallerSide
        {
            Pump,
            Host
        }

        /// <summary>
        /// 数据包头,同步头 
        /// </summary>
        [Format(1, EncodingType.BIN, -100)]
        public virtual byte Prefix
        {
            get { return 0xFA; }
            set
            {
                ;
            }
        }

        /// <summary>
        /// 在 PC 机到加油 机的通讯中是 加油机的通讯 终端的逻辑编 号 POS-P；一个 逻辑编号对应 一个通讯的物 理端口 
        /// </summary>
        [Format(1, EncodingType.BIN, -99)]
        public virtual byte TargetAddress { get; set; }

        /// <summary>
        /// 在加油机到 PC 机 的通讯是加油机 的通讯终端的逻 辑编号 POS-P .
        /// PC 机的地址范围：0xE0~0xF9
        /// </summary>
        [Format(1, EncodingType.BIN, -98)]
        public virtual byte SourceAddress { get; set; }

        /// <summary>
        /// 帧号/控制 
        /// </summary>
        [Format(1, EncodingType.BIN, -97)]
        public virtual byte FrameSequenceAndControlRaw { get; set; }

        /// <summary>
        /// which side send the message, the value is parsed from `FrameSequenceAndControlRaw`.
        /// </summary>
        public virtual MessageCallerSide GetMessageCallerSide()
        {
            var callerBit = this.FrameSequenceAndControlRaw.GetBit(6);
            if (callerBit == 1) return MessageCallerSide.Host;
            return MessageCallerSide.Pump;
        }

        /// <summary>
        /// which side send the message, the value is parsed from `FrameSequenceAndControlRaw`.
        /// </summary>
        public void SetMessageCallerSide(MessageCallerSide side)
        {
            this.FrameSequenceAndControlRaw = this.FrameSequenceAndControlRaw.SetBit(6, 6, side == MessageCallerSide.Host ? 1 : 0);
        }

        /// <summary>
        /// sequence number of the message, the value is parsed from `FrameSequenceAndControlRaw`.
        /// </summary>
        public virtual int GetMessageSequenceNumber()
        {
            var debug = (this.FrameSequenceAndControlRaw << 2);
            var d = debug.GetBinBytes(4).Last();
            var r = d >> 2;
            return r;
        }

        /// <summary>
        /// sequence number of the message, the value is parsed from `FrameSequenceAndControlRaw`.
        /// 主叫方每发 送一新帧， 此帧号加一，应答方 回送此帧号 
        /// </summary>
        public virtual void SetMessageSequenceNumber(int sequenceNumber)
        {
            // sequence number is max 5 bits.
            if (sequenceNumber > 63) throw new ArgumentOutOfRangeException("maximum sequenceNumber is 63(total 6 bits).");
            var debug = this.FrameSequenceAndControlRaw >> 6 << 6;
            this.FrameSequenceAndControlRaw = (byte)(debug + sequenceNumber);
        }

        /// <summary>
        /// 有效数据长度, 压缩 BCD，转 义字符 不计入 其中 
        /// </summary>
        [FormatAttribute(2, "%PositiveIndexLenAccumulator%", EncodingType.BCD, -96)]
        public virtual int DataLength { get; set; }

        ///// <summary>
        ///// 缩 BCD，转 义字符 不计入 其中 数据 的长 度为 “有 效数 据长 度” 的值 
        ///// </summary>
        [FormatAttribute(1, EncodingType.BIN, 0)]
        public virtual byte HANDLE { get;  set; }

        [EnumerableFormat(2, 1000, EncodingType = EncodingType.BIN)]
        public virtual List<byte> CRC16 { get; set; }

        /// <summary>
        /// Gets the code for the message, the code is variant length, most of them are 1 or 2 byte.
        /// </summary>
        //public virtual List<byte> Code { get; protected set; }

        public KaJiLianDongV11MessageTemplateBase()
        {
            // fusion always the host, the pump side msg sender is from a system embeded in pump.
            this.SetMessageCallerSide(MessageCallerSide.Host);
        }
    }

}

