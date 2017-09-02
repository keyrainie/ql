//************************************************************************
// 用户名				泰隆优选
// 系统名				分类属性管理
// 子系统名		        分类属性管理业务实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{

    [VersionExport(typeof(CategoryAccessoryAppService))]
    public class CategoryAccessoryAppService
    {
        /// <summary>
        /// 根据SysNO获取分类属性信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public CategoryAccessory GetCategoryAccessoryBySysNo(int sysNo)
        {
            var result = ObjectFactory<CategoryAccessoriesProcessor>.Instance.GetCategoryAccessoryBySysNo(sysNo);
            return result;
        }


        /// <summary>
        /// 创建分类属性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public CategoryAccessory CreatetCategoryAccessory(CategoryAccessory entity)
        {
            var result = ObjectFactory<CategoryAccessoriesProcessor>.Instance.CreatetCategoryAccessory(entity);
            return result;
        }


        /// <summary>
        /// 修改分类属性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public CategoryAccessory UpdateCategoryAccessory(CategoryAccessory entity)
        {
            var result = ObjectFactory<CategoryAccessoriesProcessor>.Instance.UpdateCategoryAccessory(entity);
            return result;
        }

    }
}
