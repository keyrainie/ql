using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
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

namespace ECCentral.Portal.UI.IM.Facades.RmaPolicy
{
    public class DefaultRMAPolicyFacade
    {
        #region Private Method
        private readonly RestClient restClient;
        private const string GetDefaultRMAPolicyByQueryUrl = "/IMService/DefaultRMAPolicy/GetDefaultRMAPolicyByQuery";
        private const string DefaultRMAPolicyInfoAddUrl = "/IMService/DefaultRMAPolicy/DefaultRMAPolicyInfoAdd";
        private const string UpdateDefaultRMAPolicyBySysNoUrl = "/IMService/DefaultRMAPolicy/UpdateDefaultRMAPolicyBySysNo";
        private const string DelDelDefaultRMAPolicyBySysNoBySysNosUrl = "/IMService/DefaultRMAPolicy/DelDelDefaultRMAPolicyBySysNoBySysNos";
        #endregion

        #region Method
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window
                    .Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

         public DefaultRMAPolicyFacade()
         {
            restClient = new RestClient(ServiceBaseUrl);
         }

         public DefaultRMAPolicyFacade(IPage page)
         {
            restClient = new RestClient(ServiceBaseUrl, page);
         }

        //查询退换货政策设置
         public void GetDefaultRMAPolicy(DefaultRMAPolicyFilter query, int PageSize, int PageIndex
             , string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
         {
             query.PageInfo = new PagingInfo
             {
                 PageSize = PageSize,
                 PageIndex = PageIndex,
                 SortBy = SortField
             };
             restClient.QueryDynamicData(GetDefaultRMAPolicyByQueryUrl, query, (obj, args) =>
             {
                 if (args.FaultsHandle())
                 {
                     return;
                 }
                 callback(obj, args);
             });
         }

        //添加退换货政策设置
         public void DefaultRMAPolicyInfoAdd(RmaPolicySettingQueryVM vm
             , EventHandler<RestClientEventArgs<dynamic>> callback)
         {
             DefaultRMAPolicyInfo data = new DefaultRMAPolicyInfo();
             data.BrandSysNo = vm.BrandSysNo;
             data.C3SysNo = vm.C3SysNo;
             data.RMAPolicySysNo = vm.RMAPolicySysNo;
             data.EditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.UserSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
             data.CreateUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.UserSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
             restClient.Update(DefaultRMAPolicyInfoAddUrl, data, callback);
         }

         //更新退换货政策设置
         public void UpdateDefaultRMAPolicy(RmaPolicySettingQueryVM vm
             , EventHandler<RestClientEventArgs<dynamic>> callback)
         {
             DefaultRMAPolicyInfo data = new DefaultRMAPolicyInfo();
             data.SysNo = vm.SysNo;
             data.RMAPolicySysNo = vm.RMAPolicySysNo;
             data.EditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.UserSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
             restClient.Update(UpdateDefaultRMAPolicyBySysNoUrl, data, callback);
         }
        //删除退换货政策设置
         public void DelDefaultRMAPolicy(List<Int32> sysNos
             , EventHandler<RestClientEventArgs<dynamic>> callback)
         {
             List<DefaultRMAPolicyInfo> datas = new List<DefaultRMAPolicyInfo>();
             sysNos.ForEach(p =>
             {
                 datas.Add(new DefaultRMAPolicyInfo()
                 {
                     SysNo = p,
                     EditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.UserSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName }
                 });
             });
             restClient.Delete(DelDelDefaultRMAPolicyBySysNoBySysNosUrl, datas, callback);
         }
        #endregion
    }
}