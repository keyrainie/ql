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
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Portal.UI.ExternalSYS.Facades
{
    public class CpsUserFacade
    {
         private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.Key_ServiceBaseUrl);

        public CpsUserFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public CpsUserFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void GetCpsUser(CpsUserQueryVM mode, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string GetCpsUserUrl = "ExternalSYSService/CpsUser/GetCpsUser";
            CpsUserQueryFilter query = ConvertQuery(mode);
            query.PageInfo = new QueryFilter.Common.PagingInfo() 
            {
                PageIndex=PageIndex,
                PageSize=PageSize,
                SortBy=SortField
            };
            restClient.QueryDynamicData(GetCpsUserUrl, query, callback);
        }

        /// <summary>
        /// 获取网站类型
        /// </summary>
        /// <param name="callback"></param>
        public void GetWebSiteType(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string GetWebSiteTypeUrl = "ExternalSYSService/CpsUser/GetWebSiteType";
            restClient.QueryDynamicData(GetWebSiteTypeUrl, null, callback);
        }
        /// <summary>
        /// 获取银行类型
        /// </summary>
        /// <param name="callback"></param>
        public void GetBankType(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string GetBankTypeUrl = "ExternalSYSService/CpsUser/GetBankType";
            restClient.QueryDynamicData(GetBankTypeUrl, null, callback);
        }

        public void UpdateUserStatus(CpsUserVM vm, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string UpdateUserStatusUrl = "ExternalSYSService/CpsUser/UpdateUserStatus";
            restClient.Update(UpdateUserStatusUrl, ConvertInfo(vm), callback);
        }

        public void UpdateBasicUser(CpsUserVM vm, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string UpdateBasicUserUrl = "ExternalSYSService/CpsUser/UpdateBasicUser";
            restClient.Update(UpdateBasicUserUrl, ConvertInfo(vm), callback);
        }

        public void UpdateCpsReceivablesAccount(CpsUserVM vm, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string UpdateCpsReceivablesAccountUrl = "ExternalSYSService/CpsUser/UpdateCpsReceivablesAccount";
            restClient.Update(UpdateCpsReceivablesAccountUrl, ConvertInfo(vm), callback);
        }
        public void CreateUserSource(CpsUserVM vm, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string CreateUserSourceUrl = "ExternalSYSService/CpsUser/CreateUserSource";
            restClient.Create(CreateUserSourceUrl, ConvertInfo(vm), callback);
        }
        public void GetUserSource(int userSysNo,int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string GetUserSourceUrl = "ExternalSYSService/CpsUser/GetUserSource";
            CpsUserSourceQueryFilter query=new CpsUserSourceQueryFilter()
            {
                UserSysNo=userSysNo,
                PageInfo=new QueryFilter.Common.PagingInfo()
                {
                    PageIndex=PageIndex,
                    PageSize=PageSize,
                    SortBy=SortField
                }
            };
            restClient.QueryDynamicData(GetUserSourceUrl, query, callback);
        }
        public void UpdateUserSource(CpsUserVM vm, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string UpdateUserSourceUrl = "ExternalSYSService/CpsUser/UpdateUserSource";
            restClient.Update(UpdateUserSourceUrl, ConvertInfo(vm), callback);
        }

        /// <summary>
        /// 获取审核记录
        /// </summary>
        /// <param name="callback"></param>
        public void GetAuditHistory(int SysNo,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string GetAuditHistoryUrl = "ExternalSYSService/CpsUser/GetAuditHistory";
            restClient.QueryDynamicData(GetAuditHistoryUrl, SysNo, callback);
        }
        public void AuditUser(CpsUserVM vm, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string AuditUserUrl = "ExternalSYSService/CpsUser/AuditUser";
            restClient.Update(AuditUserUrl, ConvertInfo(vm), callback);
        }

        /// <summary>
        /// model转换
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private CpsUserInfo ConvertInfo(CpsUserVM data)
        {
            CpsUserInfo info = new CpsUserInfo();
            if (data != null)
            {
                if (data.BasicUser != null)
                {
                    info.UserBasicInfo = new CpsBasicUserInfo()
                    {
                        AllianceAccount = data.BasicUser.AllianceAccount,
                        Contact = data.BasicUser.Contact,
                        ContactAddress = data.BasicUser.ContactAddress,
                        ContactPhone = data.BasicUser.ContactPhone,
                        Email = data.BasicUser.Email,
                        IsActive = data.BasicUser.IsActive,
                        UserType = data.BasicUser.UserType,
                        WebSiteAddress = data.BasicUser.WebSiteAddress,
                        WebSiteCode = data.BasicUser.WebSiteCode,
                        WebSiteName = data.BasicUser.WebSiteName,
                        Zipcode = data.BasicUser.ZipCode
                    };
                }
                if (data.UserSource != null)
                {
                    info.Source = new CpsUserSource() 
                    {
                        ChanlName = data.UserSource.ChanlName,
                        Source = data.UserSource.Source,
                        SysNo = data.UserSource.SysNo,
                        UserType = data.UserSource.UserType
                    };
                }
                if (data.ReceivablesAccount != null)
                {
                    info.ReceivablesAccount = new CpsReceivablesAccount() 
                    {
                        BranchBank = data.ReceivablesAccount.BranchBank,
                        BrankCardNumber = data.ReceivablesAccount.BrankCardNumber,
                        BrankCode = data.ReceivablesAccount.BrankCode,
                        BrankName = data.ReceivablesAccount.BrankName,
                        IsLock =data.ReceivablesAccount.IsLock,
                        ReceivablesAccountType = data.ReceivablesAccount.ReceivablesAccountType,
                        ReceiveablesName = data.ReceivablesAccount.ReceiveablesName
                    };
                }
                 info.CompanyCode=CPApplication.Current.CompanyCode;
                 info.LanguageCode=CPApplication.Current.LanguageCode;
                 info.User = new BizEntity.Common.UserInfo()
                 {
                     SysNo = CPApplication.Current.LoginUser.UserSysNo,
                     UserName = CPApplication.Current.LoginUser.LoginName
                 };
                 info.SysNo = data.SysNo;
                 info.AuditNoClearanceInfo = data.AuditNoClearanceInfo;
                 info.Status = data.Status;
            }
            return info;


         
        }

        /// <summary>
        /// query model转换
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private CpsUserQueryFilter ConvertQuery(CpsUserQueryVM data)
        {
            return new CpsUserQueryFilter()
            {
                AuditStatus = data.AuditStatus,
                CustomerID = data.CustomerID,
                Email = data.Email,
                IsActive = data.IsActive,
                ImMessenger = data.ImMessenger,
                Phone = data.Phone,
                ReceivablesName = data.ReceivablesName,
                RegisterDateFrom = data.RegisterDateFrom,
                RegisterDateTo = data.RegisterDateTo,
                UserType = data.UserType,
                WebSiteType = data.WebSiteType.SelectValue,
            };
        }
    }
}
