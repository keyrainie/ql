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
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.MKT.Models.Promotion;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Models.Promotion.QueryFilter;
using ECCentral.QueryFilter.MKT;
using ECCentral.QueryFilter.MKT.Promotion;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.Facades.Promotion
{
    public class CouponCodeCustomerLogFacade
    {
        private readonly RestClient restClient;
        public CouponCodeCustomerLogQueryFilterVM _ViewModel;
        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            }
        }

        public CouponCodeCustomerLogFacade(IPage page, CouponCodeCustomerLogQueryFilterVM vm)
        {
            restClient = new RestClient(ServiceBaseUrl,page);
            _ViewModel = vm;
        }

         public CouponCodeCustomerLogFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

         public void Query(CouponCodeCustomerLogQueryFilterVM filterVM, EventHandler<RestClientEventArgs<dynamic>> callback)
         {
             if (!filterVM.HasValidationErrors)
             {
                 string relativeUrl = "/MKTService/Coupons/QueryCustomerGetLog";
                 CouponCodeCustomerLogFilter filter = filterVM.ConvertVM<CouponCodeCustomerLogQueryFilterVM, CouponCodeCustomerLogFilter>();

                 restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
                 {
                     if (args.FaultsHandle())
                     {
                         return;
                     }
                     callback(obj, args);
                 });
             }
         }


         public void ExportExcelFile(CouponCodeCustomerLogQueryFilterVM queryVM, ColumnSet[] columns)
         {
             CouponCodeCustomerLogFilter queryFilter = new CouponCodeCustomerLogFilter();
             queryFilter = queryVM.ConvertVM<CouponCodeCustomerLogQueryFilterVM, CouponCodeCustomerLogFilter>();
             queryFilter.PageInfo = new PagingInfo
             {
                 PageSize = ConstValue.MaxRowCountLimit,
                 PageIndex = 0,
                 SortBy = null
             };
             string relativeUrl = "/MKTService/Coupons/QueryCustomerGetLog";
             restClient.ExportFile(relativeUrl, queryFilter, columns);
         }
    }
}
