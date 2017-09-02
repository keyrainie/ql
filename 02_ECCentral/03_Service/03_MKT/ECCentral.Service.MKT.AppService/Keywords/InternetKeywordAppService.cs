using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity.MKT.Keyword;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(InternetKeywordAppService))]
    public class InternetKeywordAppService
    {
        #region 外网搜索优化（SearchedKeyword）
        /// <summary>
        /// 创建外网搜索
        /// </summary>
        /// <param name="keywordInfos"></param>
        /// <returns></returns>
        public virtual void CreateKeyword(List<InternetKeywordInfo> keywordInfos)
        {
            using (var scope = new TransactionScope())
            {
                ObjectFactory<InternetKeywordProcessor>.Instance.CreateKeyword(keywordInfos);
                scope.Complete();
            }
             
        }

        /// <summary>
        /// 查询外网搜索
        /// </summary>
        /// <param name="internetKeywords"></param>
        public void ModifyKeywordStatus(List<InternetKeywordInfo> internetKeywords)
        {
            using (var scope = new TransactionScope())
            {
                ObjectFactory<InternetKeywordProcessor>.Instance.ModifyKeywordStatus(internetKeywords);
                scope.Complete();
            }

        }

        #endregion
    }
}
