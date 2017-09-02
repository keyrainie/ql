//************************************************************************
// 用户名				泰隆优选
// 系统名				类别属性管理
// 子系统名		        类别属性管理NoBizQuery查询接口
// 作成者				Tom
// 改版日				2012.5.21
// 改版内容				新建
//************************************************************************

using System.Data;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public interface ICategoryPropertyQueryDA
    {
        DataTable QueryCategoryProperty(CategoryPropertyQueryFilter queryCriteria, out int totalCount);

    }
}
