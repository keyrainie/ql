using System.Collections.Generic;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(BrandRecommendedAppService))]
    public class BrandRecommendedAppService
    {
        public void UpdateBrandRecommended(BrandRecommendedInfo info)
        {
            ObjectFactory<BrandRecommendedProcessor>.Instance.UpdateBrandRecommended(info);
        }

        public void CreateBrandRecommended(BrandRecommendedInfo info)
        {
            ObjectFactory<BrandRecommendedProcessor>.Instance.CreateBrandRecommended(info);
        }
    }
}
