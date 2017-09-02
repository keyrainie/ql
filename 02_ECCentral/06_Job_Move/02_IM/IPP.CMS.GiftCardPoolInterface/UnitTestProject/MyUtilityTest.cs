using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContentMgmt.GiftCardPoolInterface;
using System.Diagnostics;
using ContentMgmt.GiftCardPoolInterface.Entities;

namespace UnitTestProject
{
    [TestClass]
    public class MyUtilityTest
    {
        [TestMethod]
        public void TestGenerateCode()
        {
            for (int i = 0; i < 1000; i++)
            {
                Debug.Write(DateTime.Now.ToString("ss.ffffff "));
                Debug.WriteLine(MyUtility.GenerateCode(i, 8));
            }
        }

        [TestMethod]
        public void TestParseCodeValue()
        {
            Assert.AreEqual<int>(0, MyUtility.ParseCodeValue("22222222"));
            Assert.AreEqual<int>(1, MyUtility.ParseCodeValue("22222223"));
            Assert.AreEqual<int>(ConstValues.CodeDimension + 1, MyUtility.ParseCodeValue("22222233"));
            Assert.AreEqual<int>(ConstValues.CodeDimension * ConstValues.CodeDimension
                + ConstValues.CodeDimension * 2 + 3
                , MyUtility.ParseCodeValue("22222345"));
        }

        [TestMethod]
        public void TestOverseaLog()
        {
            //LogHelper.WriteOverseaLog<GiftCardPoolEntity>(new GiftCardPoolEntity(),
            //    string.Empty, string.Empty, "GiftCardPoolInterface",
            //    string.Format("新增{0}条数据，花费{1}秒时间。", (ConstValues.AvailableCount - 1).ToString(),
            //        (DateTime.Now - DateTime.Now).TotalSeconds.ToString()),
            //    0.ToString());
        }

        [TestMethod]
        public void TestRefParameter()
        {
            List<int> input = new List<int>() { 3 };
            List<int> output = DoTestRefParameter(input);
            Assert.AreEqual<int>(input[0], output[0]);
        }

        private List<int> DoTestRefParameter(List<int> input)
        {
            input = new List<int>() { 1, 2 };
            return input;
        }
    }
}
