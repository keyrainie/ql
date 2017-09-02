using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECCentral.UnitTest.BizProcessor.Invoice
{
    [TestClass]
    public class MailTest
    {
        [TestMethod]
        public void Test_GiftCardMailTest()
        {
            KeyValueVariables vars = new KeyValueVariables();
            vars.Add("$$CustomerID", "Poseidon.Y.Tong");
            vars.Add("$$TotalValue", "230.00");
            vars.Add("$$ExpireDate", DateTime.Now.AddYears(2).ToString("yyyy-MM-dd"));
            vars.Add("$$LinkToMyAccount", "http://www.newegg.com.cn/Customer/MyGiftCard.aspx");

           // var html = ObjectFactory<IEmailBizInteract>.Instance.GetMailTemplatesHtml("Refund_GiftCard_Notify", vars, null, "zh-CN");
        }
    }
}