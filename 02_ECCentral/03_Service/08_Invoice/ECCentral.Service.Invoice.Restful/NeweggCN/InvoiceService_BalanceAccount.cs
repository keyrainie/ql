using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        #region NoBizQuery

        [WebInvoke(UriTemplate = "/BalanceAccount/Query", Method = "POST")]
        public QueryResultList QueryBalanceAccount(BalanceAccountQueryFilter request)
        {
            int totalCount;
            var dataSet = ObjectFactory<IBalanceAccountQueryDA>.Instance.Query(request, out totalCount);
            return new QueryResultList()
            {
                new QueryResult(){ Data = dataSet.Tables["DataResult"], TotalCount = totalCount},
                new QueryResult(){ Data = dataSet.Tables["StatisticResult"]}
            };
        }


        [WebInvoke(UriTemplate = "/BalanceAccount/Export", Method = "POST")]
        public QueryResult ExportBalanceAccount(BalanceAccountQueryFilter request)
        {
            int totalCount;
            var dataSet = ObjectFactory<IBalanceAccountQueryDA>.Instance.Query(request, out totalCount);

            return new QueryResult() { Data = dataSet.Tables["DataResult"], TotalCount = totalCount };
            
        }

        #endregion NoBizQuery
    }
}