using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Shipping;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Shipping
{
    public class ShippingDA
    {
        /// <summary>
        /// 取得所有配送方式
        /// </summary>
        /// <returns></returns>
        public static List<ShipTypeInfo> GetAllShipTypeList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Shipping_GetAllShipTypeList");
            List<ShipTypeInfo> shipTypeList = cmd.ExecuteEntityList<ShipTypeInfo>();

            return shipTypeList;
        }

        /// <summary>
        /// 取得配送方式-地区-非的配置
        /// </summary>
        /// <returns></returns>
        public static List<ShipTypeAndAreaUnInfo> GetAllShippingTypeAndAreaUnList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Shipping_GetShippingTypeAndAreaUnList");
            List<ShipTypeAndAreaUnInfo>  allShippingTypeAndAreaUnList = cmd.ExecuteEntityList<ShipTypeAndAreaUnInfo>();

            return allShippingTypeAndAreaUnList;
        }
    }
}
