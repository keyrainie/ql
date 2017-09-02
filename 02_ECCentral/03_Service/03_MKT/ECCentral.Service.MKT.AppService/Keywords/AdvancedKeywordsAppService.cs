using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor.Keywords;

namespace ECCentral.Service.MKT.AppService.Keywords
{
    [VersionExport(typeof(AdvancedKeywordsAppService))]
    public class AdvancedKeywordsAppService
    {
        /// <summary>
        /// 添加默认关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddAdvancedKeywords(AdvancedKeywordsInfo item)
        {
            ObjectFactory<AdvancedKeywordsProcessor>.Instance.AddAdvancedKeywords(item);
        }

        /// <summary>
        /// 编辑跳转关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void EditAdvancedKeywords(AdvancedKeywordsInfo item)
        {
            ObjectFactory<AdvancedKeywordsProcessor>.Instance.EditAdvancedKeywords(item);
        }

        /// <summary>
        /// 加载跳转关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual AdvancedKeywordsInfo LoadAdvancedKeywordsInfo(int sysNo)
        {
            return ObjectFactory<AdvancedKeywordsProcessor>.Instance.LoadAdvancedKeywordsInfo(sysNo);
        }
    }
}
