using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Transactions;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductSalesAreaBatchAppService))]
    public class ProductSalesAreaBatchAppService
    {
        public void RemoveItemSalesAreaListBatch(List<ProductSalesAreaBatchInfo> listInfo)
        {
            ObjectFactory<ProductSalesAreaBatchProcessor>.Instance.RemoveItemSalesAreaListBatch(listInfo);
        }
         /// <summary>
         /// 移除省份
         /// </summary>
         /// <param name="info"></param>
        public void RemoveProvince(ProductSalesAreaBatchInfo info)
        {
            ObjectFactory<ProductSalesAreaBatchProcessor>.Instance.RemoveProvince(info);
        }
    }
}
