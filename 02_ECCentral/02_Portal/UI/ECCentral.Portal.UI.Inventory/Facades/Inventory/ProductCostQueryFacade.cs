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
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class ProductCostQueryFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Inventory", "ServiceBaseUrl");
            }
        }

        public ProductCostQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryProductCostInList(ProductCostQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "InventoryService/Inventory/QueryProductCostInList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        public void BatchUpdateProductCostPriority(List<ProductCostInfo> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string BatchUpdateProductCostPriorityUrl = "/InventoryService/Inventory/BatchUpdateProductCostPriority";
            restClient.Update(BatchUpdateProductCostPriorityUrl, list, callback);
        }

    }
}
