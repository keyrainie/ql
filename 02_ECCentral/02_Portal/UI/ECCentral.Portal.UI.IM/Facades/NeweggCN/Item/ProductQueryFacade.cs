using System;
using System.Collections.Generic;
using System.Json;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;


namespace ECCentral.Portal.UI.IM.Facades
{
    partial class ProductQueryFacade
    {
        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void QueryProductEx(ProductQueryExVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            NeweggProductQueryFilter filter;
            filter = model.ConvertVM<ProductQueryExVM, NeweggProductQueryFilter>();
            model.ProductManufactureQuery.ConvertVM<ProductManufactureQueryFilterVM, ProductManufactureQueryFilter>();
            model.ProductPriceQuery.ConvertVM<ProductPriceQueryFilterVM, ProductPriceQueryFilter>();
            model.ProductInventoryQuery.ConvertVM<ProductInventoryQueryFilterVM, ProductInventoryQueryFilter>();
            model.ProductStatusQuery.ConvertVM<ProductStatusQueryFilterVM, ProductPriceQueryFilter>();
            model.OtherQuery.ConvertVM<OtherQueryFilterVM, OtherQueryFilter>();
            model.StockQuery.ConvertVM<StockQueryFilterVM, StockQueryFilter>();
            filter.EntryStatus = model.EntryStatus;
            filter.EntryStatusEx = model.EntryStatusEx;

            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            string relativeUrl = "/IMService/Product/QueryProductEx";
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
                            if (!ProductQueryExVM.HasDisplaycolumnPermission)
                            {
                                InitItem(item);
                            }
                         
                            
                      


                        }
                    }
                    callback(obj, args);
                }
                );
        }

        public void ExportAllProductToExcel(ProductQueryExVM queryVM, ColumnSet[] columnSet)
        {
            var queryFilter = queryVM.ConvertVM<ProductQueryExVM, NeweggProductQueryFilter>();
            //queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryVM.ProductManufactureQuery.ConvertVM<ProductManufactureQueryFilterVM, ProductManufactureQueryFilter>();
            queryVM.ProductPriceQuery.ConvertVM<ProductPriceQueryFilterVM, ProductPriceQueryFilter>();
            queryVM.ProductInventoryQuery.ConvertVM<ProductInventoryQueryFilterVM, ProductInventoryQueryFilter>();
            queryVM.ProductStatusQuery.ConvertVM<ProductStatusQueryFilterVM, ProductPriceQueryFilter>();
            queryVM.OtherQuery.ConvertVM<OtherQueryFilterVM, OtherQueryFilter>();
            queryVM.StockQuery.ConvertVM<StockQueryFilterVM, StockQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            string relativeUrl = "/IMService/Product/QueryProductEx";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);

        }

        private void InitItem(dynamic item)
       {
           if(item==null) return;
           item.POMemo = "*";
           item.POMemoShort = "*";
           item.IngramPurchasePrice = "*";
           item.VendorName = "*";
           item.LastPrice = "*";
           item.MinPackNumber = "*";
           item.VirtualPrice = "*";
           item.UnitCost = "*";
       }



        public void ExportTariffApply(List<int> productSysNos)
        {
            restClient.ExportFile("/IMService/Product/ExportTariffApply", productSysNos, "TariffApplyExporter");
        }

        public void ExportInspection(List<int> productSysNos)
        {
            restClient.ExportFile("/IMService/Product/ExportInspection", productSysNos, "InspectionExporter");
        }
    }
}
