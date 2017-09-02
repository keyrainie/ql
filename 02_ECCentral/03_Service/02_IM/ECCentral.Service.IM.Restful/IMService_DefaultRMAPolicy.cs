using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using System.Linq.Expressions;
using System.Data;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.AppService;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        [WebInvoke(UriTemplate = "/DefaultRMAPolicy/GetDefaultRMAPolicyByQuery", Method = "POST")]
        public virtual QueryResult GetDefaultRMAPolicyByQuery(DefaultRMAPolicyFilter query)
        {
            int totalCount;
            DataTable dataTable = ObjectFactory<IDefaultRMAPolicy>
                .Instance.GetDefaultRMAPolicyByQuery(query, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/DefaultRMAPolicy/DefaultRMAPolicyInfoAdd", Method = "PUT")]
        public void DefaultRMAPolicyInfoAdd(DefaultRMAPolicyInfo defaultRMAPolicy)
        {
            ObjectFactory<DefaultRMAPolicyService>.Instance
                .DefaultRMAPolicyInfoAdd(defaultRMAPolicy);          
        }

        [WebInvoke(UriTemplate = "/DefaultRMAPolicy/DelDelDefaultRMAPolicyBySysNoBySysNos", Method = "DELETE")]
        public void DelDelDefaultRMAPolicyBySysNoBySysNos(List<DefaultRMAPolicyInfo> defaultRMAPolicyInfos)
        {
            ObjectFactory<DefaultRMAPolicyService>.Instance
               .DelDelDefaultRMAPolicyBySysNoBySysNos(defaultRMAPolicyInfos);          
        }

        [WebInvoke(UriTemplate = "/DefaultRMAPolicy/UpdateDefaultRMAPolicyBySysNo", Method = "PUT")]
        public void UpdateDefaultRMAPolicyBySysNo(DefaultRMAPolicyInfo defaultRMAPolicyInfos)
        {
            ObjectFactory<DefaultRMAPolicyService>.Instance
            .UpdateDefaultRMAPolicyBySysNo(defaultRMAPolicyInfos);        
        }
    }
}
