using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IECCategoryQueryDA
    {
        /// <summary>
        /// 获取所有前台一级分类
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <returns>一级分类列表</returns>
        List<ECCategory> GetAllECCategory1(string companyCode, string channelID);

        /// <summary>
        /// 获取所有前台二级分类
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <returns>二级分类列表</returns>
        List<ECCategory> GetAllECCategory2(string companyCode, string channelID);

        /// <summary>
        /// 获取所有前台三级分类
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <returns>三级分类列表</returns>
        List<ECCategory> GetAllECCategory3(string companyCode, string channelID);

        DataTable Query(ECCategoryQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询前台分类可用的父类
        /// </summary>
        /// <param name="sysNo">父级level</param>
        /// <returns>可用父类列表</returns>
        List<ECCategory> GetECCategoryParents(ECCategoryLevel ecCategoryLevel);

        /// <summary>
        /// 查询前台分类的父类系统编号列表
        /// </summary>
        /// <param name="sysNo">前台分类系统编号</param>
        /// <returns>父类系统编号列表</returns>
        List<int> GetECCategoryCurrentParentSysNos(int sysNo);

        /// <summary>
        /// 查询前台分类可用的子类
        /// </summary>
        /// <param name="sysNo">子级level</param>
        /// <returns>可用子类列表</returns>
        List<ECCategory> GetECCategoryChildren(ECCategoryLevel ecCategoryLevel);

        /// <summary>
        /// 查询前台分类的子类系统编号列表
        /// </summary>
        /// <param name="sysNo">前台分类系统编号</param>
        /// <param name="sysNo">前台分类层级系统编号</param>
        /// <returns>子类系统编号列表</returns>
        List<int> GetECCategoryCurrentChildSysNos(int sysNo, int rSysNo);

        List<ECCategory> GetECCategoryTree(ECCategoryQueryFilter filter);

        DataTable QueryECCategoryMapping(ECCategoryMappingQueryFilter filter, out int totalCount);

        #region 管理前台类别关系

        List<CategoryInfo> QueryCategory1(CategoryQueryFilter queryFilter);


        List<CategoryInfo> QueryAllCategory2(CategoryQueryFilter queryFilter);

        List<CategoryInfo> QueryAllCategory3(CategoryQueryFilter queryFilter);
        DataTable QueryECCCategory(ECCManageCategoryQueryFilter queryFilter, out int totalCount);
        #endregion
    }
}
