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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.Customer;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class ShipTypeSMSTemplateFacade
    {
        private readonly RestClient restClient;
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }
        public ShipTypeSMSTemplateFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }
        public ShipTypeSMSTemplateFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);

        }
        public void Query(ShipTypeSMSTemplateQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/ShipTypeSMSTemplate/Query";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }
        /// <summary>
        /// 添加 SMSTemplate
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void Create(SMSTemplate request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/ShipTypeSMSTemplate/Create";
            restClient.Create(relativeUrl, request, callback);
        }
        /// <summary>
        /// 更新 SMSTemplate
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void Update(SMSTemplate request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/ShipTypeSMSTemplate/Update";
            restClient.Update(relativeUrl, request, callback);
        }

    }
    public class ShipTypeSMSQueryFacade
    {
        private readonly RestClient restClient;
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }
        public ShipTypeSMSQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }
        public ShipTypeSMSQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);

        }
        public void Query(ShipTypeSMSQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/ShipTypeSMS/Query";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }
        /// <summary>
        /// 添加 ShipTypeSMS
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void Create(ShipTypeSMS request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/ShipTypeSMS/Create";
            restClient.Create(relativeUrl, request, callback);
        }
        /// <summary>
        /// 更新 ShipTypeSMS
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void Update(ShipTypeSMS request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/ShipTypeSMS/Update";
            restClient.Update(relativeUrl, request, callback);
        }
    }
}
