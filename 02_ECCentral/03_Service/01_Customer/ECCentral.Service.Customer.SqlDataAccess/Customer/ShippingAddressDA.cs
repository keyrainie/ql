using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(IShippingAddressDA))]
    public class ShippingAddressDA : IShippingAddressDA
    {
        #region IShippingAddressDA Members

        public virtual ShippingAddressInfo CreateShippingAddress(ShippingAddressInfo shippingAddress)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertShippingAddress");
            cmd.SetParameterValue<ShippingAddressInfo>(shippingAddress);
            cmd.ExecuteNonQuery();
            shippingAddress.SysNo = int.Parse(cmd.GetParameterValue("@SysNo").ToString());
            return shippingAddress;
        }

        public virtual void UpdateShippingAddress(ShippingAddressInfo shippingAddress)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateShippingAddress");
            cmd.SetParameterValue<ShippingAddressInfo>(shippingAddress);
            cmd.ExecuteNonQuery();
        }

        public virtual List<ShippingAddressInfo> QueryShippingAddress(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetShippingAddress");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            return cmd.ExecuteEntityList<ShippingAddressInfo>();
        }
        #endregion IShippingAddressDA Members
    }
}