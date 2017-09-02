using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(SearchedKeywordsAppService))]
    public class SearchedKeywordsAppService
    {
        #region 自动匹配关键字（SearchedKeyword）
        /// <summary>
        /// 添加自动匹配的搜索关键字    IPP系统中默认CreateUserType=0。
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddSearchedKeywords(SearchedKeywords item)
        {
            ObjectFactory<SearchedKeywordsProcessor>.Instance.AddSearchedKeywords(item);
        }

        /// <summary>
        /// 编辑自动匹配关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void EditSearchedKeywords(SearchedKeywords item)
        {
            ObjectFactory<SearchedKeywordsProcessor>.Instance.EditSearchedKeywords(item);
        }

        /// <summary>
        /// 更新自动匹配关键字屏蔽状态       ??
        /// </summary>
        /// <param name="item"></param>
        public virtual void ChangeSearchedKeywordsStatus(List<int> items)
        {
            ObjectFactory<SearchedKeywordsProcessor>.Instance.ChangeSearchedKeywordsStatus(items);
        }

        /// <summary>
        /// 删除自动匹配关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void DeleteSearchedKeywords(List<int> items)
        {
            ObjectFactory<SearchedKeywordsProcessor>.Instance.DeleteSearchedKeywords(items);
        }

        /// <summary>
        /// 加载自动匹配关键字
        /// </summary>
        /// <returns></returns>
        public virtual SearchedKeywords LoadSearchedKeywords(int sysNo)
        {
            return ObjectFactory<SearchedKeywordsProcessor>.Instance.LoadSearchedKeywords(sysNo);
        }

        /// <summary>
        /// 加载编辑用户
        /// </summary>
        /// <returns></returns>
        public virtual List<ECCentral.BizEntity.Common.UserInfo> LoadEditUsers(string companyCode)
        {
            return ObjectFactory<SearchedKeywordsProcessor>.Instance.LoadEditUsers(companyCode);
        }

        #endregion
    }
}
