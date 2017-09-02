using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.BizProcessor;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(ShipTypeAppService))]
    public class ShipTypeAppService
    {
        ShipTypeProcessor ShipTypeProcessor = ObjectFactory<ShipTypeProcessor>.Instance;
        public void CreateShiType(ShippingType entity)
        
        {
            ShipTypeProcessor.CreateShipType(entity);
        }
        public ShippingType LoadShiType(int sysNo)
        {
           return  ShipTypeProcessor.LoadShipType(sysNo);
        }
        public void UpdateShiType(ShippingType entity)
        {
            ShipTypeProcessor.UpdateShipType(entity);
        }
    }
}
