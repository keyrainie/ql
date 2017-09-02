using System;
using System.Collections.Generic;
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
using Newegg.Oversea.Silverlight.Controls;

using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.UI.Inventory.Models;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    /// <summary>
    /// 货卡查询 - QueryFacade
    /// </summary>
    public class ItemsCardQueryFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Inventory", "ServiceBaseUrl");
            }
        }
        public ItemsCardQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }       

        /// <summary>
        /// 货卡查询 - 商品渠道仓库存信息
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryItemsCardInventoryByStock(InventoryItemCardQueryVM model, Action<int, List<dynamic>> callback)
        {
            InventoryQueryFilter filter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            filter = model.ConvertVM<InventoryItemCardQueryVM, InventoryQueryFilter>();
            filter.IsShowTotalInventory = false;

            string relativeUrl = "/InventoryService/Inventory/QueryProductInventory";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    int totalCount = 0;
                    List<dynamic> vmList = null;
                    if (!args.FaultsHandle())
                    {
                        if (!(args.Result == null || args.Result.Rows == null))
                        {
                            totalCount = args.Result.TotalCount;
                            vmList = args.Result.Rows.ToList();
                        }
                        callback(totalCount, vmList);
                    }
                });
        }

        /// <summary>
        /// 货卡查询 - 商品总库存信息
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryItemsCardInventoryTotal(InventoryItemCardQueryVM model, Action<int, List<dynamic>> callback)
        {
            InventoryQueryFilter filter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            filter = model.ConvertVM<InventoryItemCardQueryVM, InventoryQueryFilter>();
            filter.IsShowTotalInventory = true;

            string relativeUrl = "/InventoryService/Inventory/QueryProductInventory";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    int totalCount = 0;
                    List<dynamic> vmList = null;
                    if (!args.FaultsHandle())
                    {
                        if (!(args.Result == null || args.Result.Rows == null))
                        {
                            totalCount = args.Result.TotalCount;
                            vmList = args.Result.Rows.ToList();
                        }
                        callback(totalCount, vmList);
                    }
                });
        }

        /// <summary>
        ///  货卡查询 - 单据信息
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryItemsCardItemOrders(InventoryItemCardQueryVM model, Action<int, List<dynamic>> callback)
        {
            InventoryItemCardQueryFilter filter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            filter = model.ConvertVM<InventoryItemCardQueryVM, InventoryItemCardQueryFilter>();

            string relativeUrl = "InventoryService/InventoryStock/QueryCardItemOrders";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    int totalCount = 0;
                    List<dynamic> vmList = null;
                    if (!args.FaultsHandle())
                    {
                        if (!(args.Result == null || args.Result.Rows == null))
                        {
                            totalCount = args.Result.TotalCount;
                            vmList = args.Result.Rows.ToList();
                        }
                        callback(totalCount, vmList);
                    }
                });
        }

        /// <summary>
        /// 导出查询结果
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columns"></param>
        public void ExportExcelForItemsCardOrders(InventoryItemCardQueryVM model, ColumnSet[] columns)
        {
            InventoryItemCardQueryFilter queryFilter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter = model.ConvertVM<InventoryItemCardQueryVM, InventoryItemCardQueryFilter>();
            string relativeUrl = "InventoryService/InventoryStock/QueryCardItemOrders";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }
    }
}
