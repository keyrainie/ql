using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Customer.Restful.RequestMsg;
using ECCentral.Service.Customer.AppService;

namespace ECCentral.Service.Customer.Restful
{
    public partial class CustomerService
    {
        [WebInvoke(UriTemplate = "/RefundReqest/Query", Method = "POST")]
        public QueryResult QueryRefundReqest(RefundRequestQueryFilter query)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<IRefundRequestQueryDA>.Instance.Query(query, out totalCount),
                TotalCount = totalCount
            };
        }
        [WebInvoke(UriTemplate = "/RefundReqest/Audit", Method = "PUT")]
        public virtual void Audit(RefundAuditReq entity)
        {
            ObjectFactory<RefundRequestAppService>.Instance.Audit(entity.RefundRequestList,entity.Status,entity.Memo);
        }
    }
}
