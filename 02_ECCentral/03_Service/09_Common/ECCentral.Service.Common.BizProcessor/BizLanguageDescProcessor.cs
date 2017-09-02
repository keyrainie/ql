using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(BizLanguageDescProcessor))]
    public class BizLanguageDescProcessor
    {
        private readonly IBizLanguageDescDA _BizLanguageDescDA = ObjectFactory<IBizLanguageDescDA>.Instance;


        public bool InsertBizObjectLanguageDesc(BizObjectLanguageDesc entity)
        {
            BizObjectLanguageDesc info = _BizLanguageDescDA.GetBizObjectLanguageInfo(entity.BizObjectType, entity.LanguageCode, entity.BizObjectSysNo, entity.BizObjectId);
            if (info == null)
            {
                return _BizLanguageDescDA.InsertBizObjectLanguageDesc(entity);
            }
            else
            {
                entity.SysNo = info.SysNo;
                return UpdateBizObjectLanguageDesc(entity);
            }
        }

        public bool UpdateBizObjectLanguageDesc(BizObjectLanguageDesc entity)
        {
            return _BizLanguageDescDA.UpdateBizObjectLanguageDesc(entity);
        }


        public List<BizObjectLanguageDesc> GetBizObjectLanguageDescByBizTypeAndBizSysNo(string bizObjectType, string bizObjectSysNo, string bizObjectId)
        {
            return _BizLanguageDescDA.GetBizObjectLanguageDescByBizTypeAndBizSysNo(bizObjectType, bizObjectSysNo, bizObjectId);
        }
    }
}
