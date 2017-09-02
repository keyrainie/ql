using System;
using System.Collections.Generic;

using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.Basic.Components.UserControls.PayTypePicker
{
    public class PayTypeFacade
    {
        private IPage m_CurrentPage;
        private readonly RestClient restClient;

        public PayTypeFacade(IPage page)
        {
            m_CurrentPage = page;
            restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl"), page);
        }

        public void GetPayTypeList(EventHandler<RestClientEventArgs<List<PayType>>> callback)
        {
            string relativeUrl = "CommonService/PayType/GetAll/{0}";

            restClient.Query<List<PayType>>(string.Format(relativeUrl,CPApplication.Current.CompanyCode), (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<PayType> payTypeList = args.Result;
                RestClientEventArgs<List<PayType>> eventArgs = new RestClientEventArgs<List<PayType>>(payTypeList, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }
    }
}