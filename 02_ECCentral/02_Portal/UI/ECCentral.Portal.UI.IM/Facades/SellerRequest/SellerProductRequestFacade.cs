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
    public class SellerProductRequestFacade
    {
        #region 构造函数和字段
        private readonly RestClient _restClient;

        const string CreateItemIDRelativeUrl = "/IMService/SellerProductRequest/CreateItemIDForNewProductRequest";

        const string ApproveRelativeUrl = "/IMService/SellerProductRequest/ApproveProductRequest";

        const string DenyRelativeUrl = "/IMService/SellerProductRequest/DenyProductRequest";

        const string UpdateRelativeUrl = "/IMService/SellerProductRequest/UpdateProductRequest";

        const string GetRelativeUrl = "/IMService/SellerProductRequest/GetSellerProductRequestInfoBySysNo";

        const string GetRelativeUrlProductID = "/IMService/SellerProductRequest/GetSellerProductInfoByProductID";

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

        public SellerProductRequestFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public SellerProductRequestFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        #region 函数
        /// <summary>
        /// 转换分类视图和分类实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private SellerProductRequestInfo CovertVMtoEntity(SellerProductRequestVM data)
        {
            SellerProductRequestInfo msg = data.ConvertVM<SellerProductRequestVM, SellerProductRequestInfo>();

            msg.CategoryInfo = data.CategoryInfo.ConvertVM<CategoryVM, CategoryInfo>((v, t) =>
            {
                t.CategoryName = new LanguageContent(v.CategoryName);
            });

            msg.Brand = data.Brand.ConvertVM<BrandVM, BrandInfo>((v, t) =>
            {
                t.BrandNameLocal = new LanguageContent(v.BrandNameLocal);
            });

            msg.Manufacturer = data.Manufacturer.ConvertVM<ManufacturerVM, ManufacturerInfo>((v, t) =>
            {
                t.ManufacturerNameLocal = new LanguageContent(v.ManufacturerNameLocal);
            });

            msg.InUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.userSysNo,UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            msg.EditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.userSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            msg.Auditor = new UserInfo { SysNo = CPApplication.Current.LoginUser.userSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            msg.CompanyCode = CPApplication.Current.CompanyCode;
            msg.LanguageCode = data.LanguageCode;
            msg.SysNo = data.SysNo;
            return msg;
        }



        /// <summary>
        /// 创建ID
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateItemIDForNewProductRequest(SellerProductRequestVM data, EventHandler<RestClientEventArgs<SellerProductRequestInfo>> callback)
        {
            _restClient.Update(CreateItemIDRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 更新商品请求
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateProductRequest(SellerProductRequestVM data, EventHandler<RestClientEventArgs<SellerProductRequestInfo>> callback)
        {

            _restClient.Update(UpdateRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 审核通过商品请求
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void ApproveProductRequest(SellerProductRequestVM data, EventHandler<RestClientEventArgs<SellerProductRequestInfo>> callback)
        {

            _restClient.Update(ApproveRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 退回商品请求
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void DenyProductRequest(SellerProductRequestVM data, EventHandler<RestClientEventArgs<SellerProductRequestInfo>> callback)
        {

            _restClient.Update(DenyRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 获取商家商品信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetSellerProductRequestBySysNo(int sysNo, EventHandler<RestClientEventArgs<SellerProductRequestInfo>> callback)
        {
            _restClient.Query(GetRelativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 获取商家商品信息
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="callback"></param>
        public void GetSellerProductRequestByProductID(string productID, EventHandler<RestClientEventArgs<SellerProductRequestInfo>> callback)
        {
            _restClient.Query(GetRelativeUrlProductID, productID, callback);
        }

        #endregion
    }
}
