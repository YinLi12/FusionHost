using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageParserTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var r = MessageParser.ExtentionMethod.GetBit(28, 3);
            Assert.AreEqual(1, r, "failed this case 28,3");

            r = MessageParser.ExtentionMethod.GetBit(28, 2);
            Assert.AreEqual(1, r, "failed this case 28,2");

            r = MessageParser.ExtentionMethod.GetBit(28, 4);
            Assert.AreEqual(1, r, "failed this case 28,4");

            r = MessageParser.ExtentionMethod.GetBit(128, 3);
            Assert.AreEqual(0, r, "failed this case 128,3");

            r = MessageParser.ExtentionMethod.GetBit(128, 3);
            Assert.AreEqual(0, r, "failed this case 128, 3");

            r = MessageParser.ExtentionMethod.GetBit(128, 4);
            Assert.AreEqual(0, r, "failed this case 128, 4");
        }

        [TestMethod]
        public void ToBCDTestMethod1()
        {
            var r = MessageParser.ExtentionMethod.ToBCD("1");
            Assert.AreEqual(1, r.Length, "1 Length");
            Assert.AreEqual(1, r.First(), "1");

            r = MessageParser.ExtentionMethod.ToBCD("12");
            Assert.AreEqual(1, r.Length, "12 Length");
            Assert.AreEqual(18, r.First(), "12");

            r = MessageParser.ExtentionMethod.ToBCD("123");
            Assert.AreEqual(2, r.Length, "123 Length");
            Assert.AreEqual(1, r.First(), "123");
            Assert.AreEqual(35, r.Skip(1).First(), "12");

            r = MessageParser.ExtentionMethod.ToBCD("35789");
            Assert.AreEqual(3, r.Length, "35789 Length");
            Assert.AreEqual(3, r.First(), "35789");
            Assert.AreEqual(87, r.Skip(1).First(), "35789");
            Assert.AreEqual(137, r.Skip(2).First(), "35789");
        }
    }
}
