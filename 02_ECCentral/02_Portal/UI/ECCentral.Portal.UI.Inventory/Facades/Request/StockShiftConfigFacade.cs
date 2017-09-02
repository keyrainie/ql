using System;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.Basic;
using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class StockShiftConfigFacade
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

        public StockShiftConfigFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(StockShiftConfigQueryVM vm, Action<int, List<dynamic>> callback)
        {
            vm.CompanyCode = CPApplication.Current.CompanyCode;
            StockShiftConfigFilter filter;
            filter = vm.ConvertVM<StockShiftConfigQueryVM, StockShiftConfigFilter>();
            string relativeUrl = "/InventoryService/StockShiftConfig/Query";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        int totalCount = 0;
                        List<dynamic> vmList = null;
                        if (!(args.Result == null || args.Result.Rows == null))
                        {
                            totalCount = args.Result.TotalCount;
                            vmList = args.Result.Rows.ToList();
                        }
                        callback(totalCount, vmList);
                    }
                });
        }

        public void Create(StockShiftConfigVM vm, Action<StockShiftConfigVM> callback)
        {
            vm.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InventoryService/StockShiftConfig/Create";
            restClient.Create<StockShiftConfigInfo>(relativeUrl, vm.ConvertVM<StockShiftConfigVM, StockShiftConfigInfo>(), (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    vm = null;
                    if (args.Result != null)
                    {
                        vm = args.Result.Convert<StockShiftConfigInfo, StockShiftConfigVM>();
                    }
                    if (callback != null)
                    {
                        callback(vm);
                    }
                }
            });
        }

        public void Update(StockShiftConfigVM vm, Action callback)
        {
            string relativeUrl = "/InventoryService/StockShiftConfig/Update";
            restClient.Update(relativeUrl, vm.ConvertVM<StockShiftConfigVM, StockShiftConfigInfo>(), (obj, args) =>
            {
                if (!args.FaultsHandle() && callback != null)
                {
                    callback();
                }
            });
        }

        public void ExportShiftConfigInfo(StockShiftConfigQueryVM vm, ColumnSet[] columns)
        {
            StockShiftConfigFilter queryFilter;
            vm.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter = vm.ConvertVM<StockShiftConfigQueryVM, StockShiftConfigFilter>();
            string relativeUrl = "/InventoryService/StockShiftConfig/Query";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }
        
    }
}
