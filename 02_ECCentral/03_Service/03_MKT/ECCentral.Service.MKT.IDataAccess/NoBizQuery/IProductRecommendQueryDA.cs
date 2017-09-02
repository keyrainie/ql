using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IProductRecommendQueryDA
    {
        DataTable Query(ProductRecommendQueryFilter filter, out int totalCount);
    }
}
