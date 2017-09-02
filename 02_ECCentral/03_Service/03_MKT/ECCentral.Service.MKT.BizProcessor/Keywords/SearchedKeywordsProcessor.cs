using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.Service.MKT.IDataAccess;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(SearchedKeywordsProcessor))]
    public class SearchedKeywordsProcessor
    {
        private ISearchedKeywordsDA keywordDA = ObjectFactory<ISearchedKeywordsDA>.Instance;

       
        #region 自动匹配关键字（SearchedKeyword）
        /// <summary>
        /// 添加自动匹配的搜索关键字    IPP系统中默认CreateUserType=0。
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddSearchedKeywords(SearchedKeywords item)
        {
            if (keywordDA.CheckSearchKeywords(item))
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_ExsitTheKeywords"));
            else
                keywordDA.AddSearchedKeywords(item);
        }

        /// <summary>
        /// 编辑自动匹配关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void EditSearchedKeywords(SearchedKeywords item)
        {
            if (!keywordDA.CheckSearchKeywords(item))
                keywordDA.EditSearchedKeywords(item);
            else
                //throw new BizException("已存在该关键字，不能修改!");
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_ExsitTheKeywords"));
        }

        /// <summary>
        /// 更新自动匹配关键字屏蔽状态       ??
        /// </summary>
        /// <param name="item"></param>
        public virtual void ChangeSearchedKeywordsStatus(List<int> items)
        {
            keywordDA.ChangeSearchedKeywordsStatus(items);
        }

        /// <summary>
        /// 删除自动匹配关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void DeleteSearchedKeywords(List<int> items)
        {
            keywordDA.DeleteSearchedKeywords(items);
        }

        /// <summary>
        /// 加载自动匹配关键字
        /// </summary>
        /// <returns></returns>
        public virtual SearchedKeywords LoadSearchedKeywords(int sysNo)
        {
            return keywordDA.LoadSearchedKeywords(sysNo);
        }

        /// <summary>
        /// 加载编辑用户
        /// </summary>
        /// <returns></returns>
        public virtual List<UserInfo> LoadEditUsers(string companyCode)
        {
            return keywordDA.LoadEditUsers(companyCode);
        }
        #endregion

    }
}
