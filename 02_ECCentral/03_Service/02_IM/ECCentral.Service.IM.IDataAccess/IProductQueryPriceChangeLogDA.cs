using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;
namespace ECCentral.Service.IM.IDataAccess
{
   public interface IProductQueryPriceChangeLogDA
    {
       DataTable GetProductQueryPriceChangeLog(ProductPriceChangeLogQueryFilter query, out int totalCount);
    }
}
