using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using System.ServiceModel.Web;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.MKT.PageType;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        [WebGet(UriTemplate = "/PageType/{companyCode}/{channelID}/{moduleType}")]
        public virtual List<CodeNamePair> GetPageType(string companyCode, string channelID, string moduleType)
        {
            ModuleType m;
            Enum.TryParse<ModuleType>(moduleType, out m);
            return ObjectFactory<PageTypeAppService>.Instance.GetPageType(companyCode, channelID, m);
        }

        [WebGet(UriTemplate = "/PageType/GetPages/{companyCode}/{channelID}/{moduleType}/{pageTypeID}")]
        public virtual PageResult GetPage(string companyCode, string channelID, string moduleType, string pageTypeID)
        {
            ModuleType m;
            Enum.TryParse<ModuleType>(moduleType, out m);
            return ObjectFactory<PageTypeAppService>.Instance.GetPage(companyCode, channelID, m, pageTypeID);
        }

        //分页查询页面类型
        [WebInvoke(UriTemplate="/PageType/Query",Method="POST")]
        public virtual QueryResult QueryPageType(PageTypeQueryFilter filter)
        {
            var pageTypeQueryDA = ObjectFactory<IPageTypeQueryDA>.Instance;
            int totalCount;
            var data = pageTypeQueryDA.Query(filter, out totalCount);

            return new QueryResult
            {
                Data=data,
                TotalCount = totalCount
            };
        }

        private PageTypeAppService _pageTypeAppService = ObjectFactory<PageTypeAppService>.Instance;

        //插入页面类型
        [WebInvoke(UriTemplate = "/PageType/Create", Method = "POST")]
        public virtual void Create(PageType entity)
        {
            _pageTypeAppService.Create(entity);
        }

        //更新页面类型
         [WebInvoke(UriTemplate = "/PageType/Update", Method = "PUT")]
        public virtual void Update(PageType entity)
        {
            _pageTypeAppService.Update(entity);
        }

        //加载页面类型
         [WebGet(UriTemplate = "/PageType/{sysNo}")]
        public virtual PageType LoadPageType(string sysNo)
        {
            int id = Convert.ToInt32(sysNo);
            return _pageTypeAppService.Load(id); ;
        }
    }
}
