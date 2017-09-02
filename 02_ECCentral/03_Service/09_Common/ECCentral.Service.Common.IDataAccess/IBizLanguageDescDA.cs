using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface IBizLanguageDescDA
    {
        bool InsertBizObjectLanguageDesc(BizObjectLanguageDesc entity);


        bool UpdateBizObjectLanguageDesc(BizObjectLanguageDesc entity);


        List<BizObjectLanguageDesc> GetBizObjectLanguageDescByBizTypeAndBizSysNo(string bizObjectType, string bizObjectSysNo, string bizObjectId);

        BizObjectLanguageDesc GetBizObjectLanguageInfo(string bizObjectType, string languageCode, int? bizObjectSysNo, string bizObjectId);
    }
}
