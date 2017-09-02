using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
   public interface IProductShowDA
    {
       /// <summary>
       /// 根据query得到商品上架信息
       /// </summary>
       /// <param name="query"></param>
       /// <param name="totalCount"></param>
       /// <returns></returns>
       DataTable GetProductShowByQuery(ProductShowQueryFilter query, out int totalCount);
    }
}
