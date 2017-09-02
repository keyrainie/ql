using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.BizProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECCentral.UnitTest.BizProcessor.Invoice
{
    [TestClass]
    public class PostPayTest
    {
        [TestMethod]
        public void Test_GetValidPostPayBySOSysNo()
        {
            int soSysNo = 767204;
            PostPayInfo entity = new PostPayProcessor().GetValidPostPayBySOSysNo(soSysNo);
            Console.WriteLine(entity.SOSysNo);
            Assert.AreEqual(soSysNo, entity.SOSysNo.Value);
        }
    }
}