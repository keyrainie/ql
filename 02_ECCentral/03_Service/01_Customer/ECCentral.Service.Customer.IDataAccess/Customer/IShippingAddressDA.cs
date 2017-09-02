using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface IShippingAddressDA
    {
        /// <summary>
        /// 创建收货地址信息
        /// </summary>
        /// <param name="shippingAddress">收货地址BizEntity</param>
        /// <returns>收货地址BizEntity</returns>
        ShippingAddressInfo CreateShippingAddress(ShippingAddressInfo shippingAddress);

        /// <summary>
        /// 更新收货地址信息
        /// </summary>
        /// <param name="shippingAddress">收货地址BizEntity</param>
        /// <returns>收货地址BizEntity</returns>
        void UpdateShippingAddress(ShippingAddressInfo shippingAddress);

        /// <summary>
        /// 查询收货地址列表
        /// </summary>
        /// <param name="customerSysNo">顾客系统编号</param>
        /// <returns>收货地址BizEntity列表</returns>
        List<ShippingAddressInfo> QueryShippingAddress(int customerSysNo);

   
    }
}