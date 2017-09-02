using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(ShipTypeAreaPriceProcessor))]
    public class ShipTypeAreaPriceProcessor
    {
        private IShipTypeAreaPriceDA IShipTypeAreaPrice = ObjectFactory<IShipTypeAreaPriceDA>.Instance;
        public virtual void VoidShipTypeAreaPrice(List<int> sysnoList)
        {
            IShipTypeAreaPrice.VoidShipTypeAreaPrice(sysnoList);
        }
        public virtual ShipTypeAreaPriceInfo CreatShipTypeAreaPrice(ShipTypeAreaPriceInfo entity)
        {
            if (entity.AreaSysNoList != null && entity.AreaSysNoList.Count > 0 && !entity.AreaSysNo.HasValue)
            {
                foreach (int i in entity.AreaSysNoList)
                {
                    entity.AreaSysNo = i;
                    IShipTypeAreaPrice.CreateShipTypeAreaPrice(entity);
                }
                ShipTypeAreaPriceInfo returnEntity = new ShipTypeAreaPriceInfo();
                return returnEntity;
            }

            return IShipTypeAreaPrice.CreateShipTypeAreaPrice(entity);

        }
        public virtual void UpdateShipTypeAreaPrice(ShipTypeAreaPriceInfo entity)
        {
            IShipTypeAreaPrice.UpdateShipTypeAreaPrice(entity);
        }
    }
}
