//************************************************************************
// 用户名				泰隆优选
// 系统名				类别配件管理
// 子系统名		        类别配件管理NoBizQuery查询接口
// 作成者				Tom
// 改版日				2012.5.21
// 改版内容				新建
//************************************************************************


using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface ICategoryAccessoriesDA
    {
        /// <summary>
        /// 根据SysNO获取三级分类配件信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        CategoryAccessory GetCategoryAccessoriesBySysNo(int sysNo);

        /// <summary>
        /// 创建三级分类配件信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        CategoryAccessory CreatetCategoryAccessories(CategoryAccessory entity);

        /// <summary>
        /// 修改三级分类配件信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        CategoryAccessory UpdateCategoryAccessories(CategoryAccessory entity);

        /// <summary>
        /// 在某个三级分类下面是否存在某个配件
        /// </summary>
        /// <param name="accessoriesSysNo"></param>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        bool IsExistCategoryAccessories(int accessoriesSysNo, int categorySysNo);

        /// <summary>
        /// 在某个三级分类下面是否存在某个配件
        /// </summary>
        /// <param name="accessoriesSysNo"></param>
        /// <param name="categorySysNo"></param>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        bool IsExistCategoryAccessories(int accessoriesSysNo, int categorySysNo, int sysNo);
    }
}
