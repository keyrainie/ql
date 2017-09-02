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

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.UI.Inventory.Models;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class ItemAllocatedCardQueryFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Inventory", "ServiceBaseUrl");
            }
        }

        public ItemAllocatedCardQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }   

        /// <summary>
        /// 已分配查询  - 商品渠道仓库存信息
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryItemAllocatedCardInventoryByStock(InventoryAllocatedCardQueryVM model, Action<int, List<dynamic>> callback)
        {
            InventoryQueryFilter filter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            filter = model.ConvertVM<InventoryAllocatedCardQueryVM, InventoryQueryFilter>();
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
        /// 已分配查询  - 商品总库存信息
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryItemAllocatedCardInventoryTotal(InventoryAllocatedCardQueryVM model, Action<int, List<dynamic>> callback)
        {
            InventoryQueryFilter filter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            filter = model.ConvertVM<InventoryAllocatedCardQueryVM, InventoryQueryFilter>();
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
        ///  已分配查询 - 单据信息
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryItemAllocatedCardtemOrders(InventoryAllocatedCardQueryVM model, Action<int, List<dynamic>> callback)
        {
            InventoryAllocatedCardQueryFilter filter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            filter = model.ConvertVM<InventoryAllocatedCardQueryVM, InventoryAllocatedCardQueryFilter>();

            string relativeUrl = "InventoryService/InventoryStock/QueryAllocatedItemOrders";
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
        public void ExportExcelForItemAllocatedCardOrders(InventoryAllocatedCardQueryVM model, ColumnSet[] columns)
        {
            InventoryAllocatedCardQueryFilter queryFilter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter = model.ConvertVM<InventoryAllocatedCardQueryVM, InventoryAllocatedCardQueryFilter>();            
            string relativeUrl = "InventoryService/InventoryStock/QueryAllocatedItemOrders";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

    }
}
