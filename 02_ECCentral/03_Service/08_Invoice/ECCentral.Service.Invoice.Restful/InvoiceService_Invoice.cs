using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Invoice.Restful
{
    [ServiceContract]
    public partial class InvoiceService
    {
        #region NoBizQuery

        /// <summary>
        /// 分公司收款单-查询
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Invoice/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResultList InvoiceQuery(InvoiceQueryFilter filter)
        {
            int totalCount = 0;
            var dataSet = ObjectFactory<IInvoiceQueryDA>.Instance.Qeury(filter, out totalCount);
            return new QueryResultList()
            {
                new QueryResult(){ Data = dataSet.Tables["ResultTable"], TotalCount = totalCount},
                new QueryResult(){ Data = dataSet.Tables["InvoiceAmtTable"]}
            };
        }

        /// <summary>
        /// 分公司收款单-导出
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Invoice/Export", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult InvoiceExport(InvoiceQueryFilter filter)
        {
            int totalCount = 0;
            var dataSet = ObjectFactory<IInvoiceQueryDA>.Instance.Qeury(filter, out totalCount);
            var dataTable = dataSet.Tables["ResultTable"];

            foreach (DataRow row in dataTable.Rows.AsParallel())
            {
                if (row["OrderType"].ToString() == "4")
                {
                    row["OrderSysNo"] = row["NewOrderSysNo"];
                }
            }
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        #endregion NoBizQuery


        [WebInvoke(UriTemplate = "/Invoice/UpdateSOInvoice/{soSysNo}/{invoiceNo}/{warehouseNo}/{companyCode}", Method = "PUT")]
        public void UpdateSOInvoice(string soSysNo, string invoiceNo,string warehouseNo,string companyCode)
        {
            ObjectFactory<InvoiceAppService>.Instance.UpdateSOInvoice(int.Parse(soSysNo), invoiceNo, warehouseNo, companyCode);
        }

        /// <summary>
        /// 查询对账单
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Invoice/QueryReconciliation", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult ReconciliationQuery(ReconciliationQueryFilter filter)
        {
            int totalCount = 0;
            DataTable dt = ObjectFactory<IInvoiceQueryDA>.Instance.ReconciliationQuery(filter, out totalCount);
            return new QueryResult() { Data = dt, TotalCount = totalCount };
        }

    }
}