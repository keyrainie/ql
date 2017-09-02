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
    [VersionExport(typeof(ShipTypeAreaPriceAppService))]
    public  class ShipTypeAreaPriceAppService
    {
        private ShipTypeAreaPriceProcessor ShipTypeAreaPriceProcessor = ObjectFactory<ShipTypeAreaPriceProcessor>.Instance;
        public virtual void VoidShipTypeAreaPrice(List<int> sysnoList)
        {
            ShipTypeAreaPriceProcessor.VoidShipTypeAreaPrice(sysnoList);

        }
        public virtual ShipTypeAreaPriceInfo CreateShipTypeAreaPrice(ShipTypeAreaPriceInfo entity)
        {
            return ShipTypeAreaPriceProcessor.CreatShipTypeAreaPrice(entity);
        }
        public virtual void UpdateShipTypeAreaPrice(ShipTypeAreaPriceInfo entity)
        {
            ShipTypeAreaPriceProcessor.UpdateShipTypeAreaPrice(entity);
        }
    }
}
