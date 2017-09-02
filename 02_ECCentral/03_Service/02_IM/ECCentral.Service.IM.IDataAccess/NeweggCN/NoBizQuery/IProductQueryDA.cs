using System.Data;
using ECCentral.QueryFilter.IM;
using System.Collections.Generic;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    partial interface IProductQueryDA
    {
        /// <summary>
        /// 查询商品
        /// </summary>
        /// <returns></returns>
        DataTable QueryProductEx(NeweggProductQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询导出商品
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        DataTable QueryExporterEntryFile(List<int> productSysNoList);
    }
}
