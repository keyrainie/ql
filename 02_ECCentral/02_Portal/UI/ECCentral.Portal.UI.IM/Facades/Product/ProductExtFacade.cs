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
using ECCentral.QueryFilter.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.BizEntity.IM;
using System.Collections.Generic;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductExtFacade
    {
        private readonly RestClient restClient;
        private const string GetItemExtUrl = "/IMService/ProductExt/GetProductExtByQuery";
        private const string UpdatePermitRefundUrl = "/IMService/ProductExt/UpdatePermitRefund";
        /// <summary>
        /// IMService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

        public ProductExtFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductExtFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        public void GetProductExtByQuery(ProductExtQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {

            ProductExtQueryFilter query;
            query = model.ConvertVM<ProductExtQueryVM, ProductExtQueryFilter>();
            query.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            restClient.QueryDynamicData(GetItemExtUrl, query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }


                callback(obj, args);
            });
        }
        public void UpdatePermitRefund(List<ProductExtVM> list, EventHandler<RestClientEventArgs<ProductExtInfo>> callback)
        {
            List<ProductExtInfo> listinfo = new List<ProductExtInfo>();
            foreach (ProductExtVM item in list)
            {
                listinfo.Add(new ProductExtInfo() { IsPermitRefund = item.IsPermitRefund, SysNo = item.SysNo });
            }
            restClient.Update(UpdatePermitRefundUrl, listinfo, callback);
        }

        public void UpdateBatchManagementInfo(ProductMaintainBatchManagementInfoVM vm, EventHandler<RestClientEventArgs<ProductBatchManagementInfo>> callback)
        {
            var msg = EntityConverter<ProductMaintainBatchManagementInfoVM, ProductBatchManagementInfo>.Convert(vm);
            //if (!string.IsNullOrEmpty(vm.Note))
            //{
            //    msg.Logs = new List<ProductBatchManagementInfoLog>();
            //    msg.Logs.Add(new ProductBatchManagementInfoLog { Note = vm.Note, BatchManagementSysNo = msg.SysNo });
            //}
            restClient.Update("/IMService/ProductExt/UpdateBatchManagementInfo", msg, callback);
        }
    }
}
