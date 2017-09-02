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
using ECCentral.Service.Invoice.Restful.ResponseMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        /// <summary>
        /// 创建付款单
        /// </summary>
        [WebInvoke(UriTemplate = "/PayItem/Create", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public PayItemInfo CreatePayItem(PayItemInfo input)
        {
            return ObjectFactory<PayItemAppService>.Instance.Create(input);
        }

        /// <summary>
        /// 更新付款单
        /// </summary>
        [WebInvoke(UriTemplate = "/PayItem/Update", Method = "PUT")]
        public void UpdatePayItem(PayItemInfo entity)
        {
            ObjectFactory<PayItemAppService>.Instance.Update(entity);
        }

        /// <summary>
        /// 作废付款单
        /// </summary>
        [WebInvoke(UriTemplate = "/PayItem/Abandon", Method = "PUT")]
        public void AbandonPayItem(PayItemInfo entity)
        {
            ObjectFactory<PayItemAppService>.Instance.Abandon(entity);
        }

        /// <summary>
        /// 取消作废付款单
        /// </summary>
        [WebInvoke(UriTemplate = "/PayItem/CancelAbandon", Method = "PUT")]
        public void CancelAbandonPayItem(PayItemInfo entity)
        {
            ObjectFactory<PayItemAppService>.Instance.CancelAbandon(entity);
        }

        /// <summary>
        /// 支付付款单
        /// </summary>
        [WebInvoke(UriTemplate = "/PayItem/Pay", Method = "PUT")]
        public void PayPayItem(PayItemInfo entity)
        {
            ObjectFactory<PayItemAppService>.Instance.Pay(entity, false);
        }

        /// <summary>
        /// 批量支付付款单
        /// </summary>
        /// <param name="payItemList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PayItem/BatchPay", Method = "PUT")]
        public string BatchPayPayItem(List<PayItemInfo> payItemList)
        {
            return ObjectFactory<PayItemAppService>.Instance.BatchPay(payItemList);
        }

        /// <summary>
        /// 取消支付付款单
        /// </summary>
        [WebInvoke(UriTemplate = "/PayItem/CancelPay", Method = "PUT")]
        public void CancelPayPayItem(PayItemInfo entity)
        {
            ObjectFactory<PayItemAppService>.Instance.CancelPay(entity);
        }

        /// <summary>
        /// 锁定付款单
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/PayItem/Lock", Method = "PUT")]
        public void LockPayItem(PayItemInfo entity)
        {
            ObjectFactory<PayItemAppService>.Instance.Lock(entity);
        }

        /// <summary>
        /// 取消锁定付款单
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/PayItem/CancelLock", Method = "PUT")]
        public void CancelLockPayItem(PayItemInfo entity)
        {
            ObjectFactory<PayItemAppService>.Instance.CancelLock(entity);
        }

        /// <summary>
        /// 批量设置付款单凭证号
        /// </summary>
        /// <param name="payItemList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PayItem/BatchSetPayItemReferenceID", Method = "PUT")]
        public string BatchSetPayItemReferenceID(List<PayItemInfo> payItemList)
        {
            return ObjectFactory<PayItemAppService>.Instance.BatchSetReferenceID(payItemList);
        }

        #region NoBizQuery

        /// <summary>
        /// 查询付款单
        /// </summary>
        /// <param name="filter">付款单查询条件</param>
        /// <returns>付款单查询结果</returns>
        [WebInvoke(UriTemplate = "/PayItem/Query", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResultList QueryPayItemList(PayItemQueryFilter filter)
        {
            int totalCount;
            var dataSet = ObjectFactory<IPayItemQueryDA>.Instance.Query(filter, out totalCount);
            return new QueryResultList()
            {
                new QueryResult(){ Data = dataSet.Tables["DataResult"], TotalCount = totalCount},
                new QueryResult(){ Data = dataSet.Tables["StatisticResult"]}
            };
        }

        /// <summary>
        /// 导出付款单
        /// </summary>
        /// <param name="filter">付款单查询条件</param>
        /// <returns>付款单查询结果</returns>
        [WebInvoke(UriTemplate = "/PayItem/Export", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult ExportPayItemList(PayItemQueryFilter filter)
        {
            int totalCount;
            var dataSet = ObjectFactory<IPayItemQueryDA>.Instance.Query(filter, out totalCount);
            var dataTable = dataSet.Tables["DataResult"];

            dataTable.Columns.Add("InvoiceUpdate");
            foreach (DataRow row in dataTable.Rows.AsParallel())
            {
                string batchnum = "";
                if (!row.IsNull("BatchNumber") && !string.IsNullOrWhiteSpace(row["BatchNumber"].ToString()))
                {
                    batchnum = "-" + row["BatchNumber"].ToString().PadLeft(2, '0');
                }
                if (row["OrderType"].ToString() == "5")
                {
                    row["OrderID"] = row["OrderID"] + "A";
                }
                else if (row["OrderType"].ToString() == "0" || row["OrderType"].ToString() == "7")
                {
                    row["OrderID"] = row["OrderID"] + batchnum;
                }

                if (string.IsNullOrWhiteSpace(row["ReferenceID"].ToString()))
                {
                    row["ReferenceID"] = "N/A";
                }

                if (!row.IsNull("CreateTime"))
                {
                    if ((DateTime)row["CreateTime"] == DateTime.MinValue)
                    {
                        row["CreateTime"] = DBNull.Value;
                    }
                    row["CreateTime"] = ((DateTime)row["CreateTime"]).ToString(InvoiceConst.StringFormat.LongDateFormat);
                }

                if (!row.IsNull("InvoiceUpdateTime"))
                {
                    row["InvoiceUpdate"] = row["UpdateInvoiceUserName"] + " " + ((DateTime)row["InvoiceUpdateTime"]).ToString(InvoiceConst.StringFormat.ShortDateFormat);
                }
                else
                {
                    row["InvoiceUpdate"] = row["UpdateInvoiceUserName"];
                }
            }

            return new QueryResult()
            {
                Data = dataTable
                ,TotalCount=dataTable.Rows.Count
            };
        }

        /// <summary>
        /// 查询可以进行支付的单据列表
        /// </summary>
        /// <param name="filter">单据查询条件</param>
        /// <returns>单据查询结果</returns>
        [WebInvoke(UriTemplate = "/PayItem/OrderQuery", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryCanBePayOrderList(CanBePayOrderQueryFilter filter)
        {
            int totalCount;
            var dataSet = ObjectFactory<ICanBePayOrderQueryDA>.Instance.Query(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataSet,
                TotalCount = totalCount
            };
        }

        #endregion NoBizQuery
    }
}