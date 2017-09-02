using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.Service.Invoice.Restful.ResponseMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        /// <summary>
        /// 更新应付款发票信息
        /// </summary>
        [WebInvoke(UriTemplate = "/Payable/UpdateInvoice", Method = "PUT")]
        public string UpdatePayableInvoice(List<PayableInfo> entities)
        {
            return ObjectFactory<PayableAppService>.Instance.BatchUpdateInvoice(entities);
        }

        /// <summary>
        /// PO Job调用，用于创建应付款信息
        /// </summary>
        /// <param name="info">应付款信息</param>
        [WebInvoke(UriTemplate = "/Payable/CreateByPO", Method = "PUT")]
        public void CreatePayByPO(PayableInfo info)
        {
            ObjectFactory<PayableAppService>.Instance.CreateByPO(info);
        }

        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Payable/BatchAudit", Method = "PUT")]
        public string BatchAudit(List<PayableInfo> entities)
        {
            return ObjectFactory<PayableAppService>.Instance.BatchAudit(entities);
        }

        /// <summary>
        /// 批量审核拒绝
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Payable/BatchRefuseAudit", Method = "PUT")]
        public string BatchRefuseAudit(List<PayableInfo> entities)
        {
            return ObjectFactory<PayableAppService>.Instance.BatchRefuseAudit(entities);
        }

        /// <summary>
        /// 批量付款
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Payable/BatchUpdateStatusAndAlreadyPayAmt", Method = "PUT")]
        public string BatchUpdateStatusAndAlreadyPayAmt(List<PayableInfo> entities)
        {
            return ObjectFactory<PayableAppService>.Instance.BatchUpdateStatusAndAlreadyPayAmt(entities);
        }

        #region NoBizQuery

        /// <summary>
        /// 查询所有的供应商账期类型
        /// </summary>
        /// <param name="companyCode">CompanyCode</param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Payable/AllVendorPayTerms/{companyCode}", Method = "GET")]
        public List<CodeNamePair> GetAllVendorPayTerms(string companyCode)
        {
            return ObjectFactory<IPayableQueryDA>.Instance.GetAllVendorPayTerms(companyCode);
        }

        /// <summary>
        /// 应付款查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Payable/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResultList QueryPayable(PayableQueryFilter request)
        {
            QueryResultList resultList = new QueryResultList();
            int totalCount;
            DataTable dtStatistical;
            var dataTable = ObjectFactory<IPayableQueryDA>.Instance.QueryPayable(request, out totalCount, out dtStatistical);
            dataTable.Columns.Add("SapInFailedDetailReason", typeof(string));

            QueryResult resultTable = new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
            QueryResult statisticalTable = new QueryResult()
            {
                Data = dtStatistical
            };
            resultList.Add(resultTable);
            resultList.Add(statisticalTable);
            return resultList;
        }

        /// <summary>
        /// 应付款导出
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Payable/Export", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult ExportPayable(PayableQueryFilter request)
        {
            QueryResultList resultList = new QueryResultList();
            int totalCount;
            DataTable dtStatistical;
            var dataTable = ObjectFactory<IPayableQueryDA>.Instance.QueryPayable(request, out totalCount, out dtStatistical);
            dataTable.Columns.Add("OrderIDDesc");
            dataTable.Columns.Add("Remainder");
            foreach (DataRow row in dataTable.Rows.AsParallel())
            {
                string batchnum = "";
                if (!row.IsNull("BatchNumber") && !string.IsNullOrWhiteSpace(row["BatchNumber"].ToString()))
                {
                    batchnum = "-" + row["BatchNumber"].ToString().PadLeft(2, '0');
                }

                if (row["OrderType"].ToString() == "0")
                {
                    row["OrderIDDesc"] = row["OrderID"] + batchnum;
                }
                else if (row["OrderType"].ToString() == "7")
                {
                    row["OrderIDDesc"] = row["OrderSysNo"] + batchnum;
                }
                else if (row["OrderType"].ToString() == "5")
                {
                    row["OrderIDDesc"] = row["OrderID"] + "A";
                }
                else
                {
                    row["OrderIDDesc"] = row["OrderID"];
                }

                if (!row.IsNull("PayableAmt") && !row.IsNull("AlreadyPayAmt"))
                {
                    row["PayableAmt"] = ((decimal)row["PayableAmt"]).ToString(InvoiceConst.StringFormat.DecimalFormat);
                    row["AlreadyPayAmt"] = ((decimal)row["AlreadyPayAmt"]).ToString(InvoiceConst.StringFormat.DecimalFormat);

                    row["Remainder"] = (((decimal)row["PayableAmt"]) - ((decimal)row["AlreadyPayAmt"])).ToString(InvoiceConst.StringFormat.DecimalFormat);
                }
            }
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 加载应付款和付款单数据，用于应付款编辑
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Payable/LoadForEdit", Method = "POST")]
        public PayDetailInfoResp LoadPayDetailInfo(PayItemDetailInfoReq request)
        {
            if (request.PaySysNo.HasValue)
            {
                return LoadForEditBySysNo(request.PaySysNo.Value, request.CompanyCode);
            }
            else if (request.OrderSysNo.HasValue && request.OrderType.HasValue)
            {
                return LoadForEditByOrderSysNoAndOrderType(request.OrderSysNo.Value, request.OrderType.Value, request.CompanyCode);
            }
            else
            {
                throw new ArgumentException("Invalid Parameter");
            }
        }

        #region 非Restful私有方法

        /// <summary>
        /// [Private]根据应付款系统编号加载应付款和付款单数据
        /// </summary>
        /// <param name="paySysNo">应付款系统编号</param>
        /// <param name="companyCode">CompanyCode</param>
        /// <returns></returns>
        private PayDetailInfoResp LoadForEditBySysNo(int paySysNo, string companyCode)
        {
            PayDetailInfoResp resp = new PayDetailInfoResp();

            DataTable payItemDT = ObjectFactory<IPayItemQueryDA>.Instance.SimpleQuery(paySysNo);
            if (payItemDT != null && payItemDT.Rows.Count > 0)
            {
                resp.PayItemList = DataMapper.GetEntityList<PayItemInfo, List<PayItemInfo>>(payItemDT.Rows);
            }
            if (payItemDT == null || payItemDT.Rows.Count <= 0)
            {
                throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.Payable, "Payable_LoadForEditBySysNo_DataError"));
            }
            int totalCount;
            DataTable st;
            PayableQueryFilter filter = new PayableQueryFilter();
            filter.SysNo = paySysNo;
            filter.CompanyCode = companyCode;
            filter.OrderType = resp.PayItemList[0].OrderType;
            DataTable payDT = ObjectFactory<IPayableQueryDA>.Instance.QueryPayable(filter, out totalCount, out st);

            if (payDT == null || payDT.Rows.Count <= 0)
            {
                throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.Payable, "Payable_LoadForEditBySysNo_DataError"));
            }

            var pay = payDT.Rows[0];
            var payStyle = PayItemStyle.Normal;
            if ((int)pay["OrderType"] == (int)PayableOrderType.PO)
            {
                if ((int)pay["OrderStatus"] == (int)PurchaseOrderStatus.WaitingInStock)
                {
                    payStyle = PayItemStyle.Advanced;
                }
            }

            bool isVendorHoldedControl = false;
            if ((int)pay["OrderType"] == (int)PayableOrderType.PO || (int)pay["OrderType"] == (int)PayableOrderType.VendorSettleOrder
                || (int)pay["OrderType"] == (int)PayableOrderType.CollectionSettlement)
            {
                isVendorHoldedControl = ObjectFactory<PayableAppService>.Instance.IsHolderVendorByVendorSysNo((int)pay["VendorSysNo"]);
            }

            var entity = new OrderInfo();            
               entity.PayStyle = payStyle;
               entity.OrderAmt = (decimal)pay["PayableAmt"];
               entity.OrderSysNo = (int)pay["OrderSysNo"];
               entity.OrderID = pay["OrderID"].ToString();
               entity.OrderType = (PayableOrderType)pay["OrderType"];
               entity.PaySysNo = paySysNo;
               entity.OrderStatus = (int)pay["OrderStatus"];
               entity.BatchNumber = pay.IsNull("BatchNumber") ? 1 : (int)pay["BatchNumber"];
               entity.IsVendorHoldedControl = isVendorHoldedControl;           
               resp.OrderInfo=entity;
            var totalAmt = 0.00M;
            var paidAmt = 0.00M;
            if (resp.PayItemList != null && resp.PayItemList.Count > 0)
            {
                resp.PayItemList.ForEach(p =>
                {
                    totalAmt += p.PayAmt.Value;
                    paidAmt += p.Status == PayItemStatus.Paid ? p.PayAmt.Value : 0;
                });
            }

            resp.TotalInfo = new TotalInfo()
            {
                TotalAmt = totalAmt,
                PaidAmt = paidAmt,
                OrderAmt = (decimal)pay["PayableAmt"],
                OrderSysNo = (int)pay["OrderSysNo"]
            };

            return resp;
        }

        /// <summary>
        /// [Private]根据应付款单据类型和应付款单据编号加载应付款和付款单数据
        /// </summary>
        /// <param name="orderSysNo">应付款单据编号</param>
        /// <param name="orderType">应付款单据类型</param>
        /// <param name="companyCode">CompanyCode</param>
        /// <returns></returns>
        private PayDetailInfoResp LoadForEditByOrderSysNoAndOrderType(int orderSysNo, PayableOrderType orderType, string companyCode)
        {
            PayDetailInfoResp resp = new PayDetailInfoResp();

            int totalCount;
            DataTable st;
            PayableQueryFilter filter = new PayableQueryFilter();
            filter.OrderID = orderSysNo.ToString();
            filter.OrderType = orderType;
            filter.CompanyCode = companyCode;

            DataTable payDT = ObjectFactory<IPayableQueryDA>.Instance.QueryPayable(filter, out totalCount, out st);

            bool isVendorHoldedControl = false;
            DataRow pay = null;
            if (payDT != null && payDT.Rows.Count > 0)
            {
                pay = payDT.Rows[0];
                if ((int)pay["OrderType"] == (int)PayableOrderType.PO || (int)pay["OrderType"] == (int)PayableOrderType.VendorSettleOrder
                    || (int)pay["OrderType"] == (int)PayableOrderType.CollectionSettlement)
                {
                    isVendorHoldedControl = ObjectFactory<PayableAppService>.Instance.IsHolderVendorByVendorSysNo((int)pay["VendorSysNo"]);
                }

                int paySysNo = Convert.ToInt32(pay["PaySysNo"]);
                DataTable payItemDT = ObjectFactory<IPayItemQueryDA>.Instance.SimpleQuery(paySysNo);
                resp.PayItemList = DataMapper.GetEntityList<PayItemInfo, List<PayItemInfo>>(payItemDT.Rows);
            }

            CanBePayOrderQueryFilter queryFilter = new CanBePayOrderQueryFilter();
            queryFilter.OrderID = orderSysNo.ToString();
            queryFilter.OrderType = orderType;
            queryFilter.CompanyCode = companyCode;

            DataTable orderDT = ObjectFactory<ICanBePayOrderQueryDA>.Instance.Query(queryFilter, out totalCount);
            if (orderDT == null || orderDT.Rows.Count <= 0)
            {
                throw new ECCentral.BizEntity.BizException(string.Format(
                    ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.Payable, "Payable_LoadForEditBySysNo_OrderNotExist"), orderSysNo));
            }

            var order = orderDT.Rows[0];
            var payStyle = PayItemStyle.Normal;
            if (orderType == PayableOrderType.PO)
            {
                if ((int)order["OrderStatus"] == (int)PurchaseOrderStatus.WaitingInStock)
                {
                    payStyle = PayItemStyle.Advanced;
                }
            }

            resp.OrderInfo = new OrderInfo()
            {
                PayStyle = payStyle,
                OrderAmt = (decimal)order["OrderAmt"],
                OrderSysNo = orderSysNo,
                OrderID = (string)order["OrderID"],
                OrderType = orderType,
                PaySysNo = pay != null ? (int)pay["PaySysNo"] : 0,
                OrderStatus = (int)order["OrderStatus"],
                BatchNumber = (int)order["BatchNumber"],
                IsVendorHoldedControl = isVendorHoldedControl
            };

            var totalAmt = 0.00M;
            var paidAmt = 0.00M;
            if (resp.PayItemList != null && resp.PayItemList.Count > 0)
            {
                resp.PayItemList.ForEach(p =>
                {
                    totalAmt += p.PayAmt.Value;
                    paidAmt += p.Status == PayItemStatus.Paid ? p.PayAmt.Value : 0;
                });
            }
            resp.TotalInfo = new TotalInfo()
            {
                TotalAmt = totalAmt,
                PaidAmt = paidAmt,
                OrderAmt = (decimal)order["OrderAmt"],
                OrderSysNo = (int)order["OrderSysNo"]
            };

            return resp;
        }

        #endregion 非Restful私有方法

        #endregion NoBizQuery
    }
}
