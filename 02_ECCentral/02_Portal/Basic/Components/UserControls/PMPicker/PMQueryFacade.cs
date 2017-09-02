using System;
using System.Collections.Generic;

using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.IM;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.Basic.Components.UserControls.PMPicker
{
    public class PMQueryFacade
    {
        private readonly RestClient restClient;

        public PMQueryFacade(IPage page)
        {
            restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl") + "/IMService", page);

        }
        /// <summary>
        /// 查询PM List
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryPMList(ProductManagerQueryFilter queryFilter, EventHandler<RestClientEventArgs<List<ProductManagerInfo>>> callback)
        {
            string relativeUrl = "/PM/QueryPMList";
            restClient.Query<List<ProductManagerInfo>>(relativeUrl, queryFilter, callback);
        }       
    }
}
