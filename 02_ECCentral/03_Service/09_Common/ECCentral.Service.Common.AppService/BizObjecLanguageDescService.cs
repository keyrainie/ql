using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.BizProcessor;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(BizObjecLanguageDescService))]
    public class BizObjecLanguageDescService
    {
        public bool InsertBizObjectLanguageDesc(BizObjectLanguageDesc entity)
        {
            return ObjectFactory<BizLanguageDescProcessor>.Instance.InsertBizObjectLanguageDesc(entity);
        }


        public bool UpdateBizObjectLanguageDesc(BizObjectLanguageDesc entity)
        {
            return ObjectFactory<BizLanguageDescProcessor>.Instance.UpdateBizObjectLanguageDesc(entity);
        }


        public List<BizObjectLanguageDesc> GetBizObjectLanguageDescByBizTypeAndBizSysNo(string bizObjectType, string bizObjectSysNo, string bizObjectId)
        {
            return ObjectFactory<BizLanguageDescProcessor>.Instance.GetBizObjectLanguageDescByBizTypeAndBizSysNo(bizObjectType, bizObjectSysNo, bizObjectId);
        }
    }
}
