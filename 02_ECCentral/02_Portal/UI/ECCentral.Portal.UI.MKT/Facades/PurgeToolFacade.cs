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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.Models;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class PurgeToolFacade
    {

        private const string GetPurgeToolByQueryUrl = "/MKTService/PurgeTool/GetPurgeToolByQuery";
        private const string CreatePurgeToolUrl = "/MKTService/PurgeTool/CreatePurgeTool";
         private readonly RestClient restClient;

        /// <summary>
        /// IMService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");                
            }
        }

        public PurgeToolFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public PurgeToolFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 获取PurgeTool信息
        /// </summary>
        /// <param name="clearType"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void GetPurgeToolByQuery(ClearType clearType, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            PurgeToolQueryFilter query = new PurgeToolQueryFilter()
            {
                PageInfo = new QueryFilter.Common.PagingInfo() {PageIndex=PageIndex,PageSize=PageSize,SortBy=SortField },
                ClearType=clearType
            };

            restClient.QueryDynamicData(GetPurgeToolByQueryUrl, query, callback);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        public void CreatePurgeTool(PurgeToolVM vm,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            List<PurgeToolInfo> data = new List<PurgeToolInfo>();
            string result=vm.UrlList.Replace('\r',' ');
            string[] arr = result.Split(';');
             foreach (var item in arr)
             {
                 if (!string.IsNullOrEmpty(item))
                 {
                     data.Add(new PurgeToolInfo()
                     {
                         Url = item.Trim(),
                         ClearDate = vm.ClearDate == null ? DateTime.Now : vm.ClearDate,
                         Priority = Convert.ToInt32(string.IsNullOrEmpty(vm.Priority) ? "0" : vm.Priority),
                         CompanyCode = CPApplication.Current.CompanyCode,
                         LanguageCode = CPApplication.Current.LanguageCode,
                         User = new BizEntity.Common.UserInfo() { UserName = CPApplication.Current.LoginUser.LoginName, SysNo = CPApplication.Current.LoginUser.UserSysNo }
                     });
                 }
             }
             restClient.Create(CreatePurgeToolUrl, data, callback);
        }
    }
}
