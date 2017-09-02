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
using ECCentral.BizEntity.Common;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductBrandWarrantyFacade
    {
        private readonly RestClient restClient;
        private const string GetProductBrandWarrantyUrl = "/IMService/ProductBrandWarranty/GetProductBrandWarrantyByQuery";
        private const string BrandWarrantyInfoByAddOrUpdateUrl = "/IMService/ProductBrandWarranty/BrandWarrantyInfoByAddOrUpdate";
        private const string DelBrandWarrantyInfourl = "/IMService/ProductBrandWarranty/DelBrandWarrantyInfoBySysNos";
        private const string UpdateBrandWarrantyInfoBySysNourl = "/IMService/ProductBrandWarranty/UpdateBrandWarrantyInfoBySysNo";

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

        public ProductBrandWarrantyFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductBrandWarrantyFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        //获取查询信息
        public void GetProductBrandWarrantyByQuery(ProductBrandWarrantyQueryFilter query, int PageSize, int PageIndex
            , string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            query.PageInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            restClient.QueryDynamicData(GetProductBrandWarrantyUrl, query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }


        //更新或者添加品牌信息
        public void BrandWarrantyInfoByAddOrUpdate(ProductBrandWarrantyQueryVM ProductBrandWarranty
            ,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductBrandWarrantyInfo data = new ProductBrandWarrantyInfo();
            data.SysNo = ProductBrandWarranty.SysNo;
            data.BrandSysNo = ProductBrandWarranty.BrandSysNo;
            data.C1SysNo = ProductBrandWarranty.C1SysNo;
            data.C2SysNo = ProductBrandWarranty.C2SysNo;
            data.C3SysNo = ProductBrandWarranty.C3SysNo;
            data.EditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.UserSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            data.CreateUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.UserSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            data.WarrantyDay = int.Parse(ProductBrandWarranty.WarrantyDay);
            data.WarrantyDesc = ProductBrandWarranty.WarrantyDesc;
            restClient.Update(BrandWarrantyInfoByAddOrUpdateUrl, data, callback);
        }

        //更新或者添加品牌信息
        public void UpdateBrandWarrantyInfoBySysNo(List<Int32> SysNos 
            , ProductBrandWarrantyQueryVM ProductBrandWarranty
            , EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductBrandWarrantyInfo data = new ProductBrandWarrantyInfo();
            data.SysNos = SysNos;
            data.EditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.UserSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            data.CreateUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.UserSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            data.WarrantyDay = int.Parse(ProductBrandWarranty.WarrantyDay);
            data.WarrantyDesc = ProductBrandWarranty.WarrantyDesc;
            restClient.Update(UpdateBrandWarrantyInfoBySysNourl, data, callback);
        }
        //删除品牌信息
        public void DelBrandWarrantyInfoBySysNo(List<ProductBrandWarrantyQueryVM> delProductBrandWarrantyQuery
            ,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            List<ProductBrandWarrantyInfo> ProductBrandWarrantys = new List<ProductBrandWarrantyInfo>();
            delProductBrandWarrantyQuery.ForEach(p => {
                ProductBrandWarrantys.Add(new ProductBrandWarrantyInfo() { 
                     SysNo = p.SysNo,
                     C3SysNo = p.C3SysNo,
                     BrandSysNo = p.BrandSysNo,
                     EditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.UserSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName }
                });
            });
            restClient.Delete(DelBrandWarrantyInfourl, ProductBrandWarrantys, callback);
        }

    }
}
