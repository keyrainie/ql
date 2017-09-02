using System;
using System.Collections.Generic;

using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.Basic.Components.UserControls.ReasonCodePicker
{
    public class ReasonCodeFacade
    {
        private IPage m_CurrentPage;
        private readonly RestClient restClient;

        public ReasonCodeFacade(IPage page)
        {
            m_CurrentPage = page;
            restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl"), page);
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
                List<ReasonCodeEntity> payTypeList = args.Result;
                RestClientEventArgs<List<ReasonCodeEntity>> eventArgs = new RestClientEventArgs<List<ReasonCodeEntity>>(payTypeList, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }
    }
}