using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    public interface IConvertRequestQueryDA
    {
        /// <summary>
        /// 查询转换单List
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryConvertRequest(ConvertRequestQueryFilter queryFilter, out int totalCount);
    }
}
