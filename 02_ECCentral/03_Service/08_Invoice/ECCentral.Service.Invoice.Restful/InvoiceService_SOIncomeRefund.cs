using System;
using System.Collections.Generic;
using System.Data;
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
    /// <summary>
    /// 销售退款单RestfulService
    /// </summary>
    public partial class InvoiceService
    {
        /// <summary>
        /// 更新销售退款单
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/SOIncomeRefund/Update", Method = "PUT")]
        public void UpdateSOIncomeRefund(SOIncomeRefundInfo entity)
        {
            ObjectFactory<SOIncomeRefundAppService>.Instance.Update(entity);
        }

        /// <summary>
        /// 批量CS审核退款单
        /// </summary>
        [WebInvoke(UriTemplate = "/SOIncomeRefund/BatchCSAudit", Method = "PUT")]
        public string BatchCSAuditSOIncomeRefund(List<SOIncomeRefundInfo> entitys)
        {
            return ObjectFactory<SOIncomeRefundAppService>.Instance.BatchCSAudit(entitys);
        }

        /// <summary>
        /// 批量CS审核拒绝退款单
        /// </summary>
        [WebInvoke(UriTemplate = "/SOIncomeRefund/BatchCSReject", Method = "PUT")]
        public string BatchCSRejectSOIncomeRefund(List<int> sysNoList)
        {
            return ObjectFactory<SOIncomeRefundAppService>.Instance.BatchCSReject(sysNoList);
        }

        /// <summary>
        /// 批量财务审核退款单
        /// </summary>
        [WebInvoke(UriTemplate = "/SOIncomeRefund/BatchFinAudit", Method = "PUT")]
        public string BatchFinAuditSOIncomeRefund(List<SOIncomeRefundInfo> entitys)
        {
            return ObjectFactory<SOIncomeRefundAppService>.Instance.BatchFinAudit(entitys);
        }

        /// <summary>
        /// 批量财务审核拒绝退款单
        /// </summary>
        [WebInvoke(UriTemplate = "/SOIncomeRefund/BatchFinReject", Method = "PUT")]
        public string BatchFinRejectSOIncomeRefund(BatchFinRejectSOIncomeRefundReq request)
        {
            return ObjectFactory<SOIncomeRefundAppService>.Instance.BatchFinReject(request.SysNoList, request.FinAppendNote);
        }

        /// <summary>
        /// 批量取消审核退款单
        /// </summary>
        [WebInvoke(UriTemplate = "/SOIncomeRefund/BatchCancelAudit", Method = "PUT")]
        public string BatchCancelAuditSOIncomeRefund(List<int> sysNoLis)
        {
            return ObjectFactory<SOIncomeRefundAppService>.Instance.BatchCancelAudit(sysNoLis);
        }

        /// <summary>
        /// 审核RMA物流拒收
        /// </summary>
        /// <param name="sysNo"></param>
        [WebInvoke(UriTemplate = "/SOIncomeRefund/AuditAutoRMA", Method = "PUT")]
        public void SOIncomeRefundAuditAutoRMA(int sysNo)
        {
            ObjectFactory<SOIncomeRefundAppService>.Instance.AuditAutoRMA(sysNo);
        }

        #region NoBizQuery

        [WebInvoke(UriTemplate = "/SOIncomeRefund/Query", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryAuditRefund(AuditRefundQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IAuditRefundQueryDA>.Instance.Query(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/SOIncomeRefund/Export", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult ExportAuditRefund(AuditRefundQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IAuditRefundQueryDA>.Instance.Query(request, out totalCount);
            dataTable.Columns.Add("RefundStatusDesc");
            foreach (DataRow row in dataTable.Rows.AsParallel())
            {
                if (string.IsNullOrWhiteSpace(row["RefundStatus"].ToString()))
                {
                    row["RefundStatus"] = "N/A";
                }

                if (row.IsNull("RefundStatus") || row["RefundStatus"].ToString() == "-99")
                {
                    row["RefundStatusDesc"] = "N/A";
                }
                else
                {
                    row["RefundStatusDesc"] = ((RefundStatus)row["RefundStatus"]).ToDisplayText();
                }

                if (!row.IsNull("ShipRejected") && row["ShipRejected"].ToString() == "1")
                {
                    if (!row.IsNull("IncomeAmt"))
                    {
                        row["RefundCashAmt"] = ((decimal)row["IncomeAmt"]).ToString(InvoiceConst.StringFormat.DecimalFormat);
                    }
                    else
                    {
                        row["RefundCashAmt"] = "0.00";
                    }
                }
                else
                {
                    if (!row.IsNull("RefundCashAmt"))
                    {
                        row["RefundCashAmt"] = ((decimal)row["RefundCashAmt"]).ToString(InvoiceConst.StringFormat.DecimalFormat);
                    }
                    else
                    {
                        row["RefundCashAmt"] = "0.00";
                    }
                }
            }

            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        #endregion NoBizQuery
    }
}