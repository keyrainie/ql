//************************************************************************
// 用户名				泰隆优选
// 系统名				品牌管理
// 子系统名		        品牌管理NoBizQuery查询接口
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************
using System.Data;

using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public interface IBrandQueryDA
    {
        /// <summary>
        /// 查询品牌
        /// </summary>
        /// <returns></returns>
        DataTable QueryBrand(BrandQueryFilter queryCriteria, out int totalCount);
        DataTable QueryBrandInfo(BrandQueryFilter queryCriteria, out int totalCount);
    }
}
