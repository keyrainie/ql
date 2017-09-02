using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using System.Transactions;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(UnifiedImageAppService))]
    public class UnifiedImageAppService
    {
        public void CreateUnifiedImage(UnifiedImage entity)
        {
            ObjectFactory<UnifiedImageProcessor>.Instance.CreateUnifiedImage(entity);
        }
    }
}
