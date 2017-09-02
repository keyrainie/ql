using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.BizProcessor;

namespace ECCentral.Service.Common.AppService
{
     [VersionExport(typeof(ShipTypePayTypeAppService))]
    public class ShipTypePayTypeAppService
    {
         public virtual ShipTypePayTypeInfo Create(ShipTypePayTypeInfo entity)
        {
            return ObjectFactory<ShipTypePayTypeProcessor>.Instance.Create(entity);
        }

         public virtual void Delete(List<int> sysNos)
        {
            ObjectFactory<ShipTypePayTypeProcessor>.Instance.DeleteBatch(sysNos);
        }
    }
}
