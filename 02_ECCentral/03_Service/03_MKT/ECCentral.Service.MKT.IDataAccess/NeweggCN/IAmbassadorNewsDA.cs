using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IAmbassadorNewsDA
    {
        void BatchUpdateAmbassadorNewsStatus(AmbassadorNewsBatchInfo batchInfo);
    }
}
