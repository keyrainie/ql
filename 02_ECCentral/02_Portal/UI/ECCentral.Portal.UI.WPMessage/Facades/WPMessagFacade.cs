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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using System.Collections.Generic;
using ECCentral.Service.WPMessage.Restful.RequestMsg;
using ECCentral.Portal.UI.WPMessage.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using WPM = ECCentral.WPMessage.BizEntity;
using ECCentral.WPMessage.QueryFilter;
using ECCentral.Portal.Basic;


namespace ECCentral.Portal.UI.WPMessage.Facades
{
    public class WPMessagFacade
    {

        private readonly RestClient restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public WPMessagFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Common, "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }


        //private string ServiceBaseUrl
        //{
        //    get
        //    {
        //        return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_SO, ConstValue.Key_ServiceBaseUrl);
        //    }
        //}

        public WPMessagFacade()
            : this(CPApplication.Current.CurrentPage)
        {

        }

        /// <summary>
        /// 查询用户角色列表
        /// </summary>
        /// <param name="callback"></param>
        public void QueryAllRoles(SystemRoleQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PagingInfo = new ECCentral.QueryFilter.Common.PagingInfo()   //这个PagingInfo是我们在ECCentral.BizEntity里自己定义的，只有这3个属性
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/CommonService/AuthCenter/SystemRole/Query";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void GetCurrentUserWPMessage(WPMessageQueryFilter req, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/WPMService/Message/Query";
            restClient.QueryDynamicData(relativeUrl, req, callback);
        }


        /// <summary>
        /// 待办事项的类别
        /// </summary>
        /// <param name="callback"></param>
        public void GetAllCategory(EventHandler<RestClientEventArgs<List<WPM.WPMessageCategory>>> callback)
        {
            string relativeUrl = "/WPMService/Category/GetAll";
            restClient.Query<List<WPM.WPMessageCategory>>(relativeUrl, callback);
        }

        public void GetCategoryByUserSysNo(int userSysNo, EventHandler<RestClientEventArgs<List<WPM.WPMessageCategory>>> callback)
        {
            string relativeUrl = "/WPMService/Category/Get/" + userSysNo.ToString();
            restClient.Query<List<WPM.WPMessageCategory>>(relativeUrl, callback);
        }

        /// <summary>
        /// 获取类别角色
        /// </summary>
        /// <param name="categorySysNo"></param>
        /// <param name="callback"></param>
        public void GetCategoryRole(string categorySysNo, EventHandler<RestClientEventArgs<List<int>>> callback)
        {
            string relativeUrl = "/WPMService/Category/GetRole/" + categorySysNo;
            restClient.Query<List<int>>(relativeUrl, callback);
        }
        /// <summary>
        /// 更新类别角色
        /// </summary>
        /// <param name="req"></param>
        /// <param name="callback"></param>
        public void UpdateCategoryRole(UpdateWPMessageCategoryRoleReq req, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/WPMService/Category/UpdateRole";
            restClient.Update(relativeUrl, req, callback);
        }

        public void UpdateMessageToProcessing(int msgSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/WPMService/Message/Processing";
            restClient.Update(relativeUrl, msgSysNo, callback);
        }



        /// <summary>
        /// 获取角色类别
        /// </summary>
        /// <param name="categorySysNo"></param>
        /// <param name="callback"></param>
        public void GetRoleCategory(string roleSysNo, EventHandler<RestClientEventArgs<List<int>>> callback)
        {
            string relativeUrl = "/WPMService/Category/GetCategory/" + roleSysNo;
            restClient.Query<List<int>>(relativeUrl, callback);
        }

        /// <summary>
        /// 更新角色类别
        /// </summary>
        /// <param name="req"></param>
        /// <param name="callback"></param>
        public void UpdateRoleCategory(UpdateWPMessageCategoryRoleByRoleSysNoReq req, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/WPMService/Category/UpdateRoleCategory";
            restClient.Update(relativeUrl, req, callback);
        }

        /// <summary>
        /// 检查当前用户是否存在待办事项
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <param name="callback"></param>
        public void CheckCurrentUserHasWPMessage(string userSysNo, EventHandler<RestClientEventArgs<bool>> callback)
        {
            try
            {
                ServiceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Common, "ServiceBaseUrl");
                string relativeUrl = "/WPMService/Message/HasMesssage/" + userSysNo;
                restClient.Query<bool>(relativeUrl, callback);
            }
            catch { }
        }
    }
}
