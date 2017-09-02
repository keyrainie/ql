using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class RmaPolicyFacade
    {
         private readonly RestClient restClient;

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

        public RmaPolicyFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public RmaPolicyFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryRmaPolicy(RmaPolicyQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            RmaPolicyQueryFilter filter;
            filter = model.ConvertVM<RmaPolicyQueryVM, RmaPolicyQueryFilter>((t, s) => 
            {
                s.Type = t.RmaType;
            });

            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/IMService/RmaPolicy/QueryRmaPolicy";
            restClient.QueryDynamicData(relativeUrl, filter,callback);
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void CreateRmaPolicy(RmaPolicyVM vm, EventHandler<RestClientEventArgs<dynamic>> callback) 
        {
            RmaPolicyInfo info = vm.ConvertVM<RmaPolicyVM, RmaPolicyInfo>();
            info.Status = RmaPolicyStatus.Active;
            info.IsOnlineRequest = vm.IsRequest ? IsOnlineRequst.YES : IsOnlineRequst.NO;
            info.User = new BizEntity.Common.UserInfo() 
            {
                SysNo=CPApplication.Current.LoginUser.UserSysNo,
                UserDisplayName=CPApplication.Current.LoginUser.DisplayName
            };
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.LanguageCode = CPApplication.Current.LanguageCode;
            string relativeUrl = "/IMService/RmaPolicy/CreateRmaPolicy";
            restClient.Create(relativeUrl, info, callback);
        }
        /// <summary>
        ///更新
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void UpdateRmaPolicy(RmaPolicyVM vm, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            RmaPolicyInfo info = vm.ConvertVM<RmaPolicyVM, RmaPolicyInfo>();
            info.Status = RmaPolicyStatus.Active;
            info.IsOnlineRequest = vm.IsRequest ? IsOnlineRequst.YES : IsOnlineRequst.NO;
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.LanguageCode = CPApplication.Current.LanguageCode;
            info.User = new BizEntity.Common.UserInfo()
            {
                SysNo = CPApplication.Current.LoginUser.UserSysNo,
                UserDisplayName = CPApplication.Current.LoginUser.DisplayName
            };
            string relativeUrl = "/IMService/RmaPolicy/UpdateRmaPolicy";
            restClient.Update(relativeUrl, info, callback);
        }
        /// <summary>
        ///批量作废
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void DeActiveRmaPolicy(List<RmaPolicyVM> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {

            List<RmaPolicyInfo> data = new List<RmaPolicyInfo>();
            list.ForEach(s =>
            {
                data.Add(new RmaPolicyInfo()
                {
                    SysNo = s.SysNo,
                    RmaType = s.RmaType,
                    ECDisplayName = s.ECDisplayName,
                    ChangeDate =Convert.ToInt32(s.ChangeDate),
                    ECDisplayDesc = s.ECDisplayDesc,
                    CompanyCode = CPApplication.Current.CompanyCode,
                    ECDisplayMoreDesc = s.ECDisplayMoreDesc,
                    IsOnlineRequest = s.IsRequest ? IsOnlineRequst.YES : IsOnlineRequst.NO,
                    LanguageCode = CPApplication.Current.LanguageCode,
                    Priority = s.Priority,
                    ReturnDate = Convert.ToInt32(s.ReturnDate),
                    RMAPolicyName = s.RMAPolicyName,
                    Status = s.Status,
                    User = new BizEntity.Common.UserInfo()
                    {
                        UserDisplayName = CPApplication.Current.LoginUser.LoginName,
                        SysNo = CPApplication.Current.LoginUser.UserSysNo
                    }


                });
            });
            string relativeUrl = "/IMService/RmaPolicy/DeActiveRmaPolicy";
            restClient.Update(relativeUrl, data, callback);
        }

        /// <summary>
        ///批量激活
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void ActiveRmaPolicy(List<RmaPolicyVM> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
             string relativeUrl = "/IMService/RmaPolicy/ActiveRmaPolicy";
             List<RmaPolicyInfo> data = new List<RmaPolicyInfo>();
             list.ForEach(s =>
             {
                 data.Add(new RmaPolicyInfo()
                     {
                         SysNo = s.SysNo,
                         RmaType = s.RmaType,
                         ECDisplayName = s.ECDisplayName,
                         ChangeDate =Convert.ToInt32( s.ChangeDate),
                         ECDisplayDesc = s.ECDisplayDesc,
                         CompanyCode = CPApplication.Current.CompanyCode,
                         ECDisplayMoreDesc = s.ECDisplayMoreDesc,
                         IsOnlineRequest = s.IsRequest ? IsOnlineRequst.YES : IsOnlineRequst.NO,
                         LanguageCode = CPApplication.Current.LanguageCode,
                         Priority = s.Priority,
                         ReturnDate = Convert.ToInt32(s.ReturnDate),
                         RMAPolicyName = s.RMAPolicyName,
                         Status = s.Status,
                         User = new BizEntity.Common.UserInfo() 
                         {
                             UserDisplayName=CPApplication.Current.LoginUser.LoginName,
                             SysNo=CPApplication.Current.LoginUser.UserSysNo
                         }


                     });
             });
             restClient.Update(relativeUrl, data, callback);
        }
       /// <summary>
        /// 根据SYsNo得到RmaPolicyInfo
       /// </summary>
       /// <param name="sysNo"></param>
       /// <param name="callback"></param>
        public void QueryRmaPolicyBySysNo(int SysNO, EventHandler<RestClientEventArgs<RmaPolicyInfo>> callback)
        {
            string relativeUrl = "/IMService/RmaPolicy/QueryRmaPolicyBySysNo";
          
             restClient.Query<RmaPolicyInfo>(relativeUrl, SysNO, callback);
        }

        public void GetRmaPolicyComboxList(EventHandler<RestClientEventArgs<List<RmaPolicyInfo>>> callback)
        {
            string relativeUrl = "/IMService/RmaPolicy/GetAllRmaPolicy";
            restClient.Query<List<RmaPolicyInfo>>(relativeUrl, null, callback);
        }
    }
}
