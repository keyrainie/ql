using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.BizProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECCentral.UnitTest.BizProcessor.Common
{
    [TestClass]
    public class CommonDataProcessorTest
    {
        [TestMethod]
        public void Test_Get_User_SysNo_By_User_Account()
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
        public void Test_Get_Stock_List_Of_Company()
        {
            var list = new CommonDataProcessor().GetStockList("8601");
            Console.WriteLine(list.Count);
            foreach (var s in list)
            {
                Console.WriteLine(s.StockID + " : " + s.StockName);
            }
            Assert.AreEqual(13, list.Count);
        }
    }
}
