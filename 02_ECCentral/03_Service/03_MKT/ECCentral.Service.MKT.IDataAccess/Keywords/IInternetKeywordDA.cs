using System.Collections.Generic;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.MKT.Keyword;

namespace ECCentral.Service.MKT.IDataAccess
{
    /// <summary>
    /// 外网搜素优化关键字
    /// </summary>
    public interface IInternetKeywordDA
    {
        /// <summary>
        /// 插入外网搜索关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        InternetKeywordInfo InsertKeyword(InternetKeywordInfo item);

        /// <summary>
        ///修改外网搜索关键字状态
        /// </summary>
        /// <param name="item"></param>
        InternetKeywordInfo UpdateKeywordStatus(InternetKeywordInfo item);

        /// <summary>
        /// 根据keyword获取InternetKeywordInfo实体
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        InternetKeywordInfo GetInternetKeywordInfoByKeyword(string keyword);

        /// <summary>
        /// 根据sysNo获取InternetKeywordInfo实体
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        InternetKeywordInfo GetInternetKeywordInfoBySysNo(string sysNo);
    }
}
