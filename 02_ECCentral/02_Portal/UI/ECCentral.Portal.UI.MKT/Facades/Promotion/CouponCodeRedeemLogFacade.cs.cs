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
using ECCentral.Portal.UI.MKT.Models.Promotion.QueryFilter;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.MKT.Promotion;

namespace ECCentral.Portal.UI.MKT.Facades.Promotion
{
    public class CouponCodeRedeemLogFacade
    {
        private readonly RestClient restClient;
        public CouponCodeRedeemLogQueryFilterVM _ViewModel;
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

        public CouponCodeRedeemLogFacade(IPage page, CouponCodeRedeemLogQueryFilterVM vm)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
            _ViewModel = vm;
        }

        public CouponCodeRedeemLogFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(CouponCodeRedeemLogQueryFilterVM filterVM, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            if (!filterVM.HasValidationErrors)
            {
                string relativeUrl = "/MKTService/Coupons/QueryCustomerRedeemLog";
                CouponCodeRedeemLogFilter filter = filterVM.ConvertVM<CouponCodeRedeemLogQueryFilterVM, CouponCodeRedeemLogFilter>();
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
    }
}
