using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Common.Models;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.Basic.Components.UserControls.TariffPicker
{
    public class TariffFacade
    {
        private readonly RestClient restClient;
        public TariffFacade(IPage page)
        {
            restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl"), page);
        }
        public void QueryTariffCategory(string tariffcode, EventHandler<RestClientEventArgs<List<TariffInfo>>> callback)
        {
            string relativeUrl = string.Format("/CommonService/Tariff/QueryTariffCategory/{0}", tariffcode);

            restClient.Query<List<TariffInfo>>(relativeUrl, callback);
            //restClient.Query<List<TariffInfo>>(relativeUrl, null, callback);
        }

        /// <summary>
        /// 税率规则信息查询控件
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void TariffInfoQuery(TariffInfoQueryFilterVM queryVM, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            TariffInfoQueryFilter queryFilter = new TariffInfoQueryFilter();
            queryFilter = queryVM.ConvertVM<TariffInfoQueryFilterVM, TariffInfoQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            queryFilter.SearchCode = 0;
            string relativeUrl = "/CommonService/Tariff/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
    }
}
