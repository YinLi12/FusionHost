using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MessageParser;
using MessageParser.MessageEntity.Outgoing.BlackAndWhiteList;

namespace XmlFileGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateFuelPriceList();
        }

        public static void GenerateStationInfo()
        {
            var result = new PumpStationInfo();
            result.Ver = 0xDF;
            result.City_地市代码 = 11;
            result.Prov_省代号 = 11;
            result.Superior_上级单位代号 = "11111111";
            result.S_ID_加油站ID = "11111111";
            result.POS_P_通讯终端逻辑编号 = 0;
#warning where this configuration info come from?
            result.GUN_N_油枪数 = 4;
#warning where this configuration info number come from?
            result.MZN_单个油枪 = new List<byte>() { 1, 2, 3, 4 };

            XmlSerializer mySerializer = new
                XmlSerializer(typeof(PumpStationInfo));
            // To write to a file, create a StreamWriter object.  
            StreamWriter myWriter = new StreamWriter("Station.xml");
            mySerializer.Serialize(myWriter, result);
            myWriter.Close();
        }

        public static void GenerateWhiteList()
        {
            var result = new WhiteList(0x8E, DateTime.Now.Subtract(new TimeSpan(366, 1, 1, 1)), DateTime.Now.AddYears(1), BlackAndWhiteListBase.BlackListEffectiveAreaEnum.本省黑名单, 11, 1);
            //result.名单数量 = 51;

            #region

            result.CardSerialNumbers = new List<CardSerialNumber>();
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000419900800001084", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000001", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000002", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000003", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000004", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000005", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000006", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000007", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000008", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000009", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000010", });

            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000011", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000012", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000013", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000014", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000015", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000016", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000017", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000018", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000019", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000020", });

            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000021", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000022", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000023", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000024", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000025", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000026", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000027", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000028", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000029", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000030", });

            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000031", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000032", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000033", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000034", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000035", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000036", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000037", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000038", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000039", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000040", });

            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000041", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000042", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000043", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000044", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000045", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000046", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000047", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000048", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000049", });
            result.CardSerialNumbers.Add(new CardSerialNumber() { SerialNumber = "01000411100100000050", });

            #endregion

            XmlSerializer mySerializer = new
                XmlSerializer(typeof(WhiteList));
            // To write to a file, create a StreamWriter object.  
            StreamWriter myWriter = new StreamWriter("WhiteList.xml");
            mySerializer.Serialize(myWriter, result);
            myWriter.Close();
        }

        public static void GenerateFuelPriceList()
        {
            var result = new FuelPriceList(0xF8);
            result.Set_V_D_andT_新油品油价生效时间(DateTime.Now.Subtract(new TimeSpan(20, 1, 1, 1)));
            result.FieldNum_记录数 = 4;
            result.当前油品油价记录List = new List<FuelPriceRecord>();
            result.当前油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 1,
                O_Type_油品代码 = "1031",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x02B0) }
            });
            result.当前油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 4,
                O_Type_油品代码 = "1031",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x02B0) }
            });
            result.当前油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 2,
                O_Type_油品代码 = "1011",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x024C) }
            });
            result.当前油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 3,
                O_Type_油品代码 = "1011",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x024C) }
            });


            result.新油品油价记录List = new List<FuelPriceRecord>();
            result.新油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 1,
                O_Type_油品代码 = "1031",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x02B0) }
            });
            result.新油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 4,
                O_Type_油品代码 = "1031",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x02B0) }
            });
            result.新油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 2,
                O_Type_油品代码 = "1011",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x024C) }
            });
            result.新油品油价记录List.Add(new FuelPriceRecord()
            {
                NZN_枪号 = 3,
                O_Type_油品代码 = "1011",
                Den_密度 = 0,
                PRC_n_价格数目 = 1,
                PrcList = new List<Price>() { new Price(0x024C) }
            });

            XmlSerializer mySerializer = new
                XmlSerializer(typeof(FuelPriceList));
            // To write to a file, create a StreamWriter object.  
            StreamWriter myWriter = new StreamWriter("FuelPriceList.xml");
            mySerializer.Serialize(myWriter, result);
            myWriter.Close();
        }
    }
}
