using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.QueryFilter.Customer;

namespace ECCentral.Service.Customer.Restful
{
    public partial class CustomerService
    {
        [WebInvoke(UriTemplate = "/PrePay/QueryPrePayLogIncome", Method = "POST")]
        public QueryResult QueryPrePayLogIncome(PrePayQueryFilter query)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<IPrePayQueryDA>.Instance.QueryPrePayLogIncome(query, out totalCount),
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/PrePay/QueryPrePayLogPayment", Method = "POST")]
        public QueryResult QueryPrePayLogPayment(PrePayQueryFilter query)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<IPrePayQueryDA>.Instance.QueryPrePayLogPayment(query, out totalCount),
                TotalCount = totalCount
            };
        }
    }
}
