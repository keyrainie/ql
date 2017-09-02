using System;
using System.Collections.Generic;

using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.Basic.Components.UserControls.ShipTypePicker
{
    public class ShipTypeFacade
    {
        private IPage m_CurrentPage;
        private readonly RestClient restClient;

        public ShipTypeFacade(IPage page)
        {
            m_CurrentPage = page;
            restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl"), page);
        }

        public void GetShipTypeList(EventHandler<RestClientEventArgs<List<ShippingType>>> callback)
        {
            string relativeUrl = string.Format("/CommonService/ShippingType/GetAll/{0}", CPApplication.Current.CompanyCode);
            restClient.Query<List<ShippingType>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<ShippingType> shippingTypeList = args.Result;
                RestClientEventArgs<List<ShippingType>> eventArgs = new RestClientEventArgs<List<ShippingType>>(shippingTypeList, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }

    }
}