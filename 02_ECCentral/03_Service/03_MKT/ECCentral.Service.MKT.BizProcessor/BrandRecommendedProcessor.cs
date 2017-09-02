using System;
using System.Collections.Generic;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(BrandRecommendedProcessor))]
    public class BrandRecommendedProcessor
    {

        private readonly IBrandRecommendedDA brandRecommendedDA = ObjectFactory<IBrandRecommendedDA>.Instance;
        public void UpdateBrandRecommended(BrandRecommendedInfo info)
        {
            brandRecommendedDA.UpdateBrandRecommended(info);
        }

        public void CreateBrandRecommended(BrandRecommendedInfo info)
        {
            //if (brandRecommendedDA.CheckExistBrandRecommended(info))
            //{
            //    throw new BizException("已存在相同的推荐类型或前台类型！");
            //}
            //else
            //{
            //    brandRecommendedDA.CreateBrandRecommended(info);
            //}
            brandRecommendedDA.CreateBrandRecommended(info);
        }
    }
}
