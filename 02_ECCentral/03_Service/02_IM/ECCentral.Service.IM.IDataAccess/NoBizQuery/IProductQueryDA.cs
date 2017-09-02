using System.Data;

using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public partial interface IProductQueryDA
    {
        /// <summary>
        /// 查询商品
        /// </summary>
        /// <returns></returns>
        DataTable QueryProduct(ProductQueryFilter queryCriteria, out int totalCount);
    }
}
