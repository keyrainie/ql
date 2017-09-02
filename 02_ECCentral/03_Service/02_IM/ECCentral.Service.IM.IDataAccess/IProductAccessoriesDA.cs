using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
   public interface IProductAccessoriesDA
    {
        #region "配件查询操作"
        
       /// <summary>
       /// 根据query得到配件信息 
       /// </summary>
       /// <param name="query"></param>
       /// <param name="?"></param>
       /// <returns></returns>
       DataTable GetProductAccessoriesByQuery(ProductAccessoriesQueryFilter query, out int totalCount);


       /// <summary>
       /// 新建查询功能
       /// </summary>
       /// <param name="info"></param>
       void CreateAccessoriesQueryMaster(ProductAccessoriesInfo info);

       /// <summary>
       /// 修改查询功能
       /// </summary>
       /// <param name="info"></param>
       void UpdateAccessoriesQueryMaster(ProductAccessoriesInfo info);
       
       /// <summary>
       /// 是否存在该查询功能
       /// </summary>
       /// <param name="info"></param>
       /// <returns> true 存在</returns>
       bool IsExistsAccessoriesQuery(ProductAccessoriesInfo info);

        #endregion

       #region "查询条件操作"

      
       /// <summary>
       /// 得到某个查询功能的所有查询条件
       /// </summary>
       /// <param name="SysNo"></param>
       /// <returns></returns>
       DataTable GetAccessoriesQueryConditionBySysNo(int SysNo);

       /// <summary>
       /// 新建查询条件
       /// </summary>
       /// <param name="Info"></param>
       void CreateAccessoriesQueryCondition(ProductAccessoriesQueryConditionInfo Info);

       /// <summary>
       /// 修改查询条件
       /// </summary>
       /// <param name="Info"></param>
       void UpdateAccessoriesQueryCondition(ProductAccessoriesQueryConditionInfo Info);

       /// <summary>
       /// 删除查询条件
       /// </summary>
       /// <param name="SysNo"></param>
       void DeleteAccessoriesQueryCondition(int SysNo);
       
       /// <summary>
       /// 检查条件是否已存在
       /// </summary>
       /// <param name="info"></param>
       /// <returns>true 存在</returns>
       bool IsExistsAccessoriesQueryCondition(ProductAccessoriesQueryConditionInfo info);
       #endregion

        #region "条件选项值操作"

       /// <summary>
       /// 查询某一个配件查询的条件值
       /// </summary>
       /// <param name="Query"></param>
       /// <returns></returns>
       DataTable GetProductAccessoriesConditionValueByQuery(ProductAccessoriesConditionValueQueryFilter Query, out int totalCount);

       /// <summary>
       /// 得到该条件的父节点的所有选项值
       /// </summary>
       /// <param name="SysNo"></param>
       /// <returns></returns>
       DataTable GetProductAccessoriesConditionValueByConditionSysNo(ProductAccessoriesConditionValueQueryFilter query);


       /// <summary>
       /// 新建
       /// </summary>
       /// <param name="Info"></param>
       void CreateProductAccessoriesQueryConditionValue(ProductAccessoriesQueryConditionValueInfo Info);

       /// <summary>
       /// 更新
       /// </summary>
       /// <param name="Info"></param>
       void UpdateProductAccessoriesQueryConditionValue(ProductAccessoriesQueryConditionValueInfo Info);

       /// <summary>
       /// 删除
       /// </summary>
       /// <param name="SysNo"></param>
       void DeleteProductAccessoriesQueryConditionValue(int SysNo);

       /// <summary>
       /// 检查选项值是否存在
       /// </summary>
       /// <param name="info"></param>
       /// <returns></returns>
       bool IsExistsAccessoriesQueryConditionValue(ProductAccessoriesQueryConditionValueInfo info);

      #endregion

        #region "查询效果操作"
       /// <summary>
       /// 得到某个条件的所有选项值
       /// </summary>
       /// <param name="query"></param>
       /// <returns></returns>
       DataTable GetConditionValueByQuery(ProductAccessoriesConditionValueQueryFilter query);

       /// <summary>
       /// 得到商品和条件选项值bing的信息
       /// </summary>
       /// <param name="query"></param>
       /// <param name="totalCoutn"></param>
       /// <returns></returns>
       DataTable QueryAccessoriesQueryConditionBind(ProductAccessoriesQueryConditionPreViewQueryFilter query, out int totalCount);

       /// <summary>
       /// 删除bing
       /// </summary>
       /// <param name="info"></param>
       void DeleteAccessoriesQueryConditionBind(ProductAccessoriesQueryConditionPreViewInfo info);


        #endregion

        #region "导出"
       /// <summary>
       /// 导出
       /// </summary>
       /// <param name="query"></param>
       /// <returns></returns>
       DataTable GetAccessoriesQueryExcelOutput(ProductAccessoriesConditionValueQueryFilter query);

        #endregion

    }
}
