using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess;
using ECommerce.DataAccess.Shipping;
using ECommerce.Entity.Shipping;

namespace ECommerce.Facade.Shipping
{
    /// <summary>
    /// 用户收货地址管理
    /// </summary>
    public class CustomerShippingAddresssFacade
    {
        /// <summary>
        /// 取得用户的收货地址信息
        /// </summary>
        /// <param name="contactAddressSysNo">收货地址信息</param>
        /// <param name="customerSysNo">用户系统编号</param>
        /// <returns>收货地址信息</returns>
        public static ShippingContactInfo GetCustomerShippingAddress(int contactAddressSysNo, int customerSysNo)
        {
            return CustomerShippingAddressDA.Load(contactAddressSysNo, customerSysNo);
        }

        /// <summary>
        /// 取得用户的收货地址列表
        /// </summary>
        /// <param name="customerSysNo">用户系统编号</param>
        /// <returns>收货地址列表</returns>
        public static List<ShippingContactInfo> GetCustomerShippingAddressList(int customerSysNo)
        {
            List<ShippingContactInfo> customerShippingAddressList = CustomerShippingAddressDA.GetCustomerShippingAddressList(customerSysNo);

            if (customerShippingAddressList != null && customerShippingAddressList.Count > 0)
            {
                var theDefault = customerShippingAddressList.Find(x => x.IsDefault);
                if (theDefault == null)
                {
                    theDefault = customerShippingAddressList.First();
                    theDefault.IsDefault = true;
                }
            }
            return customerShippingAddressList;
        }

        /// <summary>
        /// 编辑用户的收货地址，包括新建和更新
        /// </summary>
        /// <param name="customerShippingAddress">收货地址信息</param>
        /// <param name="customerSysNo">用户系统编号</param>
        public static ShippingContactInfo EditCustomerContactInfo(ShippingContactInfo customerShippingAddress, int customerSysNo)
        {
            if (customerShippingAddress.SysNo > 0)
            {
                customerShippingAddress=CustomerShippingAddressDA.Update(customerShippingAddress, customerSysNo);
            }
            else
            {
                customerShippingAddress.CustomerSysNo = customerSysNo;
                customerShippingAddress = CustomerShippingAddressDA.Create(customerShippingAddress);
            }
            return customerShippingAddress;
        }

        /// <summary>
        /// 更新用户收货地址和支付方式&配送方式的对应关系
        /// </summary>
        /// <param name="customerShippingAddressSysNo">用户收货地址编号</param>
        /// <param name="paymentCateId">支付类型</param>
        /// <param name="shipTypeSysNo">配送方式编号</param>
        /// <param name="customerSysNo">用户系统编号</param>
        public static void UpdateCustomerContactInfo(int customerShippingAddressSysNo, int paymentCateID, int shipTypeSysNo, int customerSysNo)
        {
            CustomerShippingAddressDA.UpdateRelationship(customerShippingAddressSysNo, paymentCateID, shipTypeSysNo, customerSysNo);
        }

        /// <summary>
        /// 删除用户的一个收货地址
        /// </summary>
        /// <param name="contactAddressSysNo">收货地址系统编号</param>
        /// <param name="customerSysNo">用户系统编号</param>
        public static void DeleteCustomerContactInfo(int contactAddressSysNo, int customerSysNo)
        {
            var customerShippingAddress = GetCustomerShippingAddress(contactAddressSysNo, customerSysNo);
            if (customerShippingAddress != null)
            {
                CustomerShippingAddressDA.Delete(contactAddressSysNo, customerSysNo);
            }
        }
        /// <summary>
        /// 设置收货地址为默认收货地址 
        /// </summary>
        /// <param name="contactAddressSysNo">收货地址系统编号</param>
        /// <param name="customerSysNo">用户系统编号</param>
        public static void SetCustomerContactAsDefault(int contactAddressSysNo, int customerSysNo)
        {
            CustomerShippingAddressDA.SetAsDefault(contactAddressSysNo, customerSysNo);
        }
    }
}
