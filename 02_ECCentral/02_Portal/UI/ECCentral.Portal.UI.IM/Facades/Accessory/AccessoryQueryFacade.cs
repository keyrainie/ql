using System;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;


namespace ECCentral.Portal.UI.IM.Facades
{
    public class AccessoryQueryFacade
    {
        private readonly RestClient restClient;

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

        public AccessoryQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public AccessoryQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryAccessory(AccessoryQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            AccessoryQueryFilter filter;
            filter = model.ConvertVM<AccessoryQueryVM, AccessoryQueryFilter>();

            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            string relativeUrl = "/IMService/Accessory/Query";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    callback(obj, args);
                }
                );
        }
    }
}
