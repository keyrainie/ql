using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.AppService;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Customer.Restful
{
    public partial class CustomerService
    {
        #region 客服管理
        [WebInvoke(UriTemplate = "/CS/Create", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public CSInfo CreateCS(CSInfo entity)
        {
            return ObjectFactory<CSAppService>.Instance.Create(entity);
        }
        [WebInvoke(UriTemplate = "/CS/Update", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public void UpdateCS(CSInfo entity)
        {
            ObjectFactory<CSAppService>.Instance.Update(entity);
        }

        [WebInvoke(UriTemplate = "/CS/Query", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryCS(CSQueryFilter request)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<ICSQueryDA>.Instance.Query(request, out totalCount),
                TotalCount = totalCount
            };
        }
        [WebInvoke(UriTemplate = "/CS/GetCSWithDepartmentId", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<CSInfo> GetCSWithDepartmentId(int depid)
        {
            return ObjectFactory<CSAppService>.Instance.GetCSWithDepartmentId(depid);
        }

        [WebInvoke(UriTemplate = "/CS/GetAllCS", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<CSInfo> GetAllCS(string companyCode)
        {
            return ObjectFactory<CSAppService>.Instance.GetAllCS(companyCode);
        }

        [WebInvoke(UriTemplate = "/CS/GetCSByLeaderSysNo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<CSInfo> GetCSByLeaderSysNo(int leaderSysNo)
        {
            return ObjectFactory<CSAppService>.Instance.GetCSByLeaderSysNo(leaderSysNo);
        }

        #endregion
    }
}
