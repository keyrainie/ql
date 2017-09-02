using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess
{
   public interface IProductNotifyDA
    {
       /// <summary>
       /// 根据query得到到货通知信息
       /// </summary>
       /// <param name="query"></param>
       /// <param name="totalCount"></param>
       /// <returns></returns>
       DataTable GetProductNotifyByQuery(ProductNotifyQueryFilter query, out int totalCount);
    }
}
