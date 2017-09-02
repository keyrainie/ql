using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
   public interface IRmaPolicyLogQueryDA
    {
       /// <summary>
       ///查询退换货日志
       /// </summary>
       /// <param name="filter"></param>
       /// <returns></returns>
     DataTable GetRmaPolicyLog(RmaPolicyLogQueryFilter filter,out int totalCount);

    }
}
