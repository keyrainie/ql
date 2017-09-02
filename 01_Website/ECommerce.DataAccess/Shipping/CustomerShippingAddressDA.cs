using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Shipping;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Shipping
{
    public class CustomerShippingAddressDA
    {
        public static ShippingContactInfo Load(int contactAddressSysNo, int customerSysno)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Shipping_GetShippingAddressInfo");
            dataCommand.SetParameterValue("@SysNo", contactAddressSysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysno);
            return dataCommand.ExecuteEntity<ShippingContactInfo>();
        }

        public static List<ShippingContactInfo> GetCustomerShippingAddressList(int customerSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Shipping_GetShippingAddressList");
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            return dataCommand.ExecuteEntityList<ShippingContactInfo>();
        }

        public static void Delete(int contactAddressSysNo, int customerSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Shipping_DeleteShippingAddress");
            dataCommand.SetParameterValue("@SysNo", contactAddressSysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.ExecuteNonQuery();
        }

        public static ShippingContactInfo Update(ShippingContactInfo customerShippingAddress, int customerSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Shipping_UpdateShippingAddress");
            dataCommand.SetParameterValue("@SysNo", customerShippingAddress.SysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.SetParameterValue("@AddressTitle", customerShippingAddress.AddressTitle);
            dataCommand.SetParameterValue("@IsDefault", customerShippingAddress.IsDefault);
            dataCommand.SetParameterValue("@ReceiveName", customerShippingAddress.ReceiveName);
            dataCommand.SetParameterValue("@ReceiveContact", customerShippingAddress.ReceiveContact);
            dataCommand.SetParameterValue("@ReceivePhone", customerShippingAddress.ReceivePhone);
            dataCommand.SetParameterValue("@ReceiveCellPhone", customerShippingAddress.ReceiveCellPhone);
            dataCommand.SetParameterValue("@ReceiveFax", customerShippingAddress.ReceiveFax);
            dataCommand.SetParameterValue("@ReceiveAreaSysNo", customerShippingAddress.ReceiveAreaSysNo);
            dataCommand.SetParameterValue("@ReceiveAddress", customerShippingAddress.ReceiveAddress);
            dataCommand.SetParameterValue("@ReceiveZip", customerShippingAddress.ReceiveZip);
            return dataCommand.ExecuteEntity<ShippingContactInfo>();
        }

        public static ShippingContactInfo Create(ShippingContactInfo customerShippingAddress)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Shipping_CreateShippingAddress");
            dataCommand.SetParameterValue("@CustomerSysNo", customerShippingAddress.CustomerSysNo);
            dataCommand.SetParameterValue("@AddressTitle", customerShippingAddress.AddressTitle);
            dataCommand.SetParameterValue("@IsDefault", customerShippingAddress.IsDefault);
            dataCommand.SetParameterValue("@ReceiveName", customerShippingAddress.ReceiveName);
            dataCommand.SetParameterValue("@ReceiveContact", customerShippingAddress.ReceiveContact);
            dataCommand.SetParameterValue("@ReceivePhone", customerShippingAddress.ReceivePhone);
            dataCommand.SetParameterValue("@ReceiveCellPhone", customerShippingAddress.ReceiveCellPhone);
            dataCommand.SetParameterValue("@ReceiveFax", customerShippingAddress.ReceiveFax);
            dataCommand.SetParameterValue("@ReceiveAreaSysNo", customerShippingAddress.ReceiveAreaSysNo);
            dataCommand.SetParameterValue("@ReceiveAddress", customerShippingAddress.ReceiveAddress);
            dataCommand.SetParameterValue("@ReceiveZip", customerShippingAddress.ReceiveZip);

            customerShippingAddress.SysNo = Convert.ToInt32(dataCommand.ExecuteScalar());
            customerShippingAddress = Load(customerShippingAddress.SysNo, customerShippingAddress.CustomerSysNo);
            return customerShippingAddress;
        }

        public static void UpdateRelationship(int customerShippingAddressSysNo, int paymentCateID, int shipTypeSysNo, int customerSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Shipping_UpdateShippingAddressRelationship");
            dataCommand.SetParameterValue("@SysNo", customerShippingAddressSysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.SetParameterValue("@PaymentCategoryID", paymentCateID);
            dataCommand.SetParameterValue("@ShipTypeSysNo", shipTypeSysNo);

            dataCommand.ExecuteNonQuery();
        }

        public static void SetAsDefault(int contactAddressSysNo, int customerSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Shipping_SetShippingAddressAsDefault");
            dataCommand.SetParameterValue("@SysNo", contactAddressSysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.ExecuteNonQuery();
        }
    }
}
