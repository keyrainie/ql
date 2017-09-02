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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.IM.ProductManager;
using ECCentral.BizEntity.IM;
using System.Collections.Generic;

namespace ECCentral.Portal.Basic.Components.UserControls.PMPicker
{
    public class PMGroupQueryFacade
    {
        private readonly RestClient restClient;

        public PMGroupQueryFacade(IPage page)
        {
            restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Invoice", "ServiceBaseUrl") + "/InvoiceService", page);
        }

        /// <summary>
        /// 查询PMGroup
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryPMGroupList(ProductManagerGroupQueryFilter queryFilter, EventHandler<RestClientEventArgs<List<ProductManagerGroupInfo>>> callback)
        {
            string relativeUrl = "/ProductManagerGroup/QueryProductManagerGroupInfo";
            restClient.Query<List<ProductManagerGroupInfo>>(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 获取PMGroup信息
        /// </summary>
        /// <param name="callback"></param>
        public void GetPMGroup(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "Invoice/GetPMGroup";
            restClient.QueryDynamicData(relativeUrl, callback);
        }
    }
}
