using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Inventory.SqlDataAccess
{
    public class InventoryDeductionDA : IInventoryDeductionDA
    {
        #region IInventoryDeductionDA Members

        public int ExecuteInventoryDeduction(string paramXml)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InventoryDeductionManager");
            command.SetParameterValue("@Msg", paramXml);
            return command.ExecuteNonQuery();
        }

        public int GetProductTypeByItemSysNumber(int itemSysNumber)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductTypeByItemSysNumber");
            command.SetParameterValue("@itemSysNumber", itemSysNumber);
            return Convert.ToInt32(command.ExecuteScalar().ToString());
        }
        #endregion
    }
}
