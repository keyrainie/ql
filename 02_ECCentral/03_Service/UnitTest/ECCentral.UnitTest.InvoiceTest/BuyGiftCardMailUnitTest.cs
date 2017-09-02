using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Invoice.BizProcessor;

namespace ECCentral.UnitTest.InvoiceTest
{
    [TestClass]
    public class BuyGiftCardMailUnitTest
    {
        [TestMethod]
        public void BuyGrifCardMailMethod()
        {
            KeyValueVariables keyValueVariables = new KeyValueVariables();
            keyValueVariables.Add("CustomerID", "Geeker");
            keyValueVariables.Add("Quantity", 2);
            keyValueVariables.Add("TotalAmount", 300);
            keyValueVariables.Add("ExpireYear", DateTime.Now.AddYears(2).Year);
            keyValueVariables.Add("ExpireMonth", DateTime.Now.Month);
            keyValueVariables.Add("ExpireDay", DateTime.Now.Day);
            keyValueVariables.Add("NowYear", DateTime.Now.Year);
            EmailHelper.SendEmailByTemplate("Norton.C.Li@newegg.com", "SO_ActivateElectronicGiftCard", keyValueVariables, null, "zh-CN");
        }
    }
}
