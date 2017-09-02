using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Service.IM.IDataAccess
{
   public interface IProductSalesAreaBatchDA
    {
       /// <summary>
       /// 根据query获取商品信息
       /// </summary>
       /// <param name="query"></param>
       /// <param name="totalCount"></param>
       /// <returns></returns>
       DataTable GetProductByQuery(ProductSalesAreaBatchQueryFilter query, out int totalCount);

       /// <summary>
       /// 得到所有省份
       /// </summary>
       /// <returns></returns>
       DataTable GetAllProvince();

       /// <summary>
       /// 根据query得到有设置区域的商品
       /// </summary>
       /// <param name="query"></param>
       /// <param name="totalCount"></param>
       /// <returns></returns>
       DataTable GetProductSalesAreaBatchList(ProductSalesAreaBatchQueryFilter query, out int totalCount);

       /// <summary>
       /// 移除销售区域 
       /// </summary>
       /// <param name="Info"></param>
       void RemoveItemSalesAreaListBatch(ProductSalesAreaBatchInfo Info);

       /// <summary>
       /// 移除省份
       /// </summary>
       /// <param name="info"></param>
       void RemoveProvinceByProductSysNo(ProductSalesAreaBatchInfo info);
    }
}
