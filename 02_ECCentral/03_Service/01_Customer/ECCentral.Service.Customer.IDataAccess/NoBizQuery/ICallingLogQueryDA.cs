using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Customer;

namespace ECCentral.Service.Customer.IDataAccess.NoBizQuery
{
    public interface ICallingLogQueryDA
    {
        /// <summary>
        /// 查询订单列表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QuerySOList(CustomerCallingQueryFilter filter, out int totalCount);
        /// <summary>
        /// 查询RMA列表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryRMAList(CustomerCallingQueryFilter filter, out int totalCount);
        /// <summary>
        /// 查询投诉列表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryComplainList(CustomerCallingQueryFilter filter, out int totalCount);
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryCallingLog(CustomerCallingQueryFilter filter, out int totalCount);
        /// <summary>
        /// 查询RMA单件列表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable GetRMARegisterList(RMARegisterQueryFilter filter, out int totalCount);


        DataTable QueryCallsEventLog(CustomerCallsEventLogFilter filter, out int totalCount);

        DataTable GetUpdateUser(string companyCode);
    }
}
