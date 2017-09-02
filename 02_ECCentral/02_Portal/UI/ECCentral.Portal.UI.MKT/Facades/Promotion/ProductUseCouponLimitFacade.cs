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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;
using System.Collections.Generic;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class ProductUseCouponLimitFacade
    {
        private const string GetProductUseCouponLimitByQueryUrl = "/MKTService/ProductUseCouponLimit/GetProductUseCouponLimitByQuery";
        private const string CreateProductUseCouponLimitUrl = "/MKTService/ProductUseCouponLimit/CreateProductUseCouponLimit";
        private const string ModifyProductUseCouponLimitUrl = "/MKTService/ProductUseCouponLimit/ModifyProductUseCouponLimit";
      
        private readonly RestClient restClient;

        /// <summary>
        /// IMService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");                
            }
        }

        public ProductUseCouponLimitFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductUseCouponLimitFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        /// <summary>
        /// 根据query获取信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void GetProductUseCouponLimitByQuery(ProductUseCouponLimitQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductUseCouponLimitQueryFilter query;
            query = model.ConvertVM<ProductUseCouponLimitQueryVM, ProductUseCouponLimitQueryFilter>();
            query.PageInfo = new PagingInfo()
            {
                PageIndex = PageIndex,
                PageSize = PageSize,
                SortBy = SortField
            };
            restClient.QueryDynamicData(GetProductUseCouponLimitByQueryUrl, query, callback);
        }
        /// <summary>
        ///新建
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        public void CreateProductUseCouponLimit(List<string> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Create(CreateProductUseCouponLimitUrl, ConvertListEntity(list, CouponLimitType.Manually), callback);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        public void DeleteProductUseCouponLimit(List<ProductUseCouponLimitInfo> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update(ModifyProductUseCouponLimitUrl, list, callback);
        }
        /// <summary>
        /// 转换实体集合
        /// </summary>
        /// <param name="list"></param>
        /// <param name="type">新建的type为手动</param>
        /// <returns></returns>
        private  List<ProductUseCouponLimitInfo> ConvertListEntity(List<string> list,CouponLimitType type)
        {
            List<ProductUseCouponLimitInfo> data = new List<ProductUseCouponLimitInfo>();
            foreach (var item in list)
            {
                data.Add(new ProductUseCouponLimitInfo() 
                { CouponLimitType = type, 
                    ProductId = item,
                  User = new BizEntity.Common.UserInfo() { UserName = CPApplication.Current.LoginUser.LoginName, SysNo = CPApplication.Current.LoginUser.UserSysNo }
                });
            }
            return data;
        }
        
    }
  
}
