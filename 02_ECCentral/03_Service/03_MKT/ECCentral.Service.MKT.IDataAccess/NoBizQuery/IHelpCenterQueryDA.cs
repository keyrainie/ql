using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IHelpCenterQueryDA
    {
        /// <summary>
        /// 帮助中心查询
        /// </summary>
        DataTable Query(HelpCenterQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询帮助中心的帮助分类
        /// </summary>
        DataTable QueryCategory(HelpCenterCategoryQueryFilter filter);
    }
}
