//************************************************************************
// 用户名				泰隆优选
// 系统名				类别延保管理
// 子系统名		        类别延保管理NoBizQuery查询接口
// 作成者				Kevin
// 改版日				2012.6.5
// 改版内容				新建
//************************************************************************


using ECCentral.BizEntity.IM;
using System.Collections.Generic;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface ICategoryExtendWarrantyDA
    {
        /// <summary>
        /// 根据SysNO获取三级分类延保信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        CategoryExtendWarranty GetCategoryExtendWarrantyBySysNo(int sysNo);

        /// <summary>
        /// 创建三级分类延保信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        CategoryExtendWarranty CreateCategoryExtendWarranty(CategoryExtendWarranty entity);

        /// <summary>
        /// 修改三级分类延保信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        CategoryExtendWarranty UpdateCategoryExtendWarranty(CategoryExtendWarranty entity);

        /// <summary>
        /// Check三级分类延保信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int CheckCategoryExtendWarranty(CategoryExtendWarranty entity);

        /// <summary>
        /// 根据SysNO获取三级分类延保排除品牌信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        CategoryExtendWarrantyDisuseBrand GetCategoryExtendWarrantyDisuseBrandBySysNo(int sysNo);

        /// <summary>
        /// 创建三级分类延保排除品牌信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        CategoryExtendWarrantyDisuseBrand CreateCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrand entity);

        /// <summary>
        /// 修改三级分类延保排除品牌信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        CategoryExtendWarrantyDisuseBrand UpdateCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrand entity);

        /// <summary>
        /// Check三级分类延保排除品牌信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int CheckCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrand entity);

    }
}
