using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
  public  interface IProductUseCouponLimitDA
    {
      /// <summary>
      /// 根据query获取特殊商品限制使用蛋卷信息
      /// </summary>
      /// <param name="query"></param>
      /// <param name="totalCount"></param>
      /// <returns></returns>
      DataTable GetProductUseCouponLimitByQuery(ProductUseCouponLimitQueryFilter query,out int totalCount);

      /// <summary>
      /// 创建
      /// </summary>
      /// <param name="info"></param>
      int CreateProductUseCouponLimit(ProductUseCouponLimitInfo info);

      /// <summary>
      /// 删除
      /// </summary>
      /// <param name="SysNo"></param>
      void DeleteProductUseCouponLimit(int SysNo);

      /// <summary>
      /// 更新状态
      /// </summary>
      /// <param name="SysNo"></param>
      void UpdateProductUseCouponLimitStatus(int SysNo,ADStatus status);



      #region Job行为
      List<ProductJobLimitProductInfo> GetLimitProductList(string datacommandname);
      ProductJobLimitProductInfo GetLimitProductByProductSysNo(int productSysNo);
      void CreateLimitProduct(ProductJobLimitProductInfo entity);
      #endregion
    }
}
