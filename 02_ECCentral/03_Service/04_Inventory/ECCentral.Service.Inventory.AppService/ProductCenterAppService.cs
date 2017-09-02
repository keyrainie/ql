using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Inventory.BizProcessor;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.AppService
{
    [VersionExport(typeof(ProductCenterAppService))]
    public class ProductCenterAppService
    {
        public int CreateBasketItemsForPrepare(List<ProductCenterItemInfo> list)
        {
            return ObjectFactory<ProductCenterProcessor>.Instance.CreateBasketItemsForPrepare(list);
        }
    }
}
