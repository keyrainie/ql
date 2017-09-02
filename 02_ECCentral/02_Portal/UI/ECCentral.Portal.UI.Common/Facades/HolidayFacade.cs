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
    public class HolidayFacade
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

        public HolidayFacade(IPage page)
        {
            this.Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryHolidayList(HolidayQueryFilterVM filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/Holiday/QueryHoliday";

            var msg = filter.ConvertVM<HolidayQueryFilterVM, HolidayQueryFilter>();

            restClient.QueryDynamicData(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(this, new RestClientEventArgs<dynamic>(args.Result, this.Page));
            });
        }

        public void CreateHoliday(HolidayVM _viewInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/Holiday/Create";
            var msg = _viewInfo.ConvertVM<HolidayVM, Holiday>();
            msg.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            msg.CreateUserName = CPApplication.Current.LoginUser.LoginName;
            restClient.Create(relativeUrl, msg, callback);
        }

        public void DeleteHolidayBatch(List<int?> sysNos,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/Holiday/Delete";
            restClient.Delete(relativeUrl, sysNos, callback);
        }

    }
}
