using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Customer;

namespace ECCentral.Service.Customer.IDataAccess.NoBizQuery
{
    public interface IRefundAdjustQueryDA
    {
        /// <summary>
        /// 查询补偿退款单
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <returns></returns>
        DataTable RefundAdjustQuery(RefundAdjustQueryFilter filter,out int totalCount);

        /// <summary>
        /// 节能补贴导出
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        DataTable QueryEnergySubsidyExport(RefundAdjustQueryFilter filter);

        /// <summary>
        /// 节能补贴查询
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        DataTable QueryEnergySubsidy(RefundAdjustQueryFilter filter,out int totalCount);
    }
}
