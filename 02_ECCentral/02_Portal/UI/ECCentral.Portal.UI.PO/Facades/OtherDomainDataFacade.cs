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
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.PO.Facades
{
    public class OtherDomainDataFacade
    {
        private readonly RestClient restClient_IM;

        protected string ServiceBaseUrl_IM
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

        public OtherDomainDataFacade()
        {
            restClient_IM = new RestClient(ServiceBaseUrl_IM);
        }

        public OtherDomainDataFacade(IPage page)
        {
            restClient_IM = new RestClient(ServiceBaseUrl_IM, page);
        }

        public void QueryAllCategory(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CategoryQueryFilter filter = new CategoryQueryFilter() { Status = CategoryStatus.Active, Type = CategoryType.CategoryType3 };

            filter.PagingInfo = new PagingInfo
            {
                PageSize = 100000,
                PageIndex = 0
            };

            const string relativeUrl = "/IMService/Category/GetCategoryListByType";
            restClient_IM.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    callback(obj, args);
                });
        }




        public void QueryAllBrand(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            BrandQueryFilter filter = new BrandQueryFilter();
            filter.Status = ValidStatus.Active;
            filter.PagingInfo = new PagingInfo
            {
                PageSize = 100000,
                PageIndex = 0
            };

            string relativeUrl = "/IMService/Brand/QueryBrand";
            restClient_IM.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    if (!(args == null || args.Result == null || args.Result.Rows == null))
                    {
                        foreach (var item in args.Result.Rows)
                        {
                            item.IsChecked = false;
                        }
                    }
                    callback(obj, args);
                }
                );
        }
    }
}
