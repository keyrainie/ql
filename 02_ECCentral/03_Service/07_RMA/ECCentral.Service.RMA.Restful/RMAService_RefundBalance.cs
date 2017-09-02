using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.RMA;
using ECCentral.Service.Utility;
using ECCentral.Service.RMA.AppService;
using System.Data;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.IDataAccess.NoBizQuery;

namespace ECCentral.Service.RMA.Restful
{
    public partial class RMAService
    {
        #region Load
        [WebInvoke(UriTemplate = "/RefundBalance/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryRefundBalanceListByRefundSysNo(RefundBalanceQueryFilter queryFilter)
        {
            int _refundSysNo;
            if (!int.TryParse(queryFilter.RefundSysNo.ToString(), out _refundSysNo))
            {
                throw new ArgumentException("Invalid refundSysNo");
            }
            int totalCount;
            var dataTable = ObjectFactory<IRefundBalanceQueryDA>.Instance.Query(queryFilter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        [WebInvoke(UriTemplate = "/RefundBalance/LoadRefundItemList/{refundSysNo}", Method = "GET")]
        public List<RefundItemInfo> LoadRefundItemListRefundSysNo(string refundSysNo)
        {
            int _refundSysNo;
            if (!int.TryParse(refundSysNo, out _refundSysNo))
            {
                throw new ArgumentException("Invalid refundSysNo");
            }
            return ObjectFactory<RefundBalanceAppService>.Instance.LoadRefundItemListByRefundSysNo(_refundSysNo);
        }
        [WebInvoke(UriTemplate = "/RefundBalance/LoadNewRefundBalance/{refundSysNo}", Method = "GET")]
        public RefundBalanceInfo LoadNewRefundBalanceByRefundSysNo(string refundSysNo)
        {
            int _refundSysNo;
            if (!int.TryParse(refundSysNo, out _refundSysNo))
            {
                throw new ArgumentException("Invalid refundSysNo");
            }
            return ObjectFactory<RefundBalanceAppService>.Instance.LoadNewRefundBalanceByRefundSysNo(_refundSysNo);
        }

        [WebInvoke(UriTemplate = "/RefundBalance/LoadRefundBalance/{SysNo}", Method = "GET")]
        public RefundBalanceInfo LoadRefundBalanceBySysNo(string SysNo)
        {
            int sysNo;
            if (!int.TryParse(SysNo, out sysNo))
            {
                throw new ArgumentException("Invalid SysNo");
            }
            return ObjectFactory<RefundBalanceAppService>.Instance.LoadRefundBalanceBySysNo(sysNo);
        }
        #endregion

        #region action
        [WebInvoke(UriTemplate = "/RefundBalance/Create", Method = "POST")]
        public void CreateRefundBalance(RefundBalanceInfo entity)
        {
            ObjectFactory<RefundBalanceAppService>.Instance.Create(entity);
        }

        [WebInvoke(UriTemplate = "/RefundBalance/SubmitAudit", Method = "PUT")]
        public void SubmitAudit(RefundBalanceInfo entity)
        {
            ObjectFactory<RefundBalanceAppService>.Instance.SubmitAudit(entity);
        }
        [WebInvoke(UriTemplate = "/RefundBalance/Refund", Method = "PUT")]
        public void Refund(RefundBalanceInfo entity)
        {
            ObjectFactory<RefundBalanceAppService>.Instance.Refund(entity);
        }
        [WebInvoke(UriTemplate = "/RefundBalance/Abandon", Method = "PUT")]
        public void AbandonRefundBalance(int sysNo)
        {
            ObjectFactory<RefundBalanceAppService>.Instance.Abandon(sysNo);
        }
        #endregion
    }
}
