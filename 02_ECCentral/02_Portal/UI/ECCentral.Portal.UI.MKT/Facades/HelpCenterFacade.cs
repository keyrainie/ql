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
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class HelpCenterFacade
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

        public HelpCenterFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(HelpCenterQueryVM vm ,PagingInfo pagingInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<HelpCenterQueryVM, HelpCenterQueryFilter>();
            data.PageInfo = pagingInfo;
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/MKTService/HelpCenter/Query";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }

        public void QueryCategory(string companyCode,string channelID,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            HelpCenterCategoryQueryFilter filter = new HelpCenterCategoryQueryFilter();
            filter.CompanyCode = companyCode;
            filter.ChannelID = channelID;
            //filter.Status = "A";//表示有效
            string relativeUrl = "/MKTService/HelpCenter/QueryCategory";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void Create(HelpCenterVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var entity = vm.ConvertVM<HelpCenterVM, HelpTopic>();
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            entity.WebChannel = new BizEntity.Common.WebChannel
            {
                ChannelID=vm.ChannelID
            };
            string relativeUrl = "/MKTService/HelpCenter/Create";
            restClient.Create(relativeUrl, entity, callback);
        }

        public void Update(HelpCenterVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var entity = vm.ConvertVM<HelpCenterVM, HelpTopic>();
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            entity.WebChannel = new BizEntity.Common.WebChannel
            {
                ChannelID=vm.ChannelID
            };
            string relativeUrl = "/MKTService/HelpCenter/Update";
            restClient.Update(relativeUrl, entity, callback);
        }

        public void Load(string sysNo, EventHandler<RestClientEventArgs<HelpTopic>> callback)
        {
            string relativeUrl = "/MKTService/HelpCenter/" + sysNo;
            restClient.Query(relativeUrl, callback);
        }
    }
}
