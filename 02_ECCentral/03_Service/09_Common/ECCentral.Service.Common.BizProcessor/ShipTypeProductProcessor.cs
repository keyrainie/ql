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
    [VersionExport(typeof(ShipTypeProductProcessor))]
    public class ShipTypeProductProcessor
    {
        private IShipTypeProductDA ShipTypeProductDA = ObjectFactory<IShipTypeProductDA>.Instance;
        /// <summary>
        /// 删除配送方式-产品
        /// </summary>
        /// <param name="item"></param>
        public virtual void VoidShipTypeProduct(List<int?> sysnoList)
        {
            //foreach (int sysno in sysnoList)
            //{
            ShipTypeProductDA.VoidShipTypeProduct(sysnoList);

           // }
        }
        public virtual void CreateShipTypeProduct(ShipTypeProductInfo ShipTypeProductInfo)
        {
            ShipTypeProductDA.CreateShipTypeProduct(ShipTypeProductInfo);
        }
    }
}
