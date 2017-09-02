using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface IBatchManagementDA
    {
        void UpdateProductRingInfo(ProductRingDayInfo entity);

        void InsertProductRingInfo(ProductRingDayInfo entity);
    }
}
