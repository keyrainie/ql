using System;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class CategoryListFacade
    {
         private readonly RestClient restClient;
         private const string GetCategory1ListUrl = "/IMService/BrandRecommended/GetCategory1List";
         private const string GetCategory2ListUrl = "/IMService/BrandRecommended/GetCategory2List";
        
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

        public CategoryListFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public CategoryListFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);

        }
        public void GetCategory1List(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.QueryDynamicData(GetCategory1ListUrl, null, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                callback(obj, arg);
            });
            
        }


        public void GetCategory2List(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.QueryDynamicData(GetCategory2ListUrl, null, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                callback(obj, arg);
            });
        }
    }
}
