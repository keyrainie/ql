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

        /// <summary>
        /// 查询POS支付的订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/POSPay/QueryConfirmList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResultList QueryPOSPay(POSPayQueryFilter request)
        {
            int totalCount;
            var dataSet = ObjectFactory<IPOSPayQueryDA>.Instance.QueryPOSPayConfirmList(request, out totalCount);
            var list = new QueryResultList();
            list.Add(new QueryResult { Data = dataSet.Tables[0], TotalCount = totalCount });
            list.Add(new QueryResult { Data = dataSet.Tables[1], TotalCount = 2 });
            //return new QueryResultList()
            //{
            //    new QueryResult(){ Data = dataSet.Tables[0], TotalCount = totalCount},
            //    new QueryResult(){ Data = dataSet.Tables[1], TotalCount = 2}
            //};
            return list;
        }

        #endregion NoBizQuery
    }
}