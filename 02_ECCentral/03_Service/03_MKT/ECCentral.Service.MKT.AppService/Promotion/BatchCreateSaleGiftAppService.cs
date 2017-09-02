using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.MKT.AppService.Promotion
{
    [VersionExport(typeof(BatchCreateSaleGiftAppService))]
    public class BatchCreateSaleGiftAppService
    {
        private BatchCreateSaleGiftProcessor _processor = ObjectFactory<BatchCreateSaleGiftProcessor>.Instance;

        public void BatchCreateSaleGift(SaleGiftBatchInfo saleGiftBatchInfo)
        {
            _processor.BatchCreateSaleGift(saleGiftBatchInfo);
        }
        
    }
}
