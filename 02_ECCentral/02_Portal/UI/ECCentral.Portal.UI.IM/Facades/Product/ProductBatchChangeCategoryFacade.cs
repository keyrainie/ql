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
using ECCentral.BizEntity.Common;


namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductBatchChangeCategoryFacade
    {
        private readonly RestClient _restClient;

        private const string RelativeUrl = "/IMService/ProductBatchChangeCategory/BatchChangeCategory";
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

        public ProductBatchChangeCategoryFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductBatchChangeCategoryFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 批量更改商品三级类别
        /// </summary>
        /// <param name="productList"></param>
        /// <param name="c3SysNo"></param>
        /// <param name="callback"></param>
        public void BatchChangeCategory(List<string> productList, int c3SysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductBatchChangeCategoryInfo data = new ProductBatchChangeCategoryInfo();
          
            data.ProductIDs = productList;
            data.CategoryInfo = new CategoryInfo { SysNo = c3SysNo };
            data.EditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.UserSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            
            _restClient.Update(RelativeUrl, data, callback);
        }
    }
}
