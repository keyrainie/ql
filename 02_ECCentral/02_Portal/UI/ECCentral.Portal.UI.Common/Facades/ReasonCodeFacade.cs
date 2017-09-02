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
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Common.Facades
{
    public class ReasonCodeFacade
    {
        private IPage m_CurrentPage;
        //private readonly RestClient restClient;

        private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl");


        public ReasonCodeFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }



        public ReasonCodeFacade(IPage page)
        {
            m_CurrentPage = page;
            restClient = new RestClient(serviceBaseUrl, page);
        }

        public void GetReasonCodeList(int level, EventHandler<RestClientEventArgs<List<ReasonCodeEntity>>> callback)
        {
            string relativeUrl = string.Format("CommonService/ReasonCode/GetReasonCodeByNodeLevel/{0}", level);

            restClient.Query<List<ReasonCodeEntity>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<ReasonCodeEntity> list = args.Result;
                RestClientEventArgs<List<ReasonCodeEntity>> eventArgs = new RestClientEventArgs<List<ReasonCodeEntity>>(list, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }

        public void InsertReasonCode(ReasonCodeEntity reason, EventHandler<RestClientEventArgs<ReasonCodeEntity>> callback)
        {
            string relativeUrl = "CommonService/ReasonCode/InsertReasonCode";

            restClient.Create<ReasonCodeEntity>(relativeUrl, reason, callback);
        }

        public void UpdateReasonCode(ReasonCodeEntity reason, EventHandler<RestClientEventArgs<ReasonCodeEntity>> callback)
        {
            string relativeUrl = "CommonService/ReasonCode/UpdateReasonCode";
            restClient.Update<ReasonCodeEntity>(relativeUrl, reason, callback);
        }

        public void UpdateReasonStatusList(List<ReasonCodeEntity> reason, EventHandler<RestClientEventArgs<List<ReasonCodeEntity>>> callback)
        {
            string relativeUrl = "CommonService/ReasonCode/UpdateReasonStatusList";
            restClient.Update<List<ReasonCodeEntity>>(relativeUrl, reason, callback);
        }


    }
}
