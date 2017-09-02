using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.RMA;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.UI.RMA.Models;
using System.Collections.Generic;
using ECCentral.Portal.Basic;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.RMA.Facades
{
    public class RefundBalanceFacade
    {
        private readonly RestClient restClient;
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_RMA, "ServiceBaseUrl");
            }
        }
        public RefundBalanceFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }
        public RefundBalanceFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(int refundSysNo, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            RefundBalanceQueryFilter queryFilter = new RefundBalanceQueryFilter();
            queryFilter.RefundSysNo = refundSysNo;
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = string.Format("/RMAService/RefundBalance/Query");
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        #region Load
        public void LoadNewRefundBalanceByRefundSysNo(int refundSysNo, EventHandler<RestClientEventArgs<RefundBalanceInfo>> callback)
        {
            string relativeUrl = string.Format("/RMAService/RefundBalance/LoadNewRefundBalance/{0}", refundSysNo);
            restClient.Query<RefundBalanceInfo>(relativeUrl, callback);
        }

        public void LoadRefundBalanceBySysNo(int SysNo, EventHandler<RestClientEventArgs<RefundBalanceInfo>> callback)
        {
            string relativeUrl = string.Format("/RMAService/RefundBalance/LoadRefundBalance/{0}", SysNo);
            restClient.Query<RefundBalanceInfo>(relativeUrl, callback);
        }
        public void LoadRefundItemListRefundSysNo(int? refundSysNo, EventHandler<RestClientEventArgs<List<RefundItemInfo>>> callback)
        {
            string relativeUrl = string.Format("/RMAService/RefundBalance/LoadRefundItemList/{0}", refundSysNo);
            restClient.Query<List<RefundItemInfo>>(relativeUrl, callback);
        }

        #endregion Load

        public void Create(RefundBalanceInfo data, EventHandler<RestClientEventArgs<RefundBalanceInfo>> callback)
        {
            string relativeUrl = "/RMAService/RefundBalance/Create";
            restClient.Create<RefundBalanceInfo>(relativeUrl, data, callback);
        }

        public void SubmitAudit(RefundBalanceInfo data, EventHandler<RestClientEventArgs<RefundBalanceInfo>> callback)
        {
            string relativeUrl = "/RMAService/RefundBalance/SubmitAudit";
            restClient.Update(relativeUrl, data, callback);
        }

        public void Refund(RefundBalanceInfo data, EventHandler<RestClientEventArgs<RefundBalanceInfo>> callback)
        {
            string relativeUrl = "/RMAService/RefundBalance/Refund";
            restClient.Update(relativeUrl, data, callback);
        }

        public void Abandon(int sysNo, EventHandler<RestClientEventArgs<RefundBalanceInfo>> callback)
        {
            string relativeUrl = "/RMAService/RefundBalance/Abandon";
            restClient.Update(relativeUrl, sysNo, callback);
        }
    }
}
