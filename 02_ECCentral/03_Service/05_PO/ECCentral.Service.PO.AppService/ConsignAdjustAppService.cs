using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.PO.AppService
{
    [VersionExport(typeof(ConsignAdjustAppService))]
    public class ConsignAdjustAppService
    {
        public ConsignAdjustInfo MaintainStatus(ConsignAdjustInfo info)
        {
            return ObjectFactory<ConsignAdjustProcessor>.Instance.MaintainStatus(info);
        }

        public ConsignAdjustInfo Create(ConsignAdjustInfo info)
        {
            return ObjectFactory<ConsignAdjustProcessor>.Instance.Create(info);
        }
    }
}
