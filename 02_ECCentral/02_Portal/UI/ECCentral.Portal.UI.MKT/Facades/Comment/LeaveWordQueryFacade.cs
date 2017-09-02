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
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class LeaveWordQueryFacade
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

        public LeaveWordQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询客户留言
        /// </summary>
        /// <param name="callback"></param>
        public void QueryLeaveWord(LeaveWordQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/CommentInfo/QueryLeaveWord";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(LeaveWordQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/CommentInfo/QueryLeaveWord";
            restClient.ExportFile(relativeUrl, filter, columns);
        }
        /// <summary>
        /// 加载客户留言
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void LoadLeaveWord(int sysNo, EventHandler<RestClientEventArgs<LeaveWordsItem>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/LoadLeaveWord";
            restClient.Query<LeaveWordsItem>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 更新客户留言
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void UpdateLeaveWord(LeaveWordsItem item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/UpdateLeaveWord";
            restClient.Update(relativeUrl, item, callback);
        }

        /// <summary>
        /// 给客户发送邮件
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void SendCustomerEmailForLeaveWord(LeaveWordsItem item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/SendCustomerEmailForLeaveWord";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 加载编辑客户留言人员列表
        /// </summary>
        /// <param name="callback"></param>
        public void GetLeaveWordProcessUser(EventHandler<RestClientEventArgs<List<UserInfo>>> callback)
        {
            string relativeUrl = "/MKTService/CommentInfo/GetLeaveWordProcessUser";
            restClient.Query(relativeUrl, CPApplication.Current.CompanyCode, callback);
        }

    }
}
