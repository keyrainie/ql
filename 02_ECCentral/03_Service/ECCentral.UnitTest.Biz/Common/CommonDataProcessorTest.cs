using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.BizProcessor;
using ECCentral.Service.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECCentral.UnitTest.BizProcessor.Common
{
    [TestClass]
    public class CommonDataProcessorTest
    {
        [TestMethod]
        public void Test_Common_GetUserSysNoByUserAccount()
        {
            // In db, the user whose login name is jc59 has the sysno : 990
            int? sysNo = new CommonDataProcessor().GetUserSysNo("jc59");
            Console.WriteLine(sysNo.ToString());
            Assert.AreEqual(990, sysNo.Value);

            // For not exist login name, return 0 as sysNo
            int? notExist = new CommonDataProcessor().GetUserSysNo("test");
            Console.WriteLine(notExist.ToString());
            Assert.AreEqual(0, notExist.Value);
        }

        [TestMethod]
        public void Test_Common_GetStockListOfCompany()
        {
            var list = new CommonDataProcessor().GetStockList("8601");
            Console.WriteLine(list.Count);
            foreach (var s in list)
            {
                Console.WriteLine(s.StockID + " : " + s.StockName);
            }
            Assert.AreEqual(13, list.Count);
        }

        [TestMethod]
        public void Test_Common_GetMultiLanguage()
        {
            //ECCentral.BizEntity.ProductGroupContentLang lang = new BizEntity.ProductGroupContentLang() { SysNo = 1 };
            //var list = new MultiLanguageProcessor().GetMultiLanguage(lang);
            //Console.WriteLine(list.Count);
            //foreach (var s in list)
            //{
            //    Console.WriteLine(s.StockID + " : " + s.StockName);
            //}
            //Assert.AreEqual(13, list.Count);
            Assert.AreEqual(13, 12);
        }

        //[TestMethod]
        //public void Test_Common_GetProductTariffDicByProductSysNoList()
        //{
        //    List<int> productSysNoList = new List<int>();

        //    productSysNoList.Add(98090);
        //    productSysNoList.Add(98091);
        //    productSysNoList.Add(98092);
        //    productSysNoList.Add(98093);
        //    productSysNoList.Add(98094);

        //    var result = ObjectFactory<TariffProcessor>.Instance.GetProductTariffDicByProductSysNoList(productSysNoList);
        //    Assert.AreEqual(result.Count, 5);
        //}
    }
}
