using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Inventory.BizProcessor
{
    [VersionExport(typeof(BatchManagementProcessor))]
    public class BatchManagementProcessor
    {

        public void UpdateProductRingInfo(ProductRingDayInfo entity)
        {
            ObjectFactory<IBatchManagementDA>.Instance.UpdateProductRingInfo(entity);
        }

        public void AddProductRingInfo(ProductRingDayInfo entity)
        {
            ObjectFactory<IBatchManagementDA>.Instance.InsertProductRingInfo(entity);
        }
    }
}
