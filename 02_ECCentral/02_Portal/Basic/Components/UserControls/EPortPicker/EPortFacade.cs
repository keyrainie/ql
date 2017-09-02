using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
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

namespace ECCentral.Portal.Basic.Components.UserControls.EPortPicker
{
    public class EPortFacade
    {
                private IPage m_CurrentPage;
        private readonly RestClient restClient;

        public EPortFacade(IPage page)
        {
            m_CurrentPage = page;
            restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl"), page);
        }

        public void GetEPortList(EventHandler<RestClientEventArgs<List<EPortEntity>>> callback)
        {
            string relativeUrl = "/POService/EPort/GetAllEPort";

            restClient.Query<List<EPortEntity>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<EPortEntity> payTypeList = args.Result;
                RestClientEventArgs<List<EPortEntity>> eventArgs = new RestClientEventArgs<List<EPortEntity>>(payTypeList, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }
    }
}
