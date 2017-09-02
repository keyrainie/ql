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
using ECCentral.Portal.UI.Common.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Common.Facades
{
    public class LogFacade
    {
         private readonly RestClient restClient;

         public IPage Page { get; set; }
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public LogFacade(IPage page)
        {
            this.Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryLogList(LogQueryFilterVM filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = string.Empty;
            if (filter.ISSOLog)
            {
                relativeUrl = "/CommonService/Log/QuerySOLog";
            }
            else
            {
                if (filter.CancelOutStore)
                    relativeUrl = "/CommonService/Log/QuerySysLog";
                else
                    relativeUrl = "/CommonService/Log/QuerySysLogWithOutCancelOutStore";
            }

            var msg = filter.ConvertVM<LogQueryFilterVM, LogQueryFilter>();
            restClient.QueryDynamicData(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(this, new RestClientEventArgs<dynamic>(args.Result, this.Page));
            });
        }
    }
}
