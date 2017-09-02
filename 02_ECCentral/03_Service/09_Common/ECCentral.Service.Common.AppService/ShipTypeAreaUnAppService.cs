using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.BizProcessor;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(ShipTypeAreaUnAppService))]
   public  class ShipTypeAreaUnAppService
   {
       private ShipTypeAreaUnProcessor ShipTypeAreaUnProcessor = ObjectFactory<ShipTypeAreaUnProcessor>.Instance;
       public virtual void VoidShipTypeAreaUn(List<int> sysnoList)
       {
         ShipTypeAreaUnProcessor.VoidShipTypeAreaUn(sysnoList);
          
       }
       public virtual ErroDetail CreateShipTypAreaUn(ShipTypeAreaUnInfo entity)
       {
           return ShipTypeAreaUnProcessor.CreateShipTypeAreaUn(entity);
       }
   }

}
