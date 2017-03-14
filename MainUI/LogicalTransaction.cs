using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageParser;

namespace MainUI
{
    public class LogicalTransaction
    {
        public static LogicalTransaction LoadFrom(PumpNotifyTransactionDoneRequest request)
        {
            var result = new LogicalTransaction()
            {
                POS_TTC = request.POS_TTC,
                T_Type = request.T_Type,
                //TIME_Raw = request.TIME_Raw,
                ASN卡应用号 = request.ASN卡应用号,
                BAL余额 = request.BAL余额,
                AMN数额 = request.AMN数额,
                CTC = request.CTC,
                TAC电子签名 = request.TAC电子签名,
                GMAC电子签名 = request.GMAC电子签名,
                PSAM_TAC灰锁签名 = request.PSAM_TAC灰锁签名,
                PSAM_ASN_PSAM应用号 = request.PSAM_ASN_PSAM应用号,
                PSAM_TID_PSAM编号 = request.PSAM_TID_PSAM编号,
                PSAM_TTC = request.PSAM_TTC,
                DS_扣款来源 = request.DS_扣款来源,
                UNIT_结算单位_方式 = request.UNIT_结算单位_方式,
                C_TYPE_卡类 = request.C_TYPE_卡类,
                VER_版本 = request.VER_版本,
                NZN_枪号 = request.NZN_枪号,
                G_CODE_油品代码 = request.G_CODE_油品代码,
                VOL_升数 = request.VOL_升数,
                PRC_成交价格 = request.PRC_成交价格,
                EMP_员工号 = request.EMP_员工号,
                V_TOT_升累计 = request.V_TOT_升累计,
                RFU_备用 = request.RFU_备用,
                T_MAC_终端数据认证码 = request.T_MAC_终端数据认证码
            };

            result.TIME = request.GetTime();

            return result;
        }

        /// <summary>
        /// 由POS产生的终端交易序号，每笔交易自动加一 
        /// </summary>
        public int POS_TTC { get; set; }

        //public enum LogicalTransaction交易类型
        //{
        //    卡错,
        //    使用后台黑or白or名单_使用油机内黑or白or名单,
        //    扣款签名有效_无法判断用户卡TAC标记是否清除,
        //    正常加油,
        //    逃卡,
        //    错卡,
        //    补扣,
        //    补充,
        //    员工上班_每条枪一条记录_对于一个IC卡终端有多条油枪时_每枪一条记录,
        //    员工下班_每条枪一条记录_一个IC卡终端有多条油枪时_每枪一条记录,
        //    非卡机联动加油,
        //    对油价信息的回应,
        //    卡片交易出错记录_出错后在TAC即电子签名字段填写出错原因
        //}

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
        public byte T_Type { get; set; }

        public string Get_Full_T_Type()
        {
            string result = "";
            if (this.T_Type.GetBit(7) == 1) result += "卡错";
            if (this.T_Type.GetBit(6) == 0) result += "使用后台黑(白)名单"; else result += "使用油机内黑(白)名单";
            if (this.T_Type.GetBit(4) == 1) result += "扣款签名有效";
            var mask = this.T_Type & 15;
            if (mask == 0) result += "正常加油";
            if (mask == 1) result += "逃卡";
            if (mask == 2) result += "错卡";
            if (mask == 3) result += "补扣";
            if (mask == 4) result += "补充";
            if (mask == 5) result += "员工上班";
            if (mask == 6) result += "员工下班";
            if (mask == 7) result += "非卡机联动加油";
            if (mask == 8) result += "对油价信息的回应";
            if (mask == 9) result += "卡片交易出错记录";

            return result;
        }
        /// <summary>
        /// 日期及时间 
        /// </summary>
        public DateTime TIME { get; set; }

        //public DateTime GetTime()
        //{
        //    CultureInfo culture = new CultureInfo("zh-CN");
        //    var dd = this.TIME_Raw.ToString();
        //    var dt = DateTime.ParseExact(dd, "yyyyMMddHHmmss", culture);
        //    return dt;
        //}

        //public void SetTime(DateTime dateTime)
        //{
        //    var str = dateTime.ToString("yyyyMMddHHmmss");
        //    this.TIME_Raw = long.Parse(str);
        //}

        public string ASN卡应用号 { get; set; }

        /// <summary>
        /// 正常成交：成交后余额； 
        /// 正常成交：成交后余额； 逃卡或卡错：交易前的原额，单位同UNIT的规定 。
        /// </summary>
        public int BAL余额 { get; set; }

        public int AMN数额 { get; set; }

        /// <summary>
        /// 卡交易序号 
        /// </summary>
        public int CTC { get; set; }

        /// <summary>
        /// 电子签名 ,  加油、补扣、补充交易时为TAC，逃卡时为GTAC 
        /// </summary>
        public int TAC电子签名 { get; set; }

        /// <summary>
        /// 解灰认证码 , 逃卡/卡错时有效 
        /// </summary>
        public int GMAC电子签名 { get; set; }


        /// <summary>
        /// 解灰认证码 , 逃卡/卡错时有效 
        /// </summary>
        public int PSAM_TAC灰锁签名 { get; set; }

        /// <summary>
        /// PSAM卡号，右8个字节用于PSAM-TAC的密钥分散算 法
        /// </summary>
        public string PSAM_ASN_PSAM应用号 { get; set; }

        /// <summary>
        /// 用于TAC，GTAC运算 
        /// </summary>
        public List<byte> PSAM_TID_PSAM编号 { get; set; }

        /// <summary>
        ///  由PSAM卡产生的终端交易序号，签名以此为准
        /// </summary>
        public int PSAM_TTC { get; set; }

        /// <summary>
        ///  0=石油卡电子油票； 1=石油积分区积分； 2=金融卡电子钱包； 3=金融卡电子存折
        /// </summary>
        public byte DS_扣款来源 { get; set; }

        /// <summary>
        /// Bit1-0 (单位) =0:金额(分); =1:点数(0.01点) Bit7-4 (方式) =0:现金;  =1:油票; =2:记帐 =3:银行卡;=4:其他; =5:其他1
        /// </summary>
        public byte UNIT_结算单位_方式 { get; set; }

        /// <summary>
        /// b0= 0 = 石化规范卡；1 = PBOC金融卡； 
        /// </summary>
        public byte C_TYPE_卡类 { get; set; }

        /// <summary>
        /// b7-4:卡密钥索引号；b3-0：卡密钥版本号 
        /// </summary>
        public byte VER_版本 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte NZN_枪号 { get; set; }

        public string G_CODE_油品代码 { get; set; }

        public int VOL_升数 { get; set; }

        public int PRC_成交价格 { get; set; }

        /// <summary>
        ///  255：没有员工上班
        /// </summary>
        public byte EMP_员工号 { get; set; }


        public int V_TOT_升累计 { get; set; }

        /// <summary>
        ///  全部填零
        /// </summary>
        public List<byte> RFU_备用 { get; set; }

        /// <summary>
        /// PBOC 标准3DES算法 计算范围：从POS-TTC开始（含POS-TTC）
        /// </summary>
        public int T_MAC_终端数据认证码 { get; set; }
    }
}
