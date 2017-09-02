using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(ShippingAddressProcessor))]
    public class ShippingAddressProcessor
    {
        private IShippingAddressDA shippingAddrDA = ObjectFactory<IShippingAddressDA>.Instance;
        #region 收货地址信息

        /// <summary>
        /// 创建收货地址信息
        /// </summary>
        /// <param name="shippingAddress"></param>
        /// <returns></returns>
        public virtual ShippingAddressInfo CreateShippingAddress(ShippingAddressInfo shippingAddress)
        {
            return shippingAddrDA.CreateShippingAddress(shippingAddress);
        }

        /// <summary>
        /// 更新收货地址信息
        /// </summary>
        /// <param name="shippingAddress"></param>
        /// <returns></returns>
        public virtual void UpdateShippingAddress(ShippingAddressInfo shippingAddress)
        {
            shippingAddrDA.UpdateShippingAddress(shippingAddress);
        }

        /// <summary>
        /// 查询收货地址列表
        /// </summary>
        /// <param name="customerSysNo"> 顾客系统编号</param>
        /// <returns></returns>
        public virtual List<ShippingAddressInfo> QueryShippingAddress(int customerSysNo)
        {
            return shippingAddrDA.QueryShippingAddress(customerSysNo);
        }

        /// <summary>
        /// 获取默认配送地址
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public virtual ShippingAddressInfo GetDefaultShippingAddress(int customerSysNo)
        {
            return QueryShippingAddress(customerSysNo).Where(p => p.IsDefault.Value).FirstOrDefault();
        }
 
        #endregion 收货地址信息
    }
}
