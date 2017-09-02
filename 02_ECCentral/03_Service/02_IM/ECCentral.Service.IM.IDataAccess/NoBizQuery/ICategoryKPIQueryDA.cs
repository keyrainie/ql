using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public interface ICategoryKPIQueryDA
    {
        /// <summary>
        /// 查询类别KPI指标
        /// </summary>
        /// <returns></returns>
        DataTable QueryCategoryKPIList(CategoryKPIQueryFilter queryCriteria, out int totalCount);
    }
}
