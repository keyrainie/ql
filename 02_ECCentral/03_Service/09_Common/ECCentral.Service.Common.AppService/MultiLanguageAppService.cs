using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.BizProcessor;
using ECCentral.BizEntity;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(MultiLanguageAppService))]
    public class MultiLanguageAppService
    {

        public List<MultiLanguageBizEntity> GetMultiLanguageBizEntityList(MultiLanguageDataContract dataContract)
        {
            List<MultiLanguageBizEntity> list = ObjectFactory<MultiLanguageProcessor>.Instance.GetMultiLanguageBizEntityList(dataContract);
            return list;
        }

        public void SetMultiLanguageBizEntity(MultiLanguageBizEntity entity)
        {
            ObjectFactory<MultiLanguageProcessor>.Instance.SetMultiLanguageBizEntity(entity);
        }
    }
}
