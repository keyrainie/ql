//************************************************************************
// 用户名				泰隆优选
// 系统名				商品价格变动申请单据
// 子系统名		        商品价格变动申请单据NoBizQuery查询接口实现
// 作成者				Tom
// 改版日				2012.4.26
// 改版内容				新建
//************************************************************************

using System.Data;
using ECCentral.QueryFilter.IM.Product.Request;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public interface IProductPriceRequestQueryDA
    {
        /// <summary>
        /// 查询商品价格变动申请单据
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        DataTable QueryProductPriceRequesList(ProductPriceRequestQueryFilter queryFilter, out int totalCount);


    }
}
