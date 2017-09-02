using System;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class CategoryExtendWarrantyFacade
    {
        #region 构造函数和字段
        private readonly RestClient _restClient;

        const string CreateRelativeUrl = "/IMService/CategoryExtendWarranty/CreateCategoryExtendWarranty";

        const string UPdateRelativeUrl = "/IMService/CategoryExtendWarranty/UpdateCategoryExtendWarranty";

        const string GetRelativeUrl = "/IMService/CategoryExtendWarranty/GetCategoryExtendWarrantyBySysNo";

        const string CreateDisuseBrandRelativeUrl = "/IMService/CategoryExtendWarranty/CreateCategoryExtendWarrantyDisuseBrand";

        const string UPdateDisuseBrandRelativeUrl = "/IMService/CategoryExtendWarranty/UpdateCategoryExtendWarrantyDisuseBrand";

        const string GetDisuseBrandRelativeUrl = "/IMService/CategoryExtendWarranty/GetCategoryExtendWarrantyDisuseBrandBySysNo";

        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

        public CategoryExtendWarrantyFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public CategoryExtendWarrantyFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        #region 分类延保函数
        /// <summary>
        /// 转换分类视图和分类实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private CategoryExtendWarranty CovertVMtoEntity(CategoryExtendWarrantyVM data)
        {
            CategoryExtendWarranty msg = data.ConvertVM<CategoryExtendWarrantyVM, CategoryExtendWarranty>();

            msg.CategoryInfo = data.CategoryInfo.ConvertVM<CategoryVM, CategoryInfo>((v, t) =>
            {
                t.CategoryName = new LanguageContent(v.CategoryName);
            });

            msg.Brand = data.Brand.ConvertVM<BrandVM, BrandInfo>((v, t) =>
            {
                t.BrandNameLocal = new LanguageContent(v.BrandNameLocal);
            });

            msg.CompanyCode = CPApplication.Current.CompanyCode;
            msg.InUser = new UserInfo { UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            msg.EditUser = new UserInfo { UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };

            msg.SysNo = data.SysNo;
            return msg;
        }

        /// <summary>
        /// 创建配件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateCategoryExtendWarranty(CategoryExtendWarrantyVM data, EventHandler<RestClientEventArgs<CategoryExtendWarranty>> callback)
        {
            _restClient.Create(CreateRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 修改分类延保
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateCategoryExtendWarranty(CategoryExtendWarrantyVM data, EventHandler<RestClientEventArgs<CategoryExtendWarranty>> callback)
        {

            _restClient.Update(UPdateRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 获取分类延保
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetCategoryExtendWarrantyBySysNo(int sysNo, EventHandler<RestClientEventArgs<CategoryExtendWarranty>> callback)
        {
            _restClient.Query(GetRelativeUrl, sysNo, callback);
        }
        #endregion

        #region 分类延保排出品牌函数
        /// <summary>
        /// 转换分类视图和分类实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private CategoryExtendWarrantyDisuseBrand CovertVMtoEntity(CategoryExtendWarrantyDisuseBrandVM data)
        {
            CategoryExtendWarrantyDisuseBrand msg = data.ConvertVM<CategoryExtendWarrantyDisuseBrandVM, CategoryExtendWarrantyDisuseBrand>();

            msg.CategoryInfo = data.CategoryInfo.ConvertVM<CategoryVM, CategoryInfo>((v, t) =>
            {
                t.CategoryName = new LanguageContent(v.CategoryName);
            });

            msg.Brand = data.Brand.ConvertVM<BrandVM, BrandInfo>((v, t) =>
            {
                t.BrandNameLocal = new LanguageContent(v.BrandNameLocal);
            });

            msg.CompanyCode = CPApplication.Current.CompanyCode;
            msg.InUser = new UserInfo { UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            msg.EditUser = new UserInfo { UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            msg.LanguageCode = CPApplication.Current.LanguageCode;
            msg.SysNo = data.SysNo;
            return msg;
        }

        /// <summary>
        /// 创建分类延保排出品牌
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrandVM data, EventHandler<RestClientEventArgs<CategoryExtendWarrantyDisuseBrand>> callback)
        {
            _restClient.Create(CreateDisuseBrandRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 修改分类延保排出品牌
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrandVM data, EventHandler<RestClientEventArgs<CategoryExtendWarrantyDisuseBrand>> callback)
        {

            _restClient.Update(UPdateDisuseBrandRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 获取分类延保排出品牌
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetCategoryExtendWarrantyDisuseBrandBySysNo(int sysNo, EventHandler<RestClientEventArgs<CategoryExtendWarrantyDisuseBrand>> callback)
        {
            _restClient.Query(GetDisuseBrandRelativeUrl, sysNo, callback);
        }
        #endregion
    }
}
