using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public interface IProductGroupQueryDA
    {
        /// <summary>
        /// 商品组查询
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryProductGroupInfo(ProductGroupQueryFilter queryCriteria, out int totalCount);
    }
}
