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

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        [WebInvoke(UriTemplate = "/HomePageSection/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryHomePageSection(HomePageSectionQueryFilter filter)
        {
            int totalCount;
            QueryResult result = new QueryResult();
            result.Data =ObjectFactory<IHomePageSectionQueryDA>.Instance.Query(filter,out totalCount);
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 创建
        /// </summary>
        [WebInvoke(UriTemplate = "/HomePageSection/Create", Method = "POST")]
        public virtual void CreateHomePageSectionInfo(HomePageSectionInfo item)
        {
            ObjectFactory<HomePageSectionAppService>.Instance.Create(item);
        }

        /// <summary>
        /// 更新
        /// </summary>
        [WebInvoke(UriTemplate = "/HomePageSection/Update", Method = "PUT")]
        public virtual void UpdateHomePageSectionInfo(HomePageSectionInfo item)
        {
            ObjectFactory<HomePageSectionAppService>.Instance.Update(item);
        }

        /// <summary>
        /// 加载一个信息
        /// </summary>
        [WebGet(UriTemplate = "/HomePageSection/{sysNo}")]
        public virtual HomePageSectionInfo LoadHomePageSectionInfo(string sysNo)
        {
            int id = Convert.ToInt32(sysNo);
            return ObjectFactory<HomePageSectionAppService>.Instance.Load(id);
        }
    }
}
