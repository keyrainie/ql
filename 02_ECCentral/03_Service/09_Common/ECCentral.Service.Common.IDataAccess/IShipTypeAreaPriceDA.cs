using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
   public  interface IShipTypeAreaPriceDA
    {
        /// <summary>
        /// 删除配送方式-地区-价格(非)
        /// </summary>
        /// <param name="sysnoList"></param>
        void VoidShipTypeAreaPrice(List<int> sysnoList);
        /// <summary>
        /// 新增配送方式-地区-价格(非)
        /// </summary>
        /// <param name="entity"></param>
        ShipTypeAreaPriceInfo CreateShipTypeAreaPrice(ShipTypeAreaPriceInfo entity);
        /// <summary>
        /// 新增配送方式-地区-价格(非)
        /// </summary>
        /// <param name="entity"></param>
        void UpdateShipTypeAreaPrice(ShipTypeAreaPriceInfo entity);
   }
}
