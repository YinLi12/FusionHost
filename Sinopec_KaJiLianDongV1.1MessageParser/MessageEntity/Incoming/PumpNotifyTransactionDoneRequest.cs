using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageParser
{
    public class PumpNotifyTransactionDoneRequest : KaJiLianDongV11MessageTemplateBase
    {
        //[Format(1, EncodingType.BIN, 0)]
        //public byte POS_P终端机身号 { get; set; }

        /// <summary>
        /// 由POS产生的终端交易序号，每笔交易自动加一 
        /// </summary>
        [Format(4, EncodingType.BIN, 1)]
        public int POS_TTC { get; set; }

        /// <summary>
        ///  交易类型 
        ///  b7=1：卡错； 
        ///  b6=0/1：使用后台黑(白)名单/使用油机内黑(白)名单 
        ///  b4=1：扣款签名有效（无法判断用户卡 TAC 标记是否清除）； 
        ///  b3-b0：0=正常加油； 
        ///         1=逃卡； 
        ///         2=错卡； 
        ///         3=补扣； 
        ///         4=补充 
        ///         5=员工上班（每条枪一条记录；对于一个IC卡终端有多条油枪时，每枪一条记录） 
        ///         6=员工下班（每条枪一条记录；一个IC卡终端有多条油枪时，每枪一条记录） 
        ///         7=非卡机联动加油 
        ///         8=对油价信息的回应 
        ///         9=卡片交易出错记录（出错后在TAC即电子签名字段填写出错原因，出错原因代码见附录9） 
        /// </summary>
        [Format(1, EncodingType.BIN, 2)]
        public byte T_Type { get; set; }

        /// <summary>
        /// 日期及时间 
        /// </summary>
        [Format(7, EncodingType.BcdString, 3)]
        public long TIME_Raw { get; private set; }

        public DateTime GetTime()
        {
            CultureInfo culture = new CultureInfo("zh-CN");
            var dd = this.TIME_Raw.ToString();
            var dt = DateTime.ParseExact(dd, "yyyyMMddHHmmss", culture);
            return dt;
        }

        public void SetTime(DateTime dateTime)
        {
            var str = dateTime.ToString("yyyyMMddHHmmss");
            this.TIME_Raw = long.Parse(str);
        }

        [Format(10, EncodingType.BcdString, 4)]
        public string ASN卡应用号 { get; set; }

        /// <summary>
        /// 正常成交：成交后余额； 
        /// 正常成交：成交后余额； 逃卡或卡错：交易前的原额，单位同UNIT的规定 。
        /// </summary>
        [Format(4, EncodingType.BIN, 5)]
        public int BAL余额 { get; set; }

        [Format(3, EncodingType.BIN, 6)]
        public int AMN数额 { get; set; }

        /// <summary>
        /// 卡交易序号 
        /// </summary>
        [Format(2, EncodingType.BIN, 7)]
        public int CTC { get; set; }

        /// <summary>
        /// 电子签名 ,  加油、补扣、补充交易时为TAC，逃卡时为GTAC 
        /// </summary>
        [Format(4, EncodingType.BIN, 8)]
        public int TAC电子签名 { get; set; }

        /// <summary>
        /// 解灰认证码 , 逃卡/卡错时有效 
        /// </summary>
        [Format(4, EncodingType.BIN, 9)]
        public int GMAC电子签名 { get; set; }


        /// <summary>
        /// 解灰认证码 , 逃卡/卡错时有效 
        /// </summary>
        [Format(4, EncodingType.BIN, 10)]
        public int PSAM_TAC灰锁签名 { get; set; }

        /// <summary>
        /// PSAM卡号，右8个字节用于PSAM-TAC的密钥分散算 法
        /// </summary>
        [Format(10, EncodingType.BcdString, 11)]
        public string PSAM_ASN_PSAM应用号 { get; set; }

        /// <summary>
        /// 用于TAC，GTAC运算 
        /// </summary>
        [EnumerableFormat(6, 12, EncodingType = EncodingType.BIN)]
        public List<byte> PSAM_TID_PSAM编号 { get; set; }

        /// <summary>
        ///  由PSAM卡产生的终端交易序号，签名以此为准
        /// </summary>
        [Format(4, EncodingType.BIN, 13)]
        public int PSAM_TTC { get; set; }

        /// <summary>
        ///  0=石油卡电子油票； 1=石油积分区积分； 2=金融卡电子钱包； 3=金融卡电子存折
        /// </summary>
        [Format(1, EncodingType.BIN, 14)]
        public byte DS_扣款来源 { get; set; }

        /// <summary>
        /// Bit1-0 (单位) =0:金额(分); =1:点数(0.01点) Bit7-4 (方式) =0:现金;  =1:油票; =2:记帐 =3:银行卡;=4:其他; =5:其他1
        /// </summary>
        [Format(1, EncodingType.BIN, 15)]
        public byte UNIT_结算单位_方式 { get; set; }

        /// <summary>
        /// b0= 0 = 石化规范卡；1 = PBOC金融卡； 
        /// </summary>
        [Format(1, EncodingType.BIN, 16)]
        public byte C_TYPE_卡类 { get; set; }

        /// <summary>
        /// b7-4:卡密钥索引号；b3-0：卡密钥版本号 
        /// </summary>
        [Format(1, EncodingType.BIN, 17)]
        public byte VER_版本 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Format(1, EncodingType.BIN, 18)]
        public byte NZN_枪号 { get; set; }

        [Format(2, EncodingType.BcdString, 19)]
        public string G_CODE_油品代码 { get; set; }

        [Format(3, EncodingType.BIN, 20)]
        public int VOL_升数 { get; set; }

        [Format(2, EncodingType.BIN, 21)]
        public int PRC_成交价格 { get; set; }

        /// <summary>
        ///  255：没有员工上班
        /// </summary>
        [Format(1, EncodingType.BIN, 22)]
        public byte EMP_员工号 { get; set; }


        [Format(4, EncodingType.BIN, 23)]
        public int V_TOT_升累计 { get; set; }

        /// <summary>
        ///  全部填零
        /// </summary>
        [EnumerableFormat(11, 24, EncodingType = EncodingType.BIN)]
        public List<byte> RFU_备用 { get; set; }

        /// <summary>
        /// PBOC 标准3DES算法 计算范围：从POS-TTC开始（含POS-TTC）
        /// </summary>
        [Format(4, EncodingType.BIN, 25)]
        public int T_MAC_终端数据认证码 { get; set; }
    }
}
