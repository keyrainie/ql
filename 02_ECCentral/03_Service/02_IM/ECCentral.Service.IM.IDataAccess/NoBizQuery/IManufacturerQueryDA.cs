//************************************************************************
// 用户名				泰隆优选
// 系统名				厂商管理
// 子系统名		        厂商管理NoBizQuery查询接口
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************
using System.Data;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public interface IManufacturerQueryDA
    {

        /// <summary>
        /// 查询生产商
        /// </summary>
        /// <returns></returns>
        DataTable QueryManufacturer(ManufacturerQueryFilter queryCriteria, out int totalCount);

        /// <summary>
        /// 查询品牌旗舰店首页分类 
        /// </summary>
        /// <returns></returns>
        DataTable GetManufacturerCategoryBySysNo(ManufacturerQueryFilter queryCriteria, out int totalCount);
    }
}
