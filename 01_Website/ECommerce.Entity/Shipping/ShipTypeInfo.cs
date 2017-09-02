using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Shipping
{
    [Serializable]
    public class ShipTypeInfo
    {
        /// <summary>
        /// 配送方式编号
        /// </summary>
        public int ShipTypeSysNo { get; set; }

        /// <summary>
        /// 配送方式ID
        /// </summary>
        public string ShipTypeID { get; set; }

        /// <summary>
        /// 配送方式名称
        /// </summary>
        public string ShipTypeName { get; set; }

        /// <summary>
        /// 配送方式描述
        /// </summary>
        public string ShipTypeDesc { get; set; }

        /// <summary>
        /// 配送方式提供方
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// 制定出库仓编号
        /// </summary>
        public int OnlyForStockSysNo { get; set; }

        /// <summary>
        /// 是否是自提
        /// </summary>
        public bool IsGetBySelf { get; set; }

        /// <summary>
        /// 是否支持货到付款
        /// </summary>
        public bool IsPayWhenRecv { get; set; }

        /// <summary>
        /// 保价费费率
        /// </summary>
        public decimal PremiumRate { get; set; }

        /// <summary>
        /// 存储运输方式
        /// </summary>
        public int StoreType { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        ///  地址
        /// </summary>
        public string Address { get; set; }
    }
    /// <summary>
    /// 配送方式比较器
    /// </summary>
    public class ShipTypeInfoEqualityComparer : IEqualityComparer<ShipTypeInfo>
    {
        public bool Equals(ShipTypeInfo x, ShipTypeInfo y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            if (Object.ReferenceEquals(x, y))
            {
                return true;
            }
            return x.ShipTypeName.Equals(y.ShipTypeName, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(ShipTypeInfo obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
