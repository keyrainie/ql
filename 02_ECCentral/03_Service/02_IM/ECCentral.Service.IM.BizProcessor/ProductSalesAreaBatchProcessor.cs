using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM.Product;
namespace ECCentral.Service.IM.BizProcessor
{
     [VersionExport(typeof(ProductSalesAreaBatchProcessor))]
   public class ProductSalesAreaBatchProcessor
    {
         private readonly IProductSalesAreaBatchDA _productSalesAreaBatchDA = ObjectFactory<IProductSalesAreaBatchDA>.Instance;
         public void RemoveItemSalesAreaListBatch(List<ProductSalesAreaBatchInfo> listInfo)
         {
             foreach (var item in listInfo)
             {
                 _productSalesAreaBatchDA.RemoveItemSalesAreaListBatch(item);
             }
         }
         /// <summary>
         /// 移除省份
         /// </summary>
         /// <param name="info"></param>
         public void RemoveProvince(ProductSalesAreaBatchInfo info)
         {
             _productSalesAreaBatchDA.RemoveProvinceByProductSysNo(info);
         }
    }
}
