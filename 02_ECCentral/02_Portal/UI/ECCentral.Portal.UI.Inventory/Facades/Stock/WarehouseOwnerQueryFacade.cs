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
    public class WarehouseOwnerQueryFacade
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

        public WarehouseOwnerQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public WarehouseOwnerQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryWarehouseOwner(WarehouseOwnerQueryVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            WarehouseOwnerQueryFilter filter = model.ConvertVM<WarehouseOwnerQueryVM, WarehouseOwnerQueryFilter>();
            string relativeUrl = "/InventoryService/Stock/QueryWarehouseOwner";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {

                    if (!args.FaultsHandle())
                    {
                        foreach (var item in args.Result.Rows)
                        {
                            item.IsChecked = false;
                        }
                        callback(obj, args);
                    }
                });
        }

        public void GetWarehouseOwnerByCompanyCode(string companyCode, Action<List<WarehouseOwnerInfoVM>> callback)
        {
            string relativeUrl = "/InventoryService/Stock/WarehouseOwner/GetByCompanyCode";
            restClient.Query<List<WarehouseOwnerInfo>>(relativeUrl, companyCode, (obj, args) =>
            {
                if (!args.FaultsHandle() && callback != null)
                {
                    List<WarehouseOwnerInfoVM> vmList=new List<WarehouseOwnerInfoVM>();
                    var info =EntityConverter<WarehouseOwnerInfo, WarehouseOwnerInfoVM>.Convert(args.Result);

                    foreach (var item in info)//中蛋定制化 只获取中蛋的所有者
                    {
                        if(item.OwnerType == WarehouseOwnerType.Self)
                        {
                            vmList.Add(item);
                        }                        
                    }
                    callback(vmList);
                }
            });
        }

        /// <summary>
        /// 导出全部
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columns"></param>
        public void ExportExcelForWarehouseOwnerQuery(WarehouseOwnerQueryVM model, ColumnSet[] columns)
        {
            WarehouseOwnerQueryFilter queryFilter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter = model.ConvertVM<WarehouseOwnerQueryVM, WarehouseOwnerQueryFilter>();         
            string relativeUrl = "/InventoryService/Stock/QueryWarehouseOwner";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }
    }
}
