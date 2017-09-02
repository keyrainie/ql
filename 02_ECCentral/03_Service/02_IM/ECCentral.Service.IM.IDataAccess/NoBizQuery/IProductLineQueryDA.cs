using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public interface IProductLineQueryDA
    {
        List<ProductManagerInfo> GetUserNameList(string strList);

        DataTable QueryProductLineList(ProductLineFilter filter, out int totalCount);

    }
}
