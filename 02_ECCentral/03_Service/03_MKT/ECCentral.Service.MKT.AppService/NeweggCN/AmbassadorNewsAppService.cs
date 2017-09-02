using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(AmbassadorNewsAppService))]
    public class AmbassadorNewsAppService
    {
        private AmbassadorNewsProcessor _ambassadorNewsProcessor = ObjectFactory<AmbassadorNewsProcessor>.Instance;

        public void BatchUpdateAmbassadorNewsStatus(BizEntity.MKT.AmbassadorNewsBatchInfo batchInfo)
        {
            _ambassadorNewsProcessor.BatchUpdateAmbassadorNewsStatus(batchInfo);
        }
    }
}
