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
    public class BrandRequestFacade
    {
        private readonly RestClient restClient;
        private const string GetAllBrandRequestUrl = "/IMService/BrandRequest/GetAllBrandRequest";
        private const string AuditBrandRequestUrl = "/IMService/BrandRequest/AuditBrandRequest";
        private const string InsertBrandRequestUrl = "/IMService/BrandRequest/InsertBrandRequest";
        const string GetRelativeUrl = "/IMService/Brand/GetBrandInfoBySysNo";
        const string GetGetBrandCodeUrl = "/IMService/Brand/GetBrandCode";
        const string UPdateRelativeUrl = "/IMService/Brand/UpdateBrand";
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

        public BrandRequestFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public BrandRequestFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 获取待审核的所有品牌
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void GetAllBrandRequest(int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            BrandRequestQueryFilter query = new BrandRequestQueryFilter()
            {
                PagingInfo = new PagingInfo()
                {
                    PageIndex = PageIndex,
                    PageSize = PageSize,
                    SortBy = SortField
                }
            };
            restClient.QueryDynamicData(GetAllBrandRequestUrl, query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        /// <summary>
        /// 审核品牌信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void AuditManufacturerRequest(BrandRequestVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            BrandRequestInfo info = new BrandRequestInfo()
            {
                BrandDescription = new LanguageContent() { Content = model.Info },
                BrandNameGlobal = model.BrandName_En,
                BrandNameLocal = new LanguageContent() { Content = model.BrandName_Ch },
                Manufacturer = new ManufacturerInfo() { SysNo = Convert.ToInt32(model.ManufacturerSysNo), ManufacturerNameLocal = new LanguageContent() { Content = model.ManufacturerName }, ManufacturerNameGlobal = model.ManufacturerBriefName },
                BrandSupportInfo = new BrandSupportInfo() { ServiceEmail = model.SupportEmail, ServicePhone = model.CustomerServicePhone, ServiceUrl = model.ShowStoreUrl, ManufacturerUrl = model.ManufacturerWebsite },
                Status = model.BrandStatus,
                ReustStatus = model.RequestStatus,
                SysNo = model.SysNo,
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = CPApplication.Current.LanguageCode,
                BrandCode = model.BrandCode,
                User = new BizEntity.Common.UserInfo() { UserName = CPApplication.Current.LoginUser.LoginName, SysNo = CPApplication.Current.LoginUser.UserSysNo }
            };
            restClient.Update(AuditBrandRequestUrl, info, callback);

        }
        /// <summary>
        /// 提交审核 
        /// </summary>
        /// <param name="info"></param>
        public void InsertBrandRequest(BrandRequestVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {

            BrandRequestInfo info = new BrandRequestInfo()
            {
                BrandDescription = new LanguageContent() { Content = model.Reasons },
                BrandNameGlobal = model.BrandName_En,
                BrandNameLocal = new LanguageContent() { Content = model.BrandName_Ch },
                Manufacturer = null,
                BrandSupportInfo = new BrandSupportInfo() { ServiceEmail = model.SupportEmail, ServicePhone = model.CustomerServicePhone, ServiceUrl = model.ShowStoreUrl, ManufacturerUrl = model.ManufacturerWebsite },
                Status = model.BrandStatus,
                ReustStatus = model.RequestStatus,
                SysNo = model.SysNo,
                ProductLine = model.ProductLine,
                Reason = model.Reasons,
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = CPApplication.Current.LanguageCode,
                User = new BizEntity.Common.UserInfo() { UserName = CPApplication.Current.LoginUser.LoginName, SysNo = CPApplication.Current.LoginUser.UserSysNo },
                BrandCode = model.BrandCode

            };
            restClient.Create(InsertBrandRequestUrl, info, callback);
        }
        /// <summary>
        /// 获取品牌
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetBrandBySysNo(int sysNo, EventHandler<RestClientEventArgs<BrandInfo>> callback)
        {
            restClient.Query(GetRelativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 自动生成牌编号
        /// </summary>
        public void GetBrandCode(EventHandler<RestClientEventArgs<string>> callback)
        {
            restClient.Query(GetGetBrandCodeUrl, null, callback);
        }

        /// <summary>
        /// 修改品牌
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateBrand(BrandRequestVM data, EventHandler<RestClientEventArgs<BrandInfo>> callback)
        {
            BrandInfo info = new BrandInfo()
            {
                BrandDescription = new LanguageContent(data.Info),
                BrandNameGlobal = data.BrandName_En,
                BrandNameLocal = new LanguageContent(data.BrandName_Ch),
                BrandStory = data.BrandStory,
                BrandStoreType = data.BrandStoreType,
                Status = data.BrandStatus,
                SysNo = data.SysNo,
                IsLogo = data.IsLogo ? "Y" : "N",
                BrandSupportInfo = new BrandSupportInfo() { ManufacturerUrl = data.ManufacturerWebsite, ServiceEmail = data.SupportEmail, ServicePhone = data.CustomerServicePhone, ServiceUrl = data.SupportUrl },
                Manufacturer = new ManufacturerInfo() { BrandImage = data.AdImage, IsLogo = data.IsLogo, IsShowZone = data.IsDisPlayZone == true ? "Y" : "N", ShowUrl = data.ShowStoreUrl, SysNo = Convert.ToInt32(data.ManufacturerSysNo) },
                User = new BizEntity.Common.UserInfo() { UserName = CPApplication.Current.LoginUser.LoginName, SysNo = CPApplication.Current.LoginUser.UserSysNo },
                LanguageCode = CPApplication.Current.LanguageCode,
                CompanyCode = CPApplication.Current.CompanyCode,
                BrandCode = data.BrandCode

            };
            restClient.Update(UPdateRelativeUrl, info, callback);
        }


    }
}
