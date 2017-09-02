using ECCentral.QueryFilter.Inventory.Request;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    public interface IProductBatchQueryDA
    {
        /// <summary>
        /// 查询商品的批次信息
        /// </summary>
        /// <param name="batchFilter"></param>
        /// <returns></returns>
        DataTable QueryProductBatch(ProductBatchQueryFilter batchFilter,out int totalCount);
    }
}
