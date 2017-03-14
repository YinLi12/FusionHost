using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    /// <summary>
    /// 加油机发送实时信息命令 
    /// 加油机将实时信息发送给 PC 机；在加油机主动方式下，加油机定时与 PC 机进行的握手通讯； 
    /// 加油机发送实时信息命令31H
    /// </summary>
    public class PumpStateChangeRequest : KaJiLianDongV11MessageTemplateBase
    {
        [Format(1, EncodingType.BIN, 1)]
        public byte SubMessageCount { get; set; }

        // minus 3 which include 
        [EnumerableFormat("DataLength", "-2", 2, EncodingType = EncodingType.BIN)]
        public List<byte> SubMessageRaw { get; set; }

        public override string ToLogString()
        {
            var result = "PumpStateChangeRequest, SubMessageCount= " + this.SubMessageCount.ToString() + "." + System.Environment.NewLine;
            var offset = 0;
            for (int i = 0; i < SubMessageCount; i++)
            {
                //1：卡插入；2：抬枪或加油中
                if (SubMessageRaw[offset] == 1)
                {
                    var restLen = SubMessageRaw[offset + 2];
                    Parser parser = new Parser();
                    var cardSubMsg = parser.Deserialize(SubMessageRaw.Skip(offset).Take(3 + restLen).ToArray(),
                        (MessageTemplateBase)Activator.CreateInstance(typeof(PumpStateChangeCardInsertedSubState))) as PumpStateChangeCardInsertedSubState;
                    offset += 3 + restLen;
                    result += "CardInserted: cardNo->" + cardSubMsg.ASN卡应用号 + ", nozzleNo->" + cardSubMsg.MZN枪号 +
                              ", cardBalance->" + cardSubMsg.BAL余额.ToString();
                }
                else if (SubMessageRaw[offset] == 2)
                {
                    Parser parser = new Parser();
                    var nozzleSubMsg = parser.Deserialize(SubMessageRaw.Skip(offset).Take(11).ToArray(),
                        (MessageTemplateBase)Activator.CreateInstance(typeof(PumpStateChangeNozzleOperatingSubState))) as PumpStateChangeNozzleOperatingSubState;
                    offset += 11;
                    result += "NozzleChanged: nozzleNo->" + nozzleSubMsg.MZN枪号 + ", vol->" + nozzleSubMsg.VOL升数 + ", amount->" + nozzleSubMsg.AMN数额
                        + ", price->" + nozzleSubMsg.PRC价格;
                }
                else
                {
                    throw new ArgumentException("只有两种状态需要上传信息。1：卡插入；2：抬枪或加油中, there're neither 1 nor 2 in msg");
                }
            }

            return result;
        }

        public List<PumpStateChangeCardInsertedSubState> StateCardInsertedSubMessages
        {
            get;
            set;
        }

        public List<PumpStateChangeNozzleOperatingSubState> StateNozzleOperatingSubMessages
        {
            get;
            set;
        }

        public static void ParseSubMessages(List<byte> SubMessageRaw, byte SubMessageCount, PumpStateChangeRequest owner)
        {
            var offset = 0;
            for (int i = 0; i < SubMessageCount; i++)
            {
                //1：卡插入；2：抬枪或加油中
                if (SubMessageRaw[offset] == 1)
                {
                    var restLen = SubMessageRaw[offset + 2];
                    Parser parser = new Parser();
                    var cardSubMsg = parser.Deserialize(SubMessageRaw.Skip(offset).Take(3 + restLen).ToArray(),
                        (MessageTemplateBase)Activator.CreateInstance(typeof(PumpStateChangeCardInsertedSubState)));
                    if (owner.StateCardInsertedSubMessages == null) owner.StateCardInsertedSubMessages = new List<PumpStateChangeCardInsertedSubState>();
                    owner.StateCardInsertedSubMessages.Add(cardSubMsg as PumpStateChangeCardInsertedSubState);
                    offset += 3 + restLen;
                }
                else if (SubMessageRaw[offset] == 2)
                {
                    Parser parser = new Parser();
                    var cardSubMsg = parser.Deserialize(SubMessageRaw.Skip(offset).Take(11).ToArray(),
                        (MessageTemplateBase)Activator.CreateInstance(typeof(PumpStateChangeNozzleOperatingSubState)));
                    if (owner.StateNozzleOperatingSubMessages == null)
                        owner.StateNozzleOperatingSubMessages = new List<PumpStateChangeNozzleOperatingSubState>();
                    owner.StateNozzleOperatingSubMessages.Add(cardSubMsg as PumpStateChangeNozzleOperatingSubState);
                    offset += 11;
                }
                else
                {
                    throw new ArgumentException("只有两种状态需要上传信息。1：卡插入；2：抬枪或加油中, there're neither 1 nor 2 in msg");
                }
            }
        }
    }
}