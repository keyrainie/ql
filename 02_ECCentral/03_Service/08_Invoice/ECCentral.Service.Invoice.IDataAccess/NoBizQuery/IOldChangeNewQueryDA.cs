using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Invoice;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface IOldChangeNewQueryDA
    {
        /// <summary>
        /// 查询以旧换新补贴款信息
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">返回总记录数</param>
        /// <returns></returns>
        DataTable OldChangeNewQuery(OldChangeNewQueryFilter filter, out int totalCount);

        /// <summary>
        /// 获取以旧换新补贴款列表信息
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        List<OldChangeNewInfo> GetOldChangeNewList(OldChangeNewQueryFilter filter);

        /// <summary>
        /// Check以旧换新信息是否有效
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        bool IsOldChangeNewSO(OldChangeNewQueryFilter filter);
    }
}
