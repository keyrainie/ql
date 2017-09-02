using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
namespace ECCentral.Service.Common.IDataAccess
{
   public   interface  IShipTypeProductDA
    {
        /// <summary>
        /// 删除配送方式-产品
        /// </summary>
        /// <param name="sysnoList"></param>
        void VoidShipTypeProduct(List<int?> sysnolist);
        /// <summary>
        /// 新增配送方式-产品
        /// </summary>
        /// <param name="sysnoList"></param>
        void CreateShipTypeProduct(ShipTypeProductInfo ShipTypeProductInfo);
    }
}
