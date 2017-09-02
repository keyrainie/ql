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
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;

namespace ECCentral.Portal.Basic.Components.UserControls.CurrencyTypePicker
{
    public class CurrencyTypeFacade
    {
         private IPage m_CurrentPage;
        private readonly RestClient restClient;

        public CurrencyTypeFacade(IPage page)
        {
            m_CurrentPage = page;
            restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl"), page);
        }

        public void GetCurrencyList(EventHandler<RestClientEventArgs<List<CurrencyInfo>>> callback)
        {
            string relativeUrl = "CommonService/CurrencyType/GetAll";

            restClient.Query<List<CurrencyInfo>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<CurrencyInfo> currencyList = args.Result;
                RestClientEventArgs<List<CurrencyInfo>> eventArgs = new RestClientEventArgs<List<CurrencyInfo>>(currencyList, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }
    }
}
