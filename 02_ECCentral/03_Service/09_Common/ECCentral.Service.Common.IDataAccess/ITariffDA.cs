using System.Collections.Generic;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface ITariffDA
    {
        List<TariffInfo> QueryTariffCategory(string tariffcode);

        TariffInfo CreateTariffInfo(TariffInfo entity);

        TariffInfo GetTariffInfo(int sysNo);

        bool UpdateTariffInfo(TariffInfo entity);

        TariffInfo GetTariffInfoByName(string itemCategoryName);

        TariffInfo GetTariffInfoByTariffCode(string tariffCode);
    }
}