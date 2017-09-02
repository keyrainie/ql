using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;
namespace ECCentral.Service.IM.IDataAccess
{
   public  interface  IProductRelatedDA
    {
       /// <summary>
       /// 根据query得到相关商品信息
       /// </summary>
       /// <param name="query"></param>
       /// <param name="totalCount"></param>
       /// <returns></returns>
       DataTable GetProductRelatedByQuery(ProductRelatedQueryFilter query, out int totalCount);

       /// <summary>
       /// 创建ItemRelated
       /// </summary>
       /// <param name="info"></param>
       /// <returns></returns>
       int CreateProductRelate(ProductRelatedInfo info);

       /// <summary>
       /// Delete
       /// </summary>
       /// <param name="sysNo"></param>
       void DeleteProductRelate(string sysNo);

       /// <summary>
       /// 更新优先级
       /// </summary>
       /// <param name="info"></param>
       void UpdateProductRelatePriority(ProductRelatedInfo info);
    }
}
