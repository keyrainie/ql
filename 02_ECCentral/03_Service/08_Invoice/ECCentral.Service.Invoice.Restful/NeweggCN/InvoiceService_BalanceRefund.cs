using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Invoice.Refund;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        /// <summary>
        /// 创建客户余额退款
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/BalanceRefund/Create", Method = "POST")]
        public BalanceRefundInfo CreateBalanceRefund(BalanceRefundInfo entity)
        {
            return ObjectFactory<BalanceRefundAppService>.Instance.Create(entity);
        }

        /// <summary>
        /// 更新客户余额退款
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/BalanceRefund/Update", Method = "PUT")]
        public void UpdateBalanceRefund(BalanceRefundInfo entity)
        {
            ObjectFactory<BalanceRefundAppService>.Instance.Update(entity);
        }

        /// <summary>
        /// 批量CS审核
        /// </summary>
        [WebInvoke(UriTemplate = "/BalanceRefund/CSConfirm", Method = "PUT")]
        public string BatchCSConfirmBalanceRefund(List<int> sysNoList)
        {
            return ObjectFactory<BalanceRefundAppService>.Instance.BatchCSConfirm(sysNoList);
        }

        /// <summary>
        /// 批量财务审核
        /// </summary>
        [WebInvoke(UriTemplate = "/BalanceRefund/FinConfirm", Method = "PUT")]
        public virtual string BatchFinConfirmBalanceRefund(List<int> sysNoList)
        {
            return ObjectFactory<BalanceRefundAppService>.Instance.BatchFinConfirm(sysNoList);
        }

        /// <summary>
        /// 批量取消审核
        /// </summary>
        [WebInvoke(UriTemplate = "/BalanceRefund/CancelConfirm", Method = "PUT")]
        public virtual string BatchCancelConfirmBalanceRefund(List<int> sysNoList)
        {
            return ObjectFactory<BalanceRefundAppService>.Instance.BatchCancelConfirm(sysNoList);
        }

        /// <summary>
        /// 批量作废
        /// </summary>
        [WebInvoke(UriTemplate = "/BalanceRefund/Abandon", Method = "PUT")]
        public virtual string BatchAbandonBalanceRefund(List<int> sysNoList)
        {
            return ObjectFactory<BalanceRefundAppService>.Instance.BatchAbandon(sysNoList);
        }

        /// <summary>
        /// 批量设置凭证号
        /// </summary>
        [WebInvoke(UriTemplate = "/BalanceRefund/SetReferenceID", Method = "PUT")]
        public virtual string BatchSetBalanceRefundReferenceID(BatchSetBalanceRefundReferenceIDReq request)
        {
            return ObjectFactory<BalanceRefundAppService>.Instance.BatchSetReferenceID(request.SysNoList, request.ReferenceID);
        }

        #region NoBizQuery

        [WebInvoke(UriTemplate = "/BalanceRefund/Query", Method = "POST")]
        public QueryResultList QueryBalanceRefund(BalanceRefundQueryFilter request)
        {
            int totalCount;
            var dataSet = ObjectFactory<IBalanceRefundQueryDA>.Instance.Query(request, out totalCount);
            return new QueryResultList()
            {
                new QueryResult(){ Data = dataSet.Tables["DataResult"], TotalCount = totalCount},
                new QueryResult(){ Data = dataSet.Tables["StatisticResult"]}
            };
        }


        [WebInvoke(UriTemplate = "/BalanceRefund/Export", Method = "POST")]
        public QueryResult ExportBalanceRefund(BalanceRefundQueryFilter request)
        {
            int totalCount;
            var dataSet = ObjectFactory<IBalanceRefundQueryDA>.Instance.Query(request, out totalCount);

            return new QueryResult() { Data = dataSet.Tables["DataResult"], TotalCount = totalCount };
        }

        #endregion NoBizQuery

        /// <summary>
        /// 修改积分有效期 从Customer入
        /// </summary>
        //[WebInvoke(UriTemplate = "/BalanceRefund/UpdatePointExpiringDate", Method = "PUT")]
        //public virtual void UpdatePointExpiringDate(string obtainSysNo, DateTime expiredDate)
        //{
        //    ObjectFactory<BalanceRefundAppService>.Instance.UpdatePointExpiringDate(obtainSysNo, expiredDate);
        //}
    }
}