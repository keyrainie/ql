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
using ECCentral.QueryFilter.PO;
using ECCentral.BizEntity.PO;
using System.Collections.Generic;
using ECCentral.Service.PO.Restful.ResponseMsg;

namespace ECCentral.Portal.UI.PO.Facades
{
    public class GatherSettlementFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }
        public GatherSettlementFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询代收结算单列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryGatherSettlements(GatherSettleQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/GatherSettlement/QueryGatherSettlementList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 查询代收结算单商品List
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void QueryGatherSettlementItemList(GatherSettleItemsQueryFilter filter, EventHandler<RestClientEventArgs<GatherSettlementItemsQueryRsp>> callback)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/GatherSettlement/QueryGatherSettlementItemList";
            restClient.Query<GatherSettlementItemsQueryRsp>(relativeUrl, filter, callback);
        }

        /// <summary>
        ///  加载代收结算单
        /// </summary>
        /// <param name="gatherSysNo"></param>
        /// <param name="callback"></param>
        public void LoadGatherSettlementInfo(string gatherSysNo, EventHandler<RestClientEventArgs<GatherSettlementInfo>> callback)
        {
            string relativeUrl = string.Format("/POService/GatherSettlement/LoadGatherSettlement/{0}", gatherSysNo);
            restClient.Query<GatherSettlementInfo>(relativeUrl, callback);
        }

        /// <summary>
        /// 创建代收结算单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void CreateGatherSettlementInfo(GatherSettlementInfo info, EventHandler<RestClientEventArgs<GatherSettlementInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.CreateUserName = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/POService/GatherSettlement/CreateGatherSettlement";
            restClient.Create<GatherSettlementInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 更新代收结算单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void UpdateGatherSettlementInfo(GatherSettlementInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.CreateUserName = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/POService/GatherSettlement/UpdateGatherSettlement";
            restClient.Update(relativeUrl, info, callback);
        }

        /// <summary>
        /// 作废代收结算单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void AbandonGatherSettlementInfo(GatherSettlementInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.CreateUserName = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/POService/GatherSettlement/AbandonGatherSettlement";
            restClient.Update(relativeUrl, info, callback);
        }

        /// <summary>
        /// 审核代收结算单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void AuditGatherSettlementInfo(GatherSettlementInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            if (info.AuditUser == null)
                info.AuditUser = new BizEntity.Common.UserInfo();
            info.AuditUser.UserDisplayName = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/POService/GatherSettlement/AuditGatherSettlement";
            restClient.Update(relativeUrl, info, callback);
        }

        /// <summary>
        /// 取消审核代收结算单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void CancelAuditGatherSettlementInfo(GatherSettlementInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.CreateUserName = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/POService/GatherSettlement/CancelAuditGatherSettlement";
            restClient.Update(relativeUrl, info, callback);
        }
        /// <summary>
        /// 结算代收结算单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void SettleGatherSettlementInfo(GatherSettlementInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.CreateUserName = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/POService/GatherSettlement/SettleGatherSettlement";
            restClient.Update(relativeUrl, info, callback);
        }
        /// <summary>
        /// 取消结算代收结算单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void CancelSettleGatherSettlementInfo(GatherSettlementInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.CreateUserName = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/POService/GatherSettlement/CancelSettleGatherSettlement";
            restClient.Update(relativeUrl, info, callback);
        }
    }
}
