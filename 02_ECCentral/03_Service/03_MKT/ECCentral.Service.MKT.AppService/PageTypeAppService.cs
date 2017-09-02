using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT.PageType;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(PageTypeAppService))]
    public class PageTypeAppService
    {
        public virtual List<CodeNamePair> GetPageType(string companyCode, string channelID, ModuleType moduleType)
        {
            return ObjectFactory<PageTypeProcessor>.Instance.GetPageType(companyCode, channelID, moduleType);
        }

        public virtual PageResult GetPage(string companyCode, string channelID, ModuleType moduleType, string pageTypeID)
        {
            return ObjectFactory<PageTypeProcessor>.Instance.GetPage(companyCode, channelID, moduleType,pageTypeID);
        }

        private PageTypeProcessor _pageTypeProcessor = ObjectFactory<PageTypeProcessor>.Instance;

        //插入页面类型
        public virtual void Create(PageType entity)
        {
            _pageTypeProcessor.Create(entity);
        }

        //更新页面类型
        public virtual void Update(PageType entity)
        {
            _pageTypeProcessor.Update(entity);
        }

        //加载页面类型
        public virtual PageType Load(int sysNo)
        {
            return _pageTypeProcessor.Load(sysNo); ;
        }
    }
}
