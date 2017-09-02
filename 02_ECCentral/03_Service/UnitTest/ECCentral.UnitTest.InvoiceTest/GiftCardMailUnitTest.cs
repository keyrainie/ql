using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECCentral.UnitTest.BizProcessor.Invoice;
using ECCentral.Service.Utility;

namespace ECCentral.UnitTest.InvoiceTest
{
    [TestClass]
    public class GiftCardMailUnitTest
    {
        [TestMethod]
        public void GiftCardMailMethod()
        {
            KeyValueVariables vars = new KeyValueVariables();
            vars.Add("CustomerID", "geeker");
            vars.Add("TotalValue", 500);
            vars.Add("ExpireYear", DateTime.Now.AddYears(2).Year);
            vars.Add("ExpireMonth", DateTime.Now.Month);
            vars.Add("ExpireDay", DateTime.Now.Day);
            vars.Add("Year", DateTime.Now.Year);
            EmailHelper.SendEmailByTemplate("Norton.C.Li@newegg.com", "Refund_GiftCard_Notify", vars, null,"zh-CN");
        }
    }
}
