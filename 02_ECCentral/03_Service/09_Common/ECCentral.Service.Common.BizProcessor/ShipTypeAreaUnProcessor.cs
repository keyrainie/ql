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
    [VersionExport(typeof(ShipTypeAreaUnProcessor))]
    public class ShipTypeAreaUnProcessor
    {
        private IShipTypeAreaUnDA IShipTypeAreaUn = ObjectFactory<IShipTypeAreaUnDA>.Instance;
        public virtual void VoidShipTypeAreaUn(List<int> sysnoList)
        {
            IShipTypeAreaUn.VoidShipTypeAreaUn(sysnoList);
        }
        public virtual ErroDetail CreateShipTypeAreaUn(ShipTypeAreaUnInfo entity)
        {
            return IShipTypeAreaUn.CreateShipTypeAreaUn(entity);
        }

        public virtual List<ShipTypeAreaUnInfo> GetShipTypeAreaUnList(string companyCode)
        {
            return ObjectFactory<ICommonDA>.Instance.GetShipTypeAreaUnList(companyCode);
        }

        public List<ShipTypeAreaUnInfo> QueryShipAreaUnByAreaSysNo(IEnumerable<int> shipTypeSysNoS, int areaSysNo)
        {
            return ObjectFactory<IShipTypeAreaUnDA>.Instance.QueryShipAreaUnByAreaSysNo(shipTypeSysNoS, areaSysNo);
        }
    }
}
