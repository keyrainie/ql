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
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Inventory;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class AdventProductsFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Inventory", "ServiceBaseUrl");
            }
        }

        public AdventProductsFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public AdventProductsFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryAdventProductsList(AdventProductsQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "InventoryService/Inventory/QueryAdventProductsList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        public void UpdateProductRingInfo(ProductRingDayInfo entity, EventHandler<RestClientEventArgs<object>> callback)
        {
            entity.EditUser = CPApplication.Current.LoginUser.UserSysNo;
            entity.EditDate = DateTime.Now;
            string relativeUrl = "InventoryService/Inventory/UpdateProductRingDayInfo";
            restClient.Update(relativeUrl, entity, callback);
        }
        public void InsertProductRingInfo(ProductRingDayInfo entity, EventHandler<RestClientEventArgs<object>> callback)
        {
            entity.InUser = entity.EditUser = CPApplication.Current.LoginUser.UserSysNo;
            entity.InDate = DateTime.Now;
            entity.EditDate = null;
            string relativeUrl = "InventoryService/Inventory/AddProductRingDayInfo";
            restClient.Update(relativeUrl, entity, callback);
        }
    }
}
