using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
   public interface IProductRMAPolicyDA
    {
       /// <summary>
       /// 根据productSysNo获取商品退换货信息
       /// </summary>
       /// <param name="productSysNos">productSysNo 集合</param>
       /// <returns></returns>
       List<ProductRMAPolicyInfo> GetProductRMAPolicyList(string productSysNos);
       /// <summary>
       /// 根据商品编号获取商品的退换货政策
       /// </summary>
       /// <param name="productSysNo"></param>
       /// <returns></returns>
       ProductRMAPolicyInfo GetProductRMAPolicyByProductSysNo(int? productSysNo);

       /// <summary>
       /// 创建商品退换货信息
       /// </summary>
       /// <param name="info"></param>
       void CreateProductRMAPolicy(ProductRMAPolicyInfo info);

       /// <summary>
       /// 更新商品退换货信息
       /// </summary>
       /// <param name="info"></param>
       void UpdateProductRMAPolicy(ProductRMAPolicyInfo info);
    }
}
