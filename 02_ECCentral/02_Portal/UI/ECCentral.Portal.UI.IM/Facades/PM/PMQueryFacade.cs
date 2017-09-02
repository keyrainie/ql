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
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.BizEntity.IM;


namespace ECCentral.Portal.UI.IM.Facades
{
    public class PMQueryFacade
    {
        private readonly RestClient restClient;

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

        public PMQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public PMQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryPM(PMQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductManagerQueryFilter filter;
            filter = model.ConvertVM<PMQueryVM, ProductManagerQueryFilter>();

            filter.UserID = model.PMID;
            filter.UserName = model.PMName;
            filter.PMGroupName = model.PMGroupName;
            PMStatus statusValue;
            Enum.TryParse(model.Status, out statusValue);
            filter.Status = statusValue;

            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            string relativeUrl = "/IMService/ProductManager/QueryProductManagerInfo";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    callback(obj, args);
                }
                );
        }

        /// <summary>
        /// 查询PM List
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryPMList(ProductManagerQueryFilter queryFilter, EventHandler<RestClientEventArgs<List<ProductManagerInfo>>> callback)
        {
            string relativeUrl = "/IMService/PM/QueryPMList";
            restClient.Query<List<ProductManagerInfo>>(relativeUrl, queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }       

        public void QueryPMLeaderList(EventHandler<RestClientEventArgs<List<ProductManagerInfo>>> callback)
        {
            string relativeUrl = string.Format("/IMService/PM/QueryPMLeaderList/{0}", CPApplication.Current.CompanyCode);
            restClient.Query<List<ProductManagerInfo>>(relativeUrl, (obj, args) =>
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
