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

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class WarehouseQueryFacade
    {
        private readonly RestClient restClient;

        /// <summary>
        /// IMService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Inventory, ConstValue.Key_ServiceBaseUrl);
            }
        }

        public WarehouseQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public WarehouseQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryWarehouse(WarehouseQueryVM model, Action<int, List<WarehouseInfoVM>> callback)
        {
            WarehouseQueryFilter filter;
            filter = model.ConvertVM<WarehouseQueryVM, WarehouseQueryFilter>();
            string relativeUrl = "/InventoryService/Stock/QueryWarehouse";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (!(args == null || args.Result == null || args.Result.Rows == null) && !args.FaultsHandle() && callback != null)
                    {
                        DynamicConverter<WarehouseInfoVM>.ConvertToVMList(args.Result.Rows);
                        callback((int)args.Result.TotalCount, DynamicConverter<WarehouseInfoVM>.ConvertToVMList(args.Result.Rows));
                    }
                });
        }

        /// <summary>
        /// 查询Warehouse List
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void GetWarehouseListByCompanyCode(string companyCode, Action<List<WarehouseInfoVM>> callback)
        {
            string relativeUrl = "/InventoryService/Stock/WH/GetByCompanyCode";
            restClient.Query<List<WarehouseInfo>>(relativeUrl, companyCode, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    callback(EntityConverter<WarehouseInfo, WarehouseInfoVM>.Convert(args.Result));
                }
            });
        }

        /// <summary>
        /// 导出全部
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columns"></param>
        public void ExportExcelForWarehouseQuery(WarehouseQueryVM model, ColumnSet[] columns)
        {
            WarehouseQueryFilter queryFilter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter = model.ConvertVM<WarehouseQueryVM, WarehouseQueryFilter>();
            string relativeUrl = "/InventoryService/Stock/QueryWarehouse";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }
    }
}
