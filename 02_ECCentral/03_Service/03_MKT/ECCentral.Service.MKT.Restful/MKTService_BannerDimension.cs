using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using System.ServiceModel.Web;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.WCF;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.AppService;
using System.Data;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private IBannerDimensionQueryDA _bannerDimensionQueryDA = ObjectFactory<IBannerDimensionQueryDA>.Instance;
        private BannerDimensionAppService _bannerDimensionAppService = ObjectFactory<BannerDimensionAppService>.Instance;

        [WebInvoke(UriTemplate = "/BannerDimension/Query", Method = "POST")]
        public virtual QueryResult QueryBannerDimension(BannerDimensionQueryFilter filter)
        {
            int totalCount = 0;
            var dt = _bannerDimensionQueryDA.Query(filter, out totalCount);
            QueryResult queryResult = new QueryResult();
            queryResult.Data = dt;
            queryResult.TotalCount = totalCount;
            return queryResult;
        }

        /// <summary>
        /// 创建广告尺寸
        /// </summary>
        /// <param name="BannerDimension">广告尺寸</param>
        [WebInvoke(UriTemplate = "/BannerDimension/Create", Method = "POST")]
        public virtual void CreateBannerDimension(BannerDimension bannerLocation)
        {
            _bannerDimensionAppService.Create(bannerLocation);
        }

        /// <summary>
        /// 更新广告尺寸
        /// </summary>
        /// <param name="BannerDimension">广告尺寸</param>
        [WebInvoke(UriTemplate = "/BannerDimension/Update", Method = "PUT")]
        public virtual void UpdateBannerDimension(BannerDimension bannerLocation)
        {
            _bannerDimensionAppService.Update(bannerLocation);
        }
        
        /// <summary>
        /// 加载广告尺寸
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/BannerDimension/{sysNo}")]
        public virtual BannerDimension LoadBannerDimension(string sysNo)
        {
            int id = Convert.ToInt32(sysNo);
            return _bannerDimensionAppService.Load(id);
        }
    }
}
