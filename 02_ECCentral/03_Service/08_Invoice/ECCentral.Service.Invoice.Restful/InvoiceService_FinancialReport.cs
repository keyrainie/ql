using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Invoice;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.AppService;
using ECCentral.BizEntity.IM;
using System.Data;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        [WebInvoke(UriTemplate = "/IncomeCostReport/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResultList IncomeCostReportQuery(IncomeCostReportQueryFilter filter)
        {
            int totalCount;
            var ds = ObjectFactory<IFinancialReportDA>.Instance.IncomeCostReportQuery(filter, out totalCount);
            return new QueryResultList()
            {
                new QueryResult() {Data = ds.Tables[0], TotalCount = totalCount},
                new QueryResult() {Data = ds.Tables[1]}
            };
        }

        [WebInvoke(UriTemplate = "/IncomeCostReport/Export", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult IncomeCostReportExport(IncomeCostReportQueryFilter filter)
        {
            int totalCount;
            var ds = ObjectFactory<IFinancialReportDA>.Instance.IncomeCostReportQuery(filter, out totalCount);
            var dataTable = ds.Tables[0];
            return new QueryResult() { Data = dataTable, TotalCount = totalCount };
        }

        [WebInvoke(UriTemplate = "/SalesStatisticsReport/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResultList SalesStatisticsReportQuery(SalesStatisticsReportQueryFilter filter)
        {
            int totalCount;
            var ds = ObjectFactory<IFinancialReportDA>.Instance.SalesStatisticsReportQuery(filter, out totalCount);
            return new QueryResultList()
            {
                new QueryResult() {Data = ds.Tables[0], TotalCount = totalCount},
                new QueryResult() {Data = ds.Tables[1]}
            };
        }

        [WebInvoke(UriTemplate = "/SalesStatisticsReport/Export", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult SalesStatisticsReportExport(SalesStatisticsReportQueryFilter filter)
        {
            int totalCount;
            var ds = ObjectFactory<IFinancialReportDA>.Instance.SalesStatisticsReportQuery(filter, out totalCount);
            var dataTable = ds.Tables[0];
            return new QueryResult() { Data = dataTable, TotalCount = totalCount };
        }

        [WebInvoke(UriTemplate = "/CouponUseedReport/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResultList CouponUseedReportQuery(CouponUsedReportFilter filter)
        {
            int totalCount;
            var ds = ObjectFactory<IFinancialReportDA>.Instance.CouponUseedReportQuery(filter, out totalCount);
            return new QueryResultList()
            {
                new QueryResult() {Data = ds.Tables[0], TotalCount = totalCount},
                new QueryResult() {Data = ds.Tables[1]}
            };
        }

        [WebInvoke(UriTemplate = "/CouponUseedReport/Export", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult CouponUseedReportExport(CouponUsedReportFilter filter)
        {
            int totalCount;
            var ds = ObjectFactory<IFinancialReportDA>.Instance.CouponUseedReportQuery(filter, out totalCount);
            var dataTable = ds.Tables[0];
            return new QueryResult() { Data = dataTable, TotalCount = totalCount };
        }
    }
}
