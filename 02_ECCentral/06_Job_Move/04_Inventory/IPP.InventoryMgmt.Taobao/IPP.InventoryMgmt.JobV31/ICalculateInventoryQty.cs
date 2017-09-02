using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.JobV31.BusinessEntities;


namespace IPP.InventoryMgmt.JobV31
{
    public interface ICalculateInventoryQty
    {
        //List<InventoryQtyEntity> CalculateQty(List<ProductEntity> entityList);

        //QueryProduct CreateQueryProduct();

        //List<ProductEntity> FilterModifyInventerResult(List<ProductEntity> entityList);

        /// <summary>
        /// 计算给第三方同步过去的库存增量
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int CalculateSynQty(ThirdPartInventoryEntity entity);

        int CalculateQty(ThirdPartInventoryEntity entity);
    }
}
