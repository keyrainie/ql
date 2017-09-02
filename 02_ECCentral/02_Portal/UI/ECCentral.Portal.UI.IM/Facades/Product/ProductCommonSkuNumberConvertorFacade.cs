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
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.BizEntity.IM;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductCommonSkuNumberConvertorFacade
    {
        private readonly RestClient restClient;
        private const string GetCommonSkuNumbersByProductIDsUrl = "/IMService/ProductCommonSkuNumberConvertor/GetCommonSkuNumbersByProductIDs";
        private const string GetProductIDsByCommonSkuNumbersUrl = "/IMService/ProductCommonSkuNumberConvertor/GetProductIDsByCommonSkuNumbers";

        /// <summary>
        /// IMService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

        public ProductCommonSkuNumberConvertorFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductCommonSkuNumberConvertorFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void GetCommonSkuNumbersByProductIDs(List<string> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.QueryDynamicData(GetCommonSkuNumbersByProductIDsUrl, list, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void GetProductIDsByCommonSkuNumbers(List<string> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {


            restClient.QueryDynamicData(GetProductIDsByCommonSkuNumbersUrl, list, (obj, args) =>
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