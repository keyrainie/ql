using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        /// <summary>
        /// 供应商问题发票库（非按供应商汇总）查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/POVendorInvoice/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryPOVendorInvoice(POVendorInvoiceQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IPOVendorInvoiceQueryDA>.Instance.QueryPOVendorInvoice(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 供应商问题发票库（按供应商汇总）查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/POVendorInvoice/QueryByVendor", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryTotalAmountByVendor(POVendorInvoiceQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IPOVendorInvoiceQueryDA>.Instance.QueryTotalAmountByVendor(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 创建
        /// </summary>
        [WebInvoke(UriTemplate = "/POVendorInvoice/Create", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public void CreatePOVendorInvoice(POVendorInvoiceInfo input)
        {
            ObjectFactory<POVendorInvoiceAppService>.Instance.Create(input);
        }

        /// <summary>
        /// 更新
        /// </summary>
        [WebInvoke(UriTemplate = "/POVendorInvoice/Update", Method = "PUT")]
        public void UpdatePOVendorInvoice(POVendorInvoiceInfo entity)
        {
            ObjectFactory<POVendorInvoiceAppService>.Instance.Update(entity);
        }

        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/POVendorInvoice/BatchAudit", Method = "PUT")]
        public string BatchAuditPOVendorInvoice(List<int> sysNoList)
        {
            return ObjectFactory<POVendorInvoiceAppService>.Instance.BatchAuditPOVendorInvoice(sysNoList);
        }

        /// <summary>
        /// 批量取消审核
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/POVendorInvoice/BatchUnAudit", Method = "PUT")]
        public string BatchUnAuditPOVendorInvoice(List<int> sysNoList)
        {
            return ObjectFactory<POVendorInvoiceAppService>.Instance.BatchUnAuditPOVendorInvoice(sysNoList);
        }

        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/POVendorInvoice/BatchAbandon", Method = "PUT")]
        public string BatchAbandonPOVendorInvoice(List<int> sysNoList)
        {
            return ObjectFactory<POVendorInvoiceAppService>.Instance.BatchAbandonPOVendorInvoice(sysNoList);
        }

        /// <summary>
        /// 批量取消作废
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/POVendorInvoice/BatchUnAbandon", Method = "PUT")]
        public string BatchUnAbandonPOVendorInvoice(List<int> sysNoList)
        {
            return ObjectFactory<POVendorInvoiceAppService>.Instance.BatchUnAbandonPOVendorInvoice(sysNoList);
        }
    }
}
