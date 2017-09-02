using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Inventory.SqlDataAccess
{
    [VersionExport(typeof(IInventorySellerDA))]
    public class InventorySellerDA : IInventorySellerDA
    {

        #region IInventorySellerDA Members

        public int CreateAdjust4WMSCheck(string xmlMsg)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateAdjust4WMSCheck");
            command.SetParameterValue("@Msg", xmlMsg);
            return command.ExecuteNonQuery();
        }

        #endregion
    }
}
