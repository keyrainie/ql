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
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class PollFacade
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

        public PollFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询投票主题
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryPollList(PollQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/PollInfo/QueryPollList";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(PollQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/PollInfo/QueryPollList";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        #region 投票主题
        
        /// <summary>
        /// 创建投票主题
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void CreatePollMaster(PollMaster item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/PollInfo/CreatePollMaster";
            restClient.Create(relativeUrl, item, callback);
        }
            
        /// <summary>
        /// 加载投票主题
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void LoadPollMaster(int sysNo, EventHandler<RestClientEventArgs<PollMaster>> callback)
        {
            string relativeUrl = "/MKTService/PollInfo/LoadPollMaster";
            restClient.Query<PollMaster>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 修改投票主题
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void UpdatePollMaster(PollMaster item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/PollInfo/UpdatePollMaster";
            restClient.Update(relativeUrl, item, callback);
        }

        #endregion

        #region 投票问题组

        /// <summary>
        /// 创建投票问题
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void CreatePollItemGroup(PollItemGroup item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/PollInfo/CreatePollItemGroup";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 加载投票主题
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void GetPollItemGroupList(int sysNo, EventHandler<RestClientEventArgs<List<PollItemGroup>>> callback)
        {
            string relativeUrl = "/MKTService/PollInfo/GetPollItemGroupList";
            restClient.Query<List<PollItemGroup>>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 删除投票问题标题
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void DeletePollItemGroup(int sysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/PollInfo/DeletePollItemGroup";
            restClient.Update(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 更新投票问题标题
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void UpdatePollItemGroup(PollItemGroup item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/PollInfo/UpdatePollItemGroup";
            restClient.Update(relativeUrl, item, callback);
        }
        #endregion  

        #region 投票子项
        
        /// <summary>
        /// 创建投票问题相关投票子项
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void CreatePollItem(PollItem item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/PollInfo/CreatePollItem";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 加载投票主题下面相关的投票子项
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void GetPollItemList(int sysNo, EventHandler<RestClientEventArgs<List<PollItem>>> callback)
        {
            string relativeUrl = "/MKTService/PollInfo/GetPollItemList";
            restClient.Query<List<PollItem>>(relativeUrl, sysNo, callback);
        } 
        
        /// <summary>
        /// 加载投票主题下面相关的投票子项
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void GetPollAnswer(int sysNo, EventHandler<RestClientEventArgs<List<PollItemAnswer>>> callback)
        {
            string relativeUrl = "/MKTService/PollInfo/GetPollAnswer";
            restClient.Query<List<PollItemAnswer>>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 删除投票问题组投票子项
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void DeletePollItem(int sysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/PollInfo/DeletePollItem";
            restClient.Update(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 更新投票问题组投票子项
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void UpdatePollItem(PollItem item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/PollInfo/UpdatePollItem";
            restClient.Update(relativeUrl, item, callback);
        }
        #endregion
    }
}
