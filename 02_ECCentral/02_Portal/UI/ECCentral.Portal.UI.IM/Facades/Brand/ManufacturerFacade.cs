//************************************************************************
// 用户名				泰隆优选
// 系统名				生产商管理
// 子系统名		        生产商管理Facades端
// 作成者				Tom
// 改版日				2012.5.14
// 改版内容				新建
//************************************************************************

using System;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ManufacturerFacade
    {
        #region 构造函数和字段
        private readonly RestClient _restClient;
        const string CreateRelativeUrl = "/IMService/Manufacturer/CreateManufacturer";
        const string UPdateRelativeUrl = "/IMService/Manufacturer/UpdateManufacturer";
        const string GetRelativeUrl = "/IMService/Manufacturer/GetManufacturerInfoBySysNo";
        const string DeleteBrandShipCategoryRelativeUrl = "/IMService/Manufacturer/DeleteBrandShipCategory";
        const string CreateBrandShipCategoryRelativeUrl = "/IMService/Manufacturer/CreateBrandShipCategory";
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

        public ManufacturerFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public ManufacturerFacade(IPage page)
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
        private ManufacturerInfo CovertVMtoEntity(ManufacturerVM data)
        {
            ManufacturerInfo msg = data.ConvertVM<ManufacturerVM, ManufacturerInfo>((v, t) =>
                                                                                        {
                                                                                            t.ManufacturerDescription = new LanguageContent(v.ManufacturerDescription);
                                                                                            t.ManufacturerNameLocal = new LanguageContent(v.ManufacturerNameLocal); ;
                                                                                        });
            msg.SysNo = data.SysNo;
            msg.SupportInfo = data.SupportInfo.ConvertVM<ManufacturerSupportVM, ManufacturerSupportInfo>();
            return msg;
        }

        /// <summary>
        /// 创建品牌
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateManufacturer(ManufacturerVM data, EventHandler<RestClientEventArgs<ManufacturerInfo>> callback)
        {
            _restClient.Create(CreateRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 修改生產商
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateManufacturer(ManufacturerVM data, EventHandler<RestClientEventArgs<ManufacturerInfo>> callback)
        {

            _restClient.Update(UPdateRelativeUrl, CovertVMtoEntity(data), callback);
        }


        /// <summary>
        /// 获取品牌
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetManufacturerBySysNo(int sysNo, EventHandler<RestClientEventArgs<ManufacturerInfo>> callback)
        {
            _restClient.Query(GetRelativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 删除旗舰店首页分类
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void DeleteBrandShipCategory(int sysNo, EventHandler<RestClientEventArgs<object>> callback)
        {
            _restClient.Delete(DeleteBrandShipCategoryRelativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 添加旗舰店首页分类
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void CreateBrandShipCategory(BrandShipCategory brandShipCategory, EventHandler<RestClientEventArgs<BrandShipCategory>> callback)
        {
            _restClient.Create(CreateBrandShipCategoryRelativeUrl, brandShipCategory, callback);
        }
        #endregion
    }
}
