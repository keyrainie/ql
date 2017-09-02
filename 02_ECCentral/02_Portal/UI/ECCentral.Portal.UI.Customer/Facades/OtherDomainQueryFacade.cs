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
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice.Refund;
using ECCentral.Portal.UI.Customer.Models;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class OtherDomainQueryFacade
    {
        private IPage m_CurrentPage;

        private RestClient GetRestClient(string domainName)
        {
            string baseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(domainName, ConstValue.Key_ServiceBaseUrl);
            RestClient restClient = new RestClient(baseUrl, m_CurrentPage);
            return restClient;
        }

        public OtherDomainQueryFacade(IPage page)
        {
            m_CurrentPage = page;
        }

        public OtherDomainQueryFacade()
            : this(CPApplication.Current.CurrentPage)
        {

        }

        #region Customer

        #endregion

        #region RMA


        #endregion

        #region Invoice

        public void Create(PrepayRefundVM request, EventHandler<RestClientEventArgs<object>> callback)
        {
            BalanceRefundInfo entity = request.ConvertVM<PrepayRefundVM, BalanceRefundInfo>();
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/BalanceRefund/Create";
            GetRestClient(ConstValue.DomainName_Invoice).Create(relativeUrl, entity, callback);
        }

        #endregion

        #region MKT

        #endregion

        #region IM

        public void QueryCategoryC1ByProductID(string productID, EventHandler<RestClientEventArgs<ECCentral.BizEntity.IM.CategoryInfo>> callback)
        {
            GetRestClient(ConstValue.DomainName_IM).Query<ECCentral.BizEntity.IM.CategoryInfo>("/IMService/Category/GetProductC1CategoryDomain/" + productID, callback);
        }

        #endregion

        #region Common

        public void GetCustomerServiceList(EventHandler<RestClientEventArgs<List<UserInfo>>> callback)
        {
            string relativeUrl = string.Format("/CommonService/User/GetCSList/{0}", CPApplication.Current.CompanyCode);
            GetRestClient(ConstValue.DomainName_Common).Query<List<UserInfo>>(relativeUrl, callback);
        }

        #endregion

    }
}
