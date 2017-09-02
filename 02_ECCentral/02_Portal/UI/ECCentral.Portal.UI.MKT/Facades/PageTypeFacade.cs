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
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.MKT.PageType;
using ECCentral.Portal.UI.MKT.Models.PageType;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class PageTypeFacade
    {
        private readonly RestClient restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public PageTypeFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void GetPageTypes(string companyCode, string channelID, int moduleID, EventHandler<RestClientEventArgs<List<CodeNamePair>>> callback)
        {
            string relativeUrl = string.Format("/MKTService/PageType/{0}/{1}/{2}", companyCode, channelID, moduleID);
            restClient.Query<List<CodeNamePair>>(relativeUrl, callback);

        }

        public void GetPages(string companyCode, string channelID, int moduleID, string pageTypeID, EventHandler<RestClientEventArgs<PageResult>> callback)
        {
            string relativeUrl = string.Format("/MKTService/PageType/GetPages/{0}/{1}/{2}/{3}", companyCode, channelID, moduleID, pageTypeID);
            restClient.Query<PageResult>(relativeUrl, callback);

        }

        public void Create(PageTypeVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var data = vm.ConvertVM<PageTypeVM, PageType>();
            data.CompanyCode = CPApplication.Current.CompanyCode;
            data.WebChannel = new WebChannel
            {
                ChannelID = vm.ChannelID
            };
            data.InUser = CPApplication.Current.LoginUser.DisplayName;
            data.EditUser = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/MKTService/PageType/Create";
            restClient.Create(relativeUrl, data, callback);
        }

        public void Update(PageTypeVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var data = vm.ConvertVM<PageTypeVM, PageType>();
            data.CompanyCode = CPApplication.Current.CompanyCode;
            data.WebChannel = new WebChannel
            {
                ChannelID = vm.ChannelID
            };
            data.InUser = CPApplication.Current.LoginUser.DisplayName;
            data.EditUser = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/MKTService/PageType/Update";
            restClient.Update(relativeUrl, data, callback);
        }

        public void Load(int sysNo, EventHandler<RestClientEventArgs<PageType>> callback)
        {
            string relativeUrl = "/MKTService/PageType/" + sysNo.ToString();
            restClient.Query(relativeUrl, callback);
        }

        public void Query(PageTypeQueryVM vm, PagingInfo pagingInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<PageTypeQueryVM, PageTypeQueryFilter>();
            data.PageInfo = pagingInfo;
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/MKTService/PageType/Query";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }

    }
}
