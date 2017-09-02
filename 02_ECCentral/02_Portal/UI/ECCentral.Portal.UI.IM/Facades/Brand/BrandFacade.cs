//************************************************************************
// 用户名				泰隆优选
// 系统名				品牌管理
// 子系统名		        品牌管理Facades端
// 作成者				Tom
// 改版日				2012.5.14
// 改版内容				新建
//************************************************************************

using System;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using System.Collections.Generic;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class BrandFacade
    {
        #region 构造函数和字段
        private readonly RestClient _restClient;
        const string CreateRelativeUrl = "/IMService/Brand/CreateBrand";
        const string UPdateRelativeUrl = "/IMService/Brand/UpdateBrand";
        const string GetRelativeUrl = "/IMService/Brand/GetBrandInfoBySysNo";
        const string GetBrandAuthorizedByBrandSysNoUrl = "/IMService/Brand/GetBrandAuthorizedByBrandSysNo";
        const string DeleteBrandAuthorizedUrl = "/IMService/Brand/DeleteBrandAuthorized";
        const string UpdateBrandAuthorizedUrl = "/IMService/Brand/UpdateBrandAuthorized";
        const string InsertBrandAuthorizedUrl = "/IMService/Brand/InsertBrandAuthorized";
        const string IsExistBrandAuthorizedUrl = "/IMService/Brand/IsExistBrandAuthorized";
        const string UrlIsExistUrl = "/IMService/Brand/UrlIsExist";
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

        public BrandFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public BrandFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        #region 函数
        /// <summary>
        /// 转换品牌视图和品牌实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private BrandInfo CovertVMtoEntity(BrandVM data)
        {
            BrandInfo msg = data.ConvertVM<BrandVM, BrandInfo>((v, t) =>
                                                                   {
                                                                       t.BrandNameLocal = new LanguageContent(v.BrandNameLocal);
                                                                       t.BrandDescription = new LanguageContent(v.BrandDescription);
                                                                   });
            msg.Manufacturer = data.ManufacturerInfo.ConvertVM<ManufacturerVM, ManufacturerInfo>();
            msg.SysNo = data.SysNo;
            msg.BrandSupportInfo = data.BrandSupportInfo.ConvertVM<BrandSupportVM, BrandSupportInfo>();
            return msg;
        }

        /// <summary>
        /// 创建品牌
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateBrand(BrandVM data, EventHandler<RestClientEventArgs<BrandInfo>> callback)
        {
            _restClient.Create(CreateRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 修改品牌
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateBrand(BrandVM data, EventHandler<RestClientEventArgs<BrandInfo>> callback)
        {

            _restClient.Update(UPdateRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 获取品牌
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetBrandBySysNo(int sysNo, EventHandler<RestClientEventArgs<BrandInfo>> callback)
        {
            _restClient.Query(GetRelativeUrl, sysNo, callback);
        }

        #endregion
        /// <summary>
        /// 更据BrandSysNo得到该品牌的所有授权信息
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="callback"></param>
        public void GetBrandAuthorizedByBrandSysNo(int SysNo, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            BrandAuthorizedFilter query = new BrandAuthorizedFilter()
            {
                BrandSysNo = SysNo,
                PageInfo = new QueryFilter.Common.PagingInfo()
                {
                    PageIndex = PageIndex,
                    PageSize = PageSize,
                    SortBy = SortField
                }
            };
            _restClient.QueryDynamicData(GetBrandAuthorizedByBrandSysNoUrl, query, callback);
        }

        #region "授权牌操作"





        /// <summary>
        /// 批量删除授权牌
        /// </summary>
        /// <param name="SysNo"></param>

        public void DeleteBrandAuthorized(List<int> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            _restClient.Delete(DeleteBrandAuthorizedUrl, list, callback);

        }

        /// <summary>
        /// 更新授权牌的状态
        /// </summary>
        /// <param name="info"></param>
        public void UpdateBrandAuthorized(List<BrandAuthorizedVM> listInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            List<BrandAuthorizedInfo> list = new List<BrandAuthorizedInfo>();
            listInfo.ForEach(info =>
            {
                list.Add(new BrandAuthorizedInfo()
                {
                    AuthorizedStatus = info.AuthorizedAcive ? AuthorizedStatus.Active : AuthorizedStatus.DeActive,
                    SysNo = info.SysNo,
                    User = new BizEntity.Common.UserInfo() { UserName = CPApplication.Current.LoginUser.LoginName, SysNo = CPApplication.Current.LoginUser.UserSysNo }
                });
            });

            _restClient.Update(UpdateBrandAuthorizedUrl, list, callback);
        }

        /// <summary>
        /// 插入授权信息
        /// </summary>
        /// <param name="info"></param>
        public void InsertBrandAuthorized(BrandAuthorizedVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            BrandAuthorizedInfo info = new BrandAuthorizedInfo()
            {
                AuthorizedStatus = model.AuthorizedAcive == true ? AuthorizedStatus.Active : AuthorizedStatus.DeActive,
                BrandSysNo = model.BrandSysNo,
                ReferenceSysNo = model.ReferenceSysNo,
                ImageName = model.ImageName,
                EndActiveTime = model.EndActiveTime,
                StartActiveTime = model.StartActiveTime,
                SysNo = model.SysNo,
                Type = model.Type,
                IsExist = model.IsExist,
                LanguageCode = CPApplication.Current.LanguageCode,
                CompanyCode = CPApplication.Current.CompanyCode,
                User = new BizEntity.Common.UserInfo() { UserName = CPApplication.Current.LoginUser.LoginName, SysNo = CPApplication.Current.LoginUser.UserSysNo }
            };
            _restClient.Create(InsertBrandAuthorizedUrl, info, callback);
        }

        /// <summary>
        /// 是否存在该授权信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public void IsExistBrandAuthorized(BrandAuthorizedVM model, EventHandler<RestClientEventArgs<bool>> callback)
        {
            BrandAuthorizedInfo info = new BrandAuthorizedInfo()
            {
                AuthorizedStatus = model.AuthorizedAcive == true ? AuthorizedStatus.Active : AuthorizedStatus.DeActive,
                BrandSysNo = model.BrandSysNo,
                ReferenceSysNo = model.ReferenceSysNo,
                ImageName = model.ImageName,
                EndActiveTime = model.EndActiveTime,
                StartActiveTime = model.StartActiveTime,
                SysNo = model.SysNo,
                Type = model.Type,

            };
            _restClient.Query(IsExistBrandAuthorizedUrl, info, callback);
        }
        /// <summary>
        /// 检查图片地址是否正确
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public void UrlIsExist(string url, EventHandler<RestClientEventArgs<bool>> callback)
        {
            _restClient.Query(UrlIsExistUrl, url, callback);
        }
        #endregion
    }
}
