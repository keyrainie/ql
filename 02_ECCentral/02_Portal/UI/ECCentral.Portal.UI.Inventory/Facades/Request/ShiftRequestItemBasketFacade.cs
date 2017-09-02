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
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.Inventory.Facades.Request
{
    public class ShiftRequestItemBasketFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Inventory", "ServiceBaseUrl");
            }
        }
        public ShiftRequestItemBasketFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询移仓篮List
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryBasketList(ShiftRequestItemBasketQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "InventoryService/Inventory/QueryShiftRequestItemBasketList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 批量更新移仓篮Items
        /// </summary>
        /// <param name="listItem"></param>
        /// <param name="callback"></param>
        public void BatchUpdateShiftBasketItem(List<ShiftRequestItemInfo> listItem, EventHandler<RestClientEventArgs<string>> callback)
        {
            listItem.ForEach(x =>
            {
                x.CompanyCode = CPApplication.Current.CompanyCode;
            });
            string relativeUrl = "InventoryService/ShiftRequest/BatchUpdateShiftBasket";
            restClient.Update(relativeUrl, listItem, callback);
        }

        /// <summary>
        /// 批量删除移仓篮Items
        /// </summary>
        /// <param name="listItem"></param>
        /// <param name="callback"></param>
        public void BatchDeleteShiftBasketItem(List<ShiftRequestItemInfo> listItem, EventHandler<RestClientEventArgs<string>> callback)
        {
            listItem.ForEach(x =>
            {
                x.CompanyCode = CPApplication.Current.CompanyCode;
            });
            string relativeUrl = "InventoryService/ShiftRequest/BatchDeleteShiftBasket";
            restClient.Update(relativeUrl, listItem, callback);
        }

        /// <summary>
        /// 批量创建移仓单
        /// </summary>
        /// <param name="shiftInfo"></param>
        /// <param name="callback"></param>
        public void BatchCreateShiftRequest(List<ShiftRequestInfo> shiftInfo, EventHandler<RestClientEventArgs<string>> callback)
        {
            shiftInfo.ForEach(x =>
            {
                x.CompanyCode = CPApplication.Current.CompanyCode;
                x.CreateUser = new UserInfo() { SysNo=CPApplication.Current.LoginUser.UserSysNo};
                x.ShiftItemInfoList.ForEach(y =>
                {
                    y.CompanyCode = CPApplication.Current.CompanyCode;
                });
            });
            string relativeUrl = "InventoryService/ShiftRequest/BatchCreateShiftRequest";
            restClient.Update(relativeUrl, shiftInfo, callback);

        }

        /// <summary>
        /// 导出全部
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="columns"></param>
        public void ExportShiftRequestItemBasket(ShiftRequestItemBasketQueryFilter queryFilter, ColumnSet[] columns)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("InventoryService/Inventory/QueryShiftRequestItemBasketList", queryFilter, columns);
        }
    }
}
