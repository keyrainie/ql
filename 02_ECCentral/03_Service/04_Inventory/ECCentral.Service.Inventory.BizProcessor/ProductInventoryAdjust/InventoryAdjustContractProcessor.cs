using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.Service.Utility;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.BizProcessor
{
    [VersionExport(typeof(InventoryAdjustContractProcessor))]
    public class InventoryAdjustContractProcessor
    {
        public virtual string ProcessAdjustContract(InventoryAdjustContractInfo adjustContractInfo)
        {            
            InventoryAdjustSourceBizFunction sourceBizFunction = adjustContractInfo.SourceBizFunctionName;
                     
            IProductInventoryAdjustProcessor productInventoryAdjustProcessor = ObjectFactory<ProductInventoryAdjustProcessorFactory>.Instance.CreateProductInventoryAdjustProcessor(sourceBizFunction);
            productInventoryAdjustProcessor.AdjustProductInventory(adjustContractInfo);
            return productInventoryAdjustProcessor.AdjustResultMsg;
        }
    }
}
