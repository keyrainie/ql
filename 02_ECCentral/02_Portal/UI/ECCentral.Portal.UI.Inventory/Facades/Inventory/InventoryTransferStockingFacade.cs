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
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Inventory.Restful.ResponseMsg;
using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class InventoryTransferStockingFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Inventory", "ServiceBaseUrl");
            }
        }
        public InventoryTransferStockingFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询 - 备货中心List
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryInventoryTransferStockingList(InventoryTransferStockingQueryFilter queryFilter, EventHandler<RestClientEventArgs<InventoryTransferStockingQueryRsp>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "InventoryService/Inventory/QueryInventoryTransferStockingList";
            restClient.Query<InventoryTransferStockingQueryRsp>(relativeUrl, queryFilter, callback);
        }
        /// <summary>
        ///  导出excel - 备货中心List
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="columns"></param>
        public void ExportExcelForTransferStockingList(InventoryTransferStockingQueryFilter queryFilter, ColumnSet[] columns)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "InventoryService/Inventory/QueryInventoryTransferStockingListForExportExcel";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

        /// <summary>
        /// 备货中心 - 当日需备货供应商查询
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryVendorInfoListForBackOrderToday(BackOrderForTodayQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "InventoryService/Inventory/QueryVendorInfoListForBackOrderToday";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        public void CreateBasketItemsForPrepare(List<ProductCenterItemInfo> list, EventHandler<RestClientEventArgs<int>> callback)
        {
            list.ForEach(x =>
            {
                x.CompanyCode = CPApplication.Current.CompanyCode;
            });
            string relativeUrl = "InventoryService/Inventory/CreateBasketItemsForPrepare";
            restClient.Update(relativeUrl, list, callback);
        }

        public void CreateShiftBasket(ShiftRequestItemBasket basketInfo, EventHandler<RestClientEventArgs<int>> callback)
        {
            basketInfo.CompanyCode = CPApplication.Current.CompanyCode;
            basketInfo.ShiftItemInfoList.ForEach(x =>
            {
                x.CompanyCode = CPApplication.Current.CompanyCode;
            });

            string relativeUrl = "InventoryService/ShiftRequest/BatchCreateShiftBasket";
            restClient.Update(relativeUrl, basketInfo, callback);
        }
    }

}
