using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using System.Data;

namespace ECCentral.Service.Customer.Restful
{

    public partial class CustomerService
    {
        /// <summary>
        /// 获取顾客经验值历史
        /// </summary>
        [WebInvoke(UriTemplate = "/Experience/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryCustomerExperience(CustomerExperienceLogQueryFilter request)
        { 
            int totalCount;
            DataTable dataTable = ObjectFactory<ICustomerQueryDA>.Instance.QueryCustomerExperience(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 获取顾客是否是恶意用户历史
        /// </summary>
        [WebInvoke(UriTemplate = "/Experience/QueryMaliceUser", Method = "POST")]
        public QueryResult QueryMaliceCustomerExperience(int customerSysNo)
        {
            var dataTable = ObjectFactory<ICustomerQueryDA>.Instance.QueryMaliceCustomerLog(customerSysNo);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = 10
            };
        }
    }
}
