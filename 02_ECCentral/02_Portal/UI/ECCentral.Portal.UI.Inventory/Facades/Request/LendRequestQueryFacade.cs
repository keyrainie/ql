using System;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class LendRequestQueryFacade
    {
        private readonly RestClient restClient;

        /// <summary>
        /// InventoryService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Inventory, ConstValue.Key_ServiceBaseUrl);
            }
        }

        public LendRequestQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public LendRequestQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryLendRequest(LendRequestQueryVM model, Action<int, List<dynamic>> callback)
        {
            LendRequestQueryFilter filter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            filter = model.ConvertVM<LendRequestQueryVM, LendRequestQueryFilter>();

            string relativeUrl = "/InventoryService/LendRequest/QueryLendRequest";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        List<dynamic> result = null;
                        int totalCount = 0;
                        if (!(args.Result == null || args.Result.Rows == null))
                        {
                            result = args.Result.Rows.ToList();
                            totalCount = args.Result.TotalCount;
                        }
                        if (callback != null)
                        {
                            callback(totalCount, result);
                        }
                    }
                });
        }

        public void QueryLendRequest(LendRequestQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            LendRequestQueryFilter filter;
            filter = model.ConvertVM<LendRequestQueryVM, LendRequestQueryFilter>();

            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            string relativeUrl = "/InventoryService/LendRequest/QueryLendRequest";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    if (!(args == null || args.Result == null || args.Result.Rows == null))
                    {
                        foreach (var item in args.Result.Rows)
                        {
                            item.IsChecked = false;
                        }
                    }
                    callback(obj, args);
                }
                );
        }

        /// <summary>
        /// 导出查询结果
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columns"></param>
        public void ExportExcelForLendRequest(LendRequestQueryVM model, ColumnSet[] columns)
        {
            LendRequestQueryFilter queryFilter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter = model.ConvertVM<LendRequestQueryVM, LendRequestQueryFilter>();
            string relativeUrl = "/InventoryService/LendRequest/QueryLendRequest";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

        /// <summary>
        /// 按照PM导出查询结果
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columns"></param>
        public void ExportExcelForLendRequestByPM(LendRequestQueryVM model, ColumnSet[] columns)
        {
            LendRequestQueryFilter queryFilter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter = model.ConvertVM<LendRequestQueryVM, LendRequestQueryFilter>();
            string relativeUrl = "/InventoryService/LendRequest/ExportAllByPM";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

        /// <summary>
        /// 统计当前条件下的借货单初始状态和作废状态的成本
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public void QueryLendCostbyStatus(LendRequestQueryVM model, Action<List<dynamic>> callback)
        {
            LendRequestQueryFilter filter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            filter = model.ConvertVM<LendRequestQueryVM, LendRequestQueryFilter>();
            string relativeUrl = "/InventoryService/LendRequest/QueryLendCostbyStatus";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    List<dynamic> result = null;                    
                    if (!(args.Result == null || args.Result.Rows == null))
                    {
                        result = args.Result.Rows.ToList();
                    }
                    if (callback != null)
                    {
                        callback(result);
                    }
                }
            });         
        }
    }
}
