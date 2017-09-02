using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.AppService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECCentral.UnitTest.BizProcessor.Customer
{
    [TestClass]
    public class CustomerTest
    {
        [TestMethod]
        public void CustomerIsExists()
        {
            var b1 = new CustomerAppService().IsExists(3913400);
            var b2 = new CustomerAppService().IsExists(123123);
        }
    }
}
