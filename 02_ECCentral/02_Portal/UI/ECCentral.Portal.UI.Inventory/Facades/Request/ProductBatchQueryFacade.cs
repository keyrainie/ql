using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Inventory.Request;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
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
using ECCentral.Portal.UI.Inventory.Models.Inventory;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.Facades.Request
{
    public class ProductBatchQueryFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Inventory", "ServiceBaseUrl");
            }
        }

        public ProductBatchQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询商品批次信息
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryProductBatchInfo(ProductBatchQueryFilter filter,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "InventoryService/Inventory/QueryProductBacthInfo";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 修改批次信息状态
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        public void UpdateProductBatchStatus(InventoryBatchDetailsInfo productBatchInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string UpdateProductBatchStatusUrl = "/InventoryService/Inventory/UpdateProductBatchStatus";
            restClient.Update(UpdateProductBatchStatusUrl, productBatchInfo, callback);
        }

    }
}
