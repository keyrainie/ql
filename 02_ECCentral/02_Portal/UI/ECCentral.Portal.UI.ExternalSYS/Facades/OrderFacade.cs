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
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Portal.UI.ExternalSYS.Facades
{
    public class OrderFacade
    {
         private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.Key_ServiceBaseUrl);

        public OrderFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public OrderFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }

        public void Query(OrderQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback) 
        {
            string GetProductLineByQueryUrl = "ExternalSYSService/Order/OrderQuery";

            OrderQueryFilter filter = model.ConvertVM<OrderQueryVM, OrderQueryFilter>();
            filter.PageInfo = new QueryFilter.Common.PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            restClient.QueryDynamicData(GetProductLineByQueryUrl, filter, callback);
        }

    }
}
