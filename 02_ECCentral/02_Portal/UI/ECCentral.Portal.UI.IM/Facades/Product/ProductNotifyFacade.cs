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

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductNotifyFacade
    {
        private readonly RestClient restClient;
        private const string GetProductNotifyUrl = "/IMService/ProductNotify/GetProductNotifyByQuery";

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

        public ProductNotifyFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductNotifyFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        /// <summary>
        /// 根据query得到到货通知信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void GetProductNotifyByQuery(ProductNotifyQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
                
             int priductId;
            ProductNotifyQueryFilter query = new ProductNotifyQueryFilter();
            query.Category1SysNo = model.Category1SysNo;
            query.Category2SysNo = model.Category2SysNo;
            query.Category3SysNo = model.Category3SysNo;
            query.CustomserID = model.CustomserID;
            query.Email = model.Email;
            query.EndTime = model.EndTime;
             query.PMSysNo =model.PMSysNo;
            
            if (int.TryParse(model.ProductSysNo, out priductId))
            {
                query.ProductSysNo = priductId;
            }
            query.StartTime = model.StartTime;
            query.Status = model.Status;
            
            query.PageInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            restClient.QueryDynamicData(GetProductNotifyUrl, query, (obj, args) =>
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
