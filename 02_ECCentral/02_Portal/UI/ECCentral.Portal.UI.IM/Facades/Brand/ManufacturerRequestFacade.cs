using System;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ManufacturerRequestFacade
    {
        private readonly RestClient restClient;
        private const string GetAllManufacturerRequestUrl = "/IMService/ManufacturerRequest/GetAllManufacturerRequest";
        private const string AuditManufacturerRequestUrl = "/IMService/ManufacturerRequest/AuditManufacturerRequest";
        const string InsertManufacturerRequestUrl = "/IMService/ManufacturerRequest/InsertManufacturerRequest";
        const string GetManufacturerBySysNoUrl = "/IMService/Manufacturer/GetManufacturerInfoBySysNo";
        const string UPManufacturerUrl = "/IMService/Manufacturer/UpdateManufacturer";
        const string UpdateBrandMasterByManufacturerSysNoUrl = "/IMService/Brand/UpdateBrandMasterByManufacturerSysNo";

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

        public ManufacturerRequestFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ManufacturerRequestFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        public void GetAllManufacturerRequest(int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ManufacturerRequestQueryFilter query = new ManufacturerRequestQueryFilter()
            {
                PagingInfo = new PagingInfo() 
                {
                    PageIndex=PageIndex,
                    PageSize=PageSize,
                    SortBy=SortField
                }
            };
            restClient.QueryDynamicData(GetAllManufacturerRequestUrl, query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
        public void AuditManufacturerRequest(ManufacturerRequestVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ManufacturerRequestInfo info = new ManufacturerRequestInfo() 
            {
                ManufacturerName=model.ManufacturerName,
                ManufacturerBriefName=model.ManufacturerBriefName,
                Status=model.Status,
                SysNo=model.SysNo,
                ManufacturerStatus=model.ManufacturerStatus,
                ManufacturerSysNo=model.ManufacturerSysNo,
                OperationType=model.OperationType,
                ProductLine=model.ProductLine,
                Reasons=model.Reasons,
                LanguageCode=CPApplication.Current.LanguageCode,
                CompanyCode=CPApplication.Current.CompanyCode
            };
            restClient.Update(AuditManufacturerRequestUrl, info, callback);
        }

        /// <summary>
        /// 生产商提交审核
        /// </summary>
        /// <param name="info"></param>
        public void InsertManufacturerRequest(ManufacturerRequestVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ManufacturerRequestInfo info = model.ConvertVM<ManufacturerRequestVM, ManufacturerRequestInfo>();
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.LanguageCode = CPApplication.Current.LanguageCode;
            restClient.Create(InsertManufacturerRequestUrl, info, callback);
        }
        /// <summary>
        /// 获取生产商
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetManufacturerBySysNo(int sysNo, EventHandler<RestClientEventArgs<ManufacturerInfo>> callback)
        {
            restClient.Query(GetManufacturerBySysNoUrl, sysNo, callback);
        }
        /// <summary>
        /// 修改生产商
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateManufacturer(ManufacturerRequestVM data, EventHandler<RestClientEventArgs<ManufacturerInfo>> callback)
        {

            ManufacturerInfo info = new ManufacturerInfo()
            {
                ManufacturerDescription = new LanguageContent(data.Info),
                ManufacturerNameGlobal = data.ManufacturerBriefName,
                ManufacturerNameLocal = new LanguageContent(data.ManufacturerName),
                IsShowZone = data.IsShowZone == true ? "Y" : "N",
                IsLogo = data.IsLogo,
                BrandImage = data.BrandImage,
                BrandStoreType = data.BrandStoreType,
                ShowUrl = data.ShowUrl,
                Status = data.ManufacturerStatus,
                SysNo = data.SysNo,
                SupportInfo = new ManufacturerSupportInfo() {ManufacturerUrl=data.MannfacturerLink,ServiceEmail=data.AfterSalesSupportEmail,ServicePhone=data.ClientPhone,ServiceUrl=data.AfterSalesSupportLink },
               };
            restClient.Update(UPManufacturerUrl, info, callback);
        }

        /// <summary>
        /// 修改品牌
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateBrandMasterByManufacturerSysNo(ManufacturerRequestVM data, EventHandler<RestClientEventArgs<ManufacturerInfo>> callback)
        {
            BrandInfo info = new BrandInfo()
            {
                Status = data.ManufacturerStatus == ManufacturerStatus.Active ? ValidStatus.Active : ValidStatus.DeActive,
                Manufacturer = new ManufacturerInfo() { SysNo = data.SysNo, IsShowZone = data.IsShowZone ? "Y" : "N", BrandImage = data.BrandImage, ShowUrl = data.ShowUrl },
                IsLogo = data.IsLogo ? "Y" : "N",
                BrandSupportInfo = new BrandSupportInfo() { ManufacturerUrl = data.MannfacturerLink, ServiceEmail = data.AfterSalesSupportEmail, ServicePhone = data.ClientPhone, ServiceUrl = data.AfterSalesSupportLink },
                BrandStoreType = data.BrandStoreType,
                User=new BizEntity.Common.UserInfo(){UserName=CPApplication.Current.LoginUser.LoginName,SysNo=CPApplication.Current.LoginUser.UserSysNo}
            };
            restClient.Update(UpdateBrandMasterByManufacturerSysNoUrl, info, callback);
        }
    }
}
