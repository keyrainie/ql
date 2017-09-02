using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(MKTJOBAppService))]
    public class MKTJOBAppService
    {
        public void ClearTableOnLinelist(string day)
        {
            ObjectFactory<AutoInnerOnlineListProcessor>.Instance.ClearTableOnLinelist(day);
        }

        public List<ECCentral.BizEntity.SO.SOInfo> GetSOStatus()
        {
            return ExternalDomainBroker.GetSOStatus();
        }

        public void MakeOpered(int soSysNo)
        {
            ExternalDomainBroker.MakeOpered(soSysNo);
        }

        public void CheckComputerConfigInfo()
        {
            throw new NotImplementedException();
        }
    }
}
