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
using ECCentral.QueryFilter.PO;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Models;
using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.PO.Restful.RequestMsg;

namespace ECCentral.Portal.UI.PO.Facades
{
    public class SettleFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }

        public SettleFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询供应商列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void QuerySettle(SettleQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/GatherSettlement/QuerySettleList";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }
    }
}
