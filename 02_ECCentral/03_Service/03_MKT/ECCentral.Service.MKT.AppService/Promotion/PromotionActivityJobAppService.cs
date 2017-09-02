using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;

namespace ECCentral.Service.MKT.AppService.Promotion
{
    [VersionExport(typeof(PromotionActivityJobAppService))]
    public class PromotionActivityJobAppService
    {
        public void PromotionActivityCheck()
        {
            ObjectFactory<PromotionActivityJobProcessor>.Instance.PromotionActivityProcess();
        }
    }
}
