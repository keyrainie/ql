using System;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.SO;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.SO;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.SO.Facades
{
    public class SOComplainFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_SO, ConstValue.Key_ServiceBaseUrl);
            }
        }

        public SOComplainFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public SOComplainFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl,page);
        }
        #region [Query Methods]

        #endregion

        public void Create(SOComplaintCotentInfo req , EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            req.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Create("/SOService/SO/CreateComplain", req, callback);
        }

        public void Update(SOComplaintInfo req, EventHandler<RestClientEventArgs<SOComplaintInfo>> callback)
        {
            restClient.Update("/SOService/SO/UpdateComplain", req, callback);
        }

        public void CancelAssign(List<SOComplaintProcessInfo> req, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update("/SOService/SO/BathCancelAssignSOComplaintInfo", req, callback);
        }

        public void Assign(List<SOComplaintProcessInfo> req, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update("/SOService/SO/BathAssignSOComplaintInfo", req, callback);
        }

        public void Get(int sysNo, EventHandler<RestClientEventArgs<SOComplaintInfo>> callback)
        {
            restClient.Query<SOComplaintInfo>("/SOService/SO/Complain/" + sysNo.ToString(), callback);
        }

        /// <summary>
        /// 发送回馈邮件
        /// </summary>
        /// <param name="complainReply"></param>
        public void SendReplyEmail(string complainReply, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Query<dynamic>("/SOService/SO/SendReplyEmail/" + complainReply, callback);
        }
    }
}
