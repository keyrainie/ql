using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.BizProcessor;
using ECCentral.BizEntity.Common;


namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(ShipTypeProductAppService))]
   public  class ShipTypeProductAppService
    {
        public virtual void VoidShipTypeProduct(List<int?> sysnoList)
        {
            ObjectFactory<ShipTypeProductProcessor>.Instance.VoidShipTypeProduct(sysnoList);
        }
        public virtual void CreateShipTypeProduct(ShipTypeProductInfo ShipTypeProductInfo)
        {
            ObjectFactory<ShipTypeProductProcessor>.Instance.CreateShipTypeProduct(ShipTypeProductInfo);
        }
    }
}
