using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using System.Transactions;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(UnifiedImageProcessor))]
    public class UnifiedImageProcessor
    {
        public virtual void CreateUnifiedImage(UnifiedImage entity)
        {
            ObjectFactory<IUnifiedImageDA>.Instance.CreateUnifiedImage(entity);
        }
    }
}
