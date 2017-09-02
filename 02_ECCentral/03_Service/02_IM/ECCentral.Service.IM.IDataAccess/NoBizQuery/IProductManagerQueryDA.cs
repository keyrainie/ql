//************************************************************************
// 用户名				泰隆优选
// 系统名				PM管理
// 子系统名		       PM管理NoBizQuery查询接口
// 作成者				Tom
// 改版日				2012.4.24
// 改版内容				新建
//************************************************************************

using System.Data;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public interface IProductManagerQueryDA
    {
        /// <summary>
        /// 查询PM
        /// </summary>
        /// <returns></returns>
        DataTable QueryProductManagerInfo(ProductManagerQueryFilter queryCriteria, out int totalCount);
    }
}
