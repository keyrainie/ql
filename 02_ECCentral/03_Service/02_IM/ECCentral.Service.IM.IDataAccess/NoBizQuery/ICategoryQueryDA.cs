using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public interface ICategoryQueryDA
    {
        DataTable QueryCategory(CategoryQueryFilter queryCriteria, out int totalCount);

        List<CategoryInfo> QueryCategory1(CategoryQueryFilter queryFilter);

        List<CategoryInfo> QueryCategory2(CategoryQueryFilter queryFilter);

        List<CategoryInfo> QueryCategory3(CategoryQueryFilter queryFilter);

        List<CategoryInfo> QueryAllCategory2(CategoryQueryFilter queryFilter);

        List<CategoryInfo> QueryAllCategory3(CategoryQueryFilter queryFilter);

        List<CategoryInfo> QueryAllPrimaryCategory(CategoryQueryFilter queryFilter);
    }
}
