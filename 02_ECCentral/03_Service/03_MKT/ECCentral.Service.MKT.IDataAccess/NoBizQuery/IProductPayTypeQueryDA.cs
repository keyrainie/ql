using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.MKT.Promotion;
using ECCentral.QueryFilter.MKT.Promotion;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IProductPayTypeQueryDA
    {
        /// <summary>
        /// 查询商品支付方式
        /// </summary>
        /// <returns></returns>
        DataTable QueryProductPayType(ProductPayTypeQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询支付方式列表
        /// </summary>
        /// <returns></returns>
        List<PayTypeInfo> GetProductPayTypeList();
    }
}
