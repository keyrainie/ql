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
using ECCentral.BizEntity.Customer;
using ECCentral.QueryFilter.Customer;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Customer.NeweggCN.Models;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.Customer.NeweggCN.Facades
{
    public class QCSubjectFacade
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

        public QCSubjectFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void LoadTree(string channelID, QCSubjectStatus? status, EventHandler<RestClientEventArgs<QCSubject>> callback)
        {
            QCSubjectFilter filter = new QCSubjectFilter();
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.ChannelID = channelID;
            filter.Status = status;
            filter.PagingInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = 0,
                PageSize = int.MaxValue
            };
            string relativeUrl = "/CustomerService/QCSubject/GetQCSubjectTree";
            restClient.Query<QCSubject>(relativeUrl, filter, callback);
        }
        public void GetParents(QCSubjectVM vm, EventHandler<RestClientEventArgs<List<QCSubject>>> callback)
        {
            QCSubject filter = vm.ConvertVM<QCSubjectVM, QCSubject>();
            string relativeUrl = "/CustomerService/QCSubject/GetParent";
            restClient.Query<List<QCSubject>>(relativeUrl, filter, callback);
        }
        public void Update(QCSubjectVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            QCSubject filter = vm.ConvertVM<QCSubjectVM, QCSubject>();
            string relativeUrl = "/CustomerService/QCSubject/Update";
            restClient.Update(relativeUrl, filter, callback);
        }

        public void Create(QCSubjectVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            QCSubject filter = vm.ConvertVM<QCSubjectVM, QCSubject>();
            string relativeUrl = "/CustomerService/QCSubject/Create";
            restClient.Create(relativeUrl, filter, callback);
        }
    }
}
