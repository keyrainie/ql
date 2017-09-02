using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(AmbassadorNewsProcessor))]
    public class AmbassadorNewsProcessor
    {
        private IAmbassadorNewsDA _ambassadorNewsDA = ObjectFactory<IAmbassadorNewsDA>.Instance;

        public void BatchUpdateAmbassadorNewsStatus(BizEntity.MKT.AmbassadorNewsBatchInfo batchInfo)
        {
            _ambassadorNewsDA.BatchUpdateAmbassadorNewsStatus(batchInfo);
        }
    }
}
