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

namespace ECCentral.Portal.UI.PO.Facades
{
    public class VirtualPurchaseOrderFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }

        public VirtualPurchaseOrderFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询虚库采购单列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryVirtualPurchaseOrders(VirtualPurchaseOrderQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/PurchaseOrder/QueryVirtualPurchaseOrderList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        public void ExportExcelForVirtualPurchaseOrders(VirtualPurchaseOrderQueryFilter queryFilter, ColumnSet[] columns)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/PurchaseOrder/QueryVirtualPurchaseOrderList";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }


        /// <summary>
        /// 加载单个虚库采购单信息
        /// </summary>
        /// <param name="vspoSysNo"></param>
        /// <param name="callback"></param>
        public void LoadVirtualPurchaseOrderInfo(string vspoSysNo, EventHandler<RestClientEventArgs<VirtualStockPurchaseOrderInfo>> callback)
        {
            string relativeUrl = string.Format("POService/PurchaseOrder/LoadVirtualPurchaseOrder/{0}", vspoSysNo);
            restClient.Query<VirtualStockPurchaseOrderInfo>(relativeUrl, callback);
        }

        /// <summary>
        /// 更新虚库采购单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void UpdateVirtualPurchaseOrderInfo(VirtualStockPurchaseOrderInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.CreateUserName = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "POService/PurchaseOrder/UpdateVirtualPurchaseOrder";
            restClient.Update(relativeUrl, info, callback);
        }

        /// <summary>
        /// 更新虚库采购单CS备注
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void UpdateVirtualPurchaseOrderInfoCSMemo(VirtualStockPurchaseOrderInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.CreateUserName = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "POService/PurchaseOrder/UpdateVirtualPurchaseOrderCSMemo";
            restClient.Update(relativeUrl, info, callback);
        }

        /// <summary>
        /// 作废虚库采购单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void AbandonVirtualPurchaseOrder(VirtualStockPurchaseOrderInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.CreateUserName = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "POService/PurchaseOrder/AbandonVirtualPurchaseOrder";
            restClient.Update(relativeUrl, info, callback);
        }

        /// <summary>
        /// 根据传入的SoitemSysNo加载虚库采购单信息
        /// </summary>
        /// <param name="soItemSysNo"></param>
        /// <param name="callback"></param>
        public void LoadVirtualPurchaseInfoBySOItemSysNo(string soSysNoAndProductSysNo, EventHandler<RestClientEventArgs<VirtualStockPurchaseOrderInfo>> callback)
        {
            string relativeUrl = string.Format("POService/PurchaseOrder/LoadVirtualPurchaseInfoBySOItemSysNo/{0}", soSysNoAndProductSysNo.ToString());
            restClient.Query<VirtualStockPurchaseOrderInfo>(relativeUrl, callback);
        }

        /// <summary>
        /// 创建虚库采购单(从SO链接)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void CreateVSPO(VirtualStockPurchaseOrderInfo info, EventHandler<RestClientEventArgs<VirtualStockPurchaseOrderInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/PurchaseOrder/CreateVSPO";
            restClient.Update<VirtualStockPurchaseOrderInfo>(relativeUrl, info, callback);
        }

        public void IsVSPOItemPriceLimited(string soSysNoAndProductSysNo, EventHandler<RestClientEventArgs<bool>> callback)
        {
            string relativeUrl = string.Format("POService/PurchaseOrder/IsVSPOItemPriceLimited/{0}", soSysNoAndProductSysNo.ToString());
            restClient.Query<bool>(relativeUrl, callback);
        }

    }
}
