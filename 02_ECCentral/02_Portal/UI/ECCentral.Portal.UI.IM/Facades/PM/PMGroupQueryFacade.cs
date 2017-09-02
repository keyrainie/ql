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
using ECCentral.QueryFilter.IM.ProductManager;
using ECCentral.BizEntity.IM;


namespace ECCentral.Portal.UI.IM.Facades
{
    public class PMGroupQueryFacade
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

        public PMGroupQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public PMGroupQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryPMGroup(PMGroupQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductManagerGroupQueryFilter filter;
            filter = model.ConvertVM<PMGroupQueryVM, ProductManagerGroupQueryFilter>();

            filter.PMGroupName = model.PMGroupName;
            PMGroupStatus statusValue;
            Enum.TryParse(model.Status, out statusValue);
            filter.Status = statusValue;

            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            string relativeUrl = "/IMService/ProductManagerGroup/QueryProductManagerGroupInfo";
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
