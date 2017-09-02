using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(DefaultKeywordsAppService))]
    public class DefaultKeywordsAppService
    {
        #region 默认关键字（DefaultKeywordsInfo）
        /// <summary>
        /// 添加默认关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddDefaultKeywords(DefaultKeywordsInfo item)
        {
            ObjectFactory<DefaultKeywordsProcessor>.Instance.AddDefaultKeywords(item);
        }

        /// <summary>
        /// 编辑默认关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void EditDefaultKeywords(DefaultKeywordsInfo item)
        {
            ObjectFactory<DefaultKeywordsProcessor>.Instance.EditDefaultKeywords(item);
        }

        /// <summary>
        /// 加载默认关键字
        /// </summary>
        /// <returns></returns>
        public virtual DefaultKeywordsInfo LoadDefaultKeywordsInfo(int sysNo)
        {
            return ObjectFactory<DefaultKeywordsProcessor>.Instance.LoadDefaultKeywordsInfo(sysNo);
        }

        #endregion
    }
}
