using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Inventory.AppService
{
    [VersionExport(typeof(BatchManagementAppService))]
    public class BatchManagementAppService
    {

        public void UpdateProductRingInfo(ProductRingDayInfo entity)
        {
            ObjectFactory<BatchManagementProcessor>.Instance.UpdateProductRingInfo(entity);
        }

        public void AddProductRingInfo(ProductRingDayInfo entity)
        {
            ObjectFactory<BatchManagementProcessor>.Instance.AddProductRingInfo(entity);
        }
    }
}
