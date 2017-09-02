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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.PO;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.Restful.RequestMsg;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.PO.Facades
{
    public class CostChangeFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }
        public CostChangeFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询成本变价单列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryCostChange(CostChangeQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/CostChange/QueryCostChangeList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        ///  查询可用商品入库成本明细
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryCostItems(CostChangeItemsQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "InventoryService/Inventory/QueryAvaliableCostInList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 加载成本变价单信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <param name="callback"></param>
        public void LoadCostChangeInfo(string ccSysNo, EventHandler<RestClientEventArgs<CostChangeInfo>> callback)
        {
            string relativeUrl = string.Format("POService/CostChange/LoadCostChangeInfo/{0}", ccSysNo);
            restClient.Query<CostChangeInfo>(relativeUrl, callback);
        }

        /// <summary>
        /// 更新成本变价单信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callbadk"></param>
        public void UpdateCostChangeInfo(CostChangeInfo info, EventHandler<RestClientEventArgs<CostChangeInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/CostChange/UpdateCostChange";
            restClient.Update<CostChangeInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 创建成本变价单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void CreateCostChange(CostChangeInfo info, EventHandler<RestClientEventArgs<CostChangeInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            if (info.CostChangeBasicInfo == null)
            {
                info.CostChangeBasicInfo = new CostChangeBasicInfo();
            }
            if (null == info.CostChangeItems)
            {
                info.CostChangeItems = new List<CostChangeItemsInfo>();
            }
            info.CostChangeItems.ForEach(x =>
            {
                x.CompanyCode = CPApplication.Current.CompanyCode;
            });
            info.CostChangeBasicInfo.InUser = CPApplication.Current.LoginUser.UserSysNo;

            string relativeUrl = "POService/CostChange/CreateCostChange";
            restClient.Update<CostChangeInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 提交审核成本变价单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void SubmitAuditCostChange(CostChangeInfo info, EventHandler<RestClientEventArgs<CostChangeInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "POService/CostChange/SubmitAuditCostChange";
            restClient.Update<CostChangeInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 撤销提交成本变价单信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void CancelSubmitAuditPOCostChange(CostChangeInfo info, EventHandler<RestClientEventArgs<CostChangeInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "POService/CostChange/CancelSubmitAuditPOCostChange";
            restClient.Update<CostChangeInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 拒绝并退回成本变价单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void RefuseCostChange(CostChangeInfo info, EventHandler<RestClientEventArgs<CostChangeInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "POService/CostChange/RefuseCostChange";
            restClient.Update<CostChangeInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 作废成本变价单信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void AbandonCostChange(CostChangeInfo info, EventHandler<RestClientEventArgs<CostChangeInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "POService/CostChange/AbandonCostChange";
            restClient.Update<CostChangeInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 审核通过成本变价单信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void AuditCostChange(CostChangeInfo info, EventHandler<RestClientEventArgs<CostChangeInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "POService/CostChange/AuditCostChange";
            restClient.Update<CostChangeInfo>(relativeUrl, info, callback);
        }

    }
}