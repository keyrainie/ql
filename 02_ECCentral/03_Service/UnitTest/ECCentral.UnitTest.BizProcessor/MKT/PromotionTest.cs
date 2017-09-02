using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.BizProcessor;
using ECCentral.Service.SO.BizProcessor;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.BizProcessor.SO;
using ECCentral.Service.MKT.AppService.Promotion;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECCentral.UnitTest.BizProcessor.MKT
{
    [TestClass]
    public class PromotionTest
    {
        [TestMethod]
        public void TestCalculateSOPromotion()
        {
            int soSysNo = 34728751; // 附件Mapping测试 
            SOInfo soInfo = new SOProcessor().GetSOBySOSysNo(soSysNo);
            soInfo.CouponCode = "9VXU2YFXW7";
            PromotionCalculateAppService appservice = new PromotionCalculateAppService();
            appservice.CalculateSOPromotion(soInfo);
            ;
        }
    }
}
