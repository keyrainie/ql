using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    /// <summary>
    /// 默认关键字
    /// </summary>
    public interface IDefaultKeywordsDA
    {
        /// <summary>
        /// 检查是否存在该默认关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CheckDuplicate(DefaultKeywordsInfo item);

        /// <summary>
        /// 添加默认关键字
        /// </summary>
        /// <param name="item"></param>
        void AddDefaultKeywords(DefaultKeywordsInfo item);

        /// <summary>
        /// 编辑默认关键字
        /// </summary>
        /// <param name="item"></param>
        void EditDefaultKeywords(DefaultKeywordsInfo item);

        /// <summary>
        /// 加载默认关键字
        /// </summary>
        /// <returns></returns>
        DefaultKeywordsInfo LoadDefaultKeywordsInfo(int sysNo);
    }
}
