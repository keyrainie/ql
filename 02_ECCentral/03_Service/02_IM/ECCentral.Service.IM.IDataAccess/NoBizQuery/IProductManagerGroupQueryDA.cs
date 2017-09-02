//************************************************************************
// 用户名				泰隆优选
// 系统名				PM组管理
// 子系统名		       PM组管理NoBizQuery查询接口
// 作成者				Tom
// 改版日				2012.4.24
// 改版内容				新建
//************************************************************************

using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM.ProductManager;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public interface IProductManagerGroupQueryDA
    {
        /// <summary>
        /// 查询PM组
        /// </summary>
        /// <returns></returns>
        DataTable QueryProductManagerGroupInfo(ProductManagerGroupQueryFilter queryCriteria, out int totalCount);

        /// <summary>
        /// 查询所有PM集合
        /// </summary>
        /// <returns></returns>
        List<ProductManagerInfo> QueryAllProductManagerInfoByPMGroupSysNo(int pmGroupSysNo);

        /// <summary>
        /// 查询不在其他PM组的PM集合
        /// </summary>
        /// <returns></returns>
        List<KeyValuePair<string, string>> QueryAllProductManagerInfo(int productManagerGroupInfoSysNo);

        /// <summary>
        /// 得到所有PM
        /// </summary>
        /// <returns></returns>
        List<ProductManagerInfo> QueryAllProductManagerInfo();
    }
}
