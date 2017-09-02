using System.Collections.Generic;
using System.ServiceModel.Web;
using ECCentral.BizEntity.Customer;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Customer.AppService;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.Customer.Restful.RequestMsg;

namespace ECCentral.Service.Customer.Restful
{
    [RestServiceBehavior]
    public partial class CustomerService
    {
        ICustomerVisitQueryDA CustomerVisitQueryDA = ObjectFactory<ICustomerVisitQueryDA>.Instance;
        CustomerVisitAppService CustomerVisitAppService = ObjectFactory<CustomerVisitAppService>.Instance;
        ICustomerVisitDA CustomerVisitDA = ObjectFactory<ICustomerVisitDA>.Instance;

        [WebInvoke(UriTemplate = "/Visit/Query", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryCustomerVisit(CustomerVisitQueryFilter request)
        {
            int totalCount = 0;
            return new QueryResult()
            {
                Data = CustomerVisitQueryDA.Query(request, out totalCount),
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 取得回访日志列表
        /// </summary>
        /// <param name="queryType">1.值为"CustomerVisitLogs"表示取得客户回访日志，对应sysNo表示客户系统编号
        /// 2.值为"OrderVisitLogs"表示取得客户订单回访日志，对应sysNo表示客户系统编号</param>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Visit/Load/{queryType}/{sysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<VisitLog> QueryCustomerVisitLogs(string queryType, string sysNo)
        {
            switch (queryType)
            {
                case "CustomerVisitLogs":
                    {
                        int customerSysNo;
                        if (int.TryParse(sysNo, out customerSysNo))
                        {
                            return CustomerVisitAppService.GetCustomerVisitLogsByCustomerSysNo(customerSysNo);
                        }
                        break;
                    }
                case "OrderVisitLogs":
                    {
                        int customerSysNo;
                        if (int.TryParse(sysNo, out customerSysNo))
                        {
                            return CustomerVisitAppService.GetOrderVisitLogsByCustomerSysNo(customerSysNo);
                        }
                        break;
                    }
            }
            return null;
        }

        /// <summary>
        /// 取得客户回访详细信息
        /// </summary>
        /// <param name="visitSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Visit/Load/{visitSysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public CustomerVisitDetailReq GetVisitDetailByVisitSysNo(string visitSysNo)
        {
            int sysNo;
            if (int.TryParse(visitSysNo, out sysNo))
            {
                CustomerVisitDetailReq detail = new CustomerVisitDetailReq()
                {
                    VisitInfo = CustomerVisitDA.GetVisitCustomerBySysNo(sysNo),
                    MaintenanceLogs = CustomerVisitAppService.GetOrderVisitLogsByVisitSysNo(sysNo),
                    VisitLogs = CustomerVisitAppService.GetCustomerVisitLogsByVisitSysNo(sysNo)
                };
                return detail;
            }
            return null;
        }
        /// <summary>
        /// 添加客户回访日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Visit/AddLog", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public VisitLog AddVisitLog(VisitLog log)
        {
            if (log.SOSysNo.HasValue && log.SOSysNo.Value > 0)
            {
                return CustomerVisitAppService.AddOrderVisitLog(log);
            }
            else
            {
                return CustomerVisitAppService.AddCustomerVisitLog(log);
            }
        }

        /// <summary>
        /// 获取客户维护（订单号）
        /// </summary>
        /// <param name="customerSysNo">顾客编号</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Visit/LoadSoSysNo/{customerSysNo}/{companyCode}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<VisitOrder> GetVisitOrderByCustomerSysNo(string customerSysNo,string companyCode)
        {
            int CustomerSysNo = int.TryParse(customerSysNo, out CustomerSysNo) ? CustomerSysNo : 0;
            return CustomerVisitDA.GetVisitOrderByCustomerSysNo(CustomerSysNo, companyCode);
        }
    }
}