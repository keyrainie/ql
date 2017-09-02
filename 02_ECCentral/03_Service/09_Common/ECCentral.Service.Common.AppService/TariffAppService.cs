using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.BizProcessor;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(TariffAppService))]
    public class TariffAppService
    {
        public virtual List<TariffInfo> QueryTariffCategory(string tariffcode)
        {
            var s = ObjectFactory<TariffProcessor>.Instance.QueryTariffCategory(tariffcode);
            return s;
        }


        public virtual TariffInfo CreateTariffInfo(TariffInfo entity)
        {
            return ObjectFactory<TariffProcessor>.Instance.CreateTariffInfo(entity);
        }


        public virtual TariffInfo GetTariffInfo(int sysNo)
        {
            return ObjectFactory<TariffProcessor>.Instance.GetTariffInfo(sysNo);
        }

        public virtual bool UpdateTariffInfo(TariffInfo entity)
        {
            return ObjectFactory<TariffProcessor>.Instance.UpdateTariffInfo(entity);
        }

    }
}
