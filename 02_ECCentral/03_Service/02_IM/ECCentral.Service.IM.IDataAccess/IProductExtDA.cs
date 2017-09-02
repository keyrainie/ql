using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
   public interface IProductExtDA
    {
       /// <summary>
       /// 根据商品条件得到商品信息
       /// </summary>
       /// <param name="query"></param>
       /// <returns></returns>
       DataTable GetProductExtByQuery(ProductExtQueryFilter query, out int totalCount);

      /// <summary>
      /// 设置是否可以退货
      /// </summary>
      /// <param name="SysNo"></param>
      /// <param name="IsPermitRefund"></param>
       void UpdatePermitRefund(ProductExtInfo info);

       int UpdateProductExKeyKeywords(int sysNo, string keywords, string keywords0, int editUserSysNo, string companyCode);

       void UpdateIsBatch(ProductBatchManagementInfo batchManagementInfo);

       ProductBatchManagementInfo GetProductBatchManagementInfo(int productSysNo);

       void InsertProductBatchManagementLog(ProductBatchManagementInfoLog entity);

       List<ProductBatchManagementInfoLog> GetProductBatchManagementLogByBatchManagementSysNo(int batchManagementSysNo);
    }
}
