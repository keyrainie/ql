using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{

    public interface ISearchedKeywordsDA
    {
        #region 自动匹配关键字（SearchedKeyword）

        /// <summary>
        /// 检查是否存在相同的关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CheckSearchKeywords(SearchedKeywords item);

        /// <summary>
        /// 添加自动匹配的搜索关键字    IPP系统中默认CreateUserType=0。
        /// </summary>
        /// <param name="item"></param>
        void AddSearchedKeywords(SearchedKeywords item);

        /// <summary>
        /// 编辑自动匹配关键字
        /// </summary>
        /// <param name="item"></param>
        void EditSearchedKeywords(SearchedKeywords item);

        /// <summary>
        /// 更新自动匹配关键字屏蔽状态       ??
        /// </summary>
        /// <param name="item"></param>
        void ChangeSearchedKeywordsStatus(List<int> items);

        /// <summary>
        /// 删除自动匹配关键字
        /// </summary>
        /// <param name="item"></param>
        void DeleteSearchedKeywords(List<int> items);

        /// <summary>
        /// 加载自动匹配关键字
        /// </summary>
        /// <returns></returns>
        SearchedKeywords LoadSearchedKeywords(int sysNo);

        /// <summary>
        /// 加载编辑用户
        /// </summary>
        /// <returns></returns>
        List<ECCentral.BizEntity.Common.UserInfo> LoadEditUsers(string companyCode);
        #endregion

    }
}
