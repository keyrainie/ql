using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.BizProcessor;
using ECCentral.Service.SO.BizProcessor;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.BizProcessor.SO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.AppService;

namespace ECCentral.UnitTest.BizProcessor.MKT
{
    [TestClass]
    public class PromotionTest
    {
        [TestMethod]
        public void Test_MKT_CalculateSOPromotion()
        {
            int soSysNo = 34728751; // 附件Mapping测试 
            SOInfo soInfo = new SOProcessor().GetSOBySOSysNo(soSysNo);
            soInfo.CouponCode = "9VXU2YFXW7";
            //PromotionCalculateAppService appservice = new PromotionCalculateAppService();
            //appservice.CalculateSOPromotion(soInfo);
        }
        [TestMethod]
        public void CheckAndGivingPromotionCodeForSO()
        {
            AutorunManager.Startup(null);
            //SOInfo soInfo = new SOProcessor().CheckAndGivingPromotionCodeForSO(130008136);
            //soInfo.Merchant = new BizEntity.Common.Merchant() { SysNo = 1 };
            SOInfo soInfo = new SOInfo()
            {
                SysNo = 130004413,
                Merchant = new BizEntity.Common.Merchant()
                {
                    SysNo = 2062
                },
                BaseInfo = new SOBaseInfo()
                {
                    CreateTime = new DateTime(2014, 11, 27, 18, 0, 0),
                    CustomerSysNo = 92,
                    SOAmount = 200,
                },
                Items = new List<SOItemInfo>()
                {
                    new SOItemInfo(){ ProductID="AO_11049_966_0002", ProductSysNo=98914},
                },
            };
            new BizInteractAppService().CheckAndGivingPromotionCodeForSO(soInfo);
            AutorunManager.Shutdown(null);
        }
    }
}
