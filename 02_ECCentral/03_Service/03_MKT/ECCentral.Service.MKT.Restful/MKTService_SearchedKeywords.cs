using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.Common;
using ECCentral.Service.MKT.AppService;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private SearchedKeywordsAppService searchedKeywordsAppService = ObjectFactory<SearchedKeywordsAppService>.Instance;


        #region 自动匹配关键字（SearchedKeyword）
        /// <summary>
        /// 添加自动匹配的搜索关键字    IPP系统中默认CreateUserType=0。
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/AddSearchedKeywords", Method = "POST")]
        public virtual void AddSearchedKeywords(SearchedKeywords item)
        {
            searchedKeywordsAppService.AddSearchedKeywords(item);
        }

        /// <summary>
        /// 编辑自动匹配关键字
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/EditSearchedKeywords", Method = "PUT")]
        public virtual void EditSearchedKeywords(SearchedKeywords item)
        {
            searchedKeywordsAppService.EditSearchedKeywords(item);
        }

        /// <summary>
        /// 更新自动匹配关键字屏蔽状态       ??
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/ChangeSearchedKeywordsStatus", Method = "PUT")]
        public virtual void ChangeSearchedKeywordsStatus(List<int> items)
        {
            searchedKeywordsAppService.ChangeSearchedKeywordsStatus(items);
        }

        /// <summary>
        /// 删除自动匹配关键字
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/DeleteSearchedKeywords", Method = "PUT")]
        public virtual void DeleteSearchedKeywords(List<int> items)
        {
            searchedKeywordsAppService.DeleteSearchedKeywords(items);
        }

        /// <summary>
        /// 加载自动匹配关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/LoadSearchedKeywords", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual SearchedKeywords LoadSearchedKeywords(int sysNo)
        {
            return searchedKeywordsAppService.LoadSearchedKeywords(sysNo);
        }

        /// <summary>
        /// 加载编辑用户
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/LoadEditUsers", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual List<UserInfo> LoadEditUsers(string companyCode)
        {
            return searchedKeywordsAppService.LoadEditUsers(companyCode);
        }

        /// <summary>
        /// 查询自动匹配关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/QuerySearchedKeywords", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QuerySearchedKeywords(SearchedKeywordsFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IKeywordQueryDA>.Instance.QuerySearchedKeywords(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion

    }
}
