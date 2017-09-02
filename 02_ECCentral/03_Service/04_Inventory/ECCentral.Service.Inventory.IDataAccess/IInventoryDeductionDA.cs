using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface IInventoryDeductionDA
    {
        int ExecuteInventoryDeduction(string paramXml);

        int GetProductTypeByItemSysNumber(int itemSysNumber);
    }
}
