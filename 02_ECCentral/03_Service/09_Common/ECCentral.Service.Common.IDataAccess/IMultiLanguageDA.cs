using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    /// <summary>
    /// 多语言DA
    /// </summary>
    public interface IMultiLanguageDA
    {
        List<MultiLanguageBizEntity> GetMultiLanguageBizEntityList(MultiLanguageBizEntity entity);

        void SetMultiLanguageBizEntity(MultiLanguageBizEntity entity);

        List<Language> GetAllLanguageList();
    }
}
