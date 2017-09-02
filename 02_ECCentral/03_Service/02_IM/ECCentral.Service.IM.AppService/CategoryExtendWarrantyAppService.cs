//************************************************************************
// 用户名				泰隆优选
// 系统名				分类延保管理
// 子系统名		        分类延保管理业务实现
// 作成者				Kevin
// 改版日				2012.6.4
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.IM.BizProcessor.IMAOP;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{

    [VersionExport(typeof(CategoryExtendWarrantyAppService))]
    public class CategoryExtendWarrantyAppService
    {
        /// <summary>
        /// 根据SysNO获取分类延保信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public CategoryExtendWarranty GetCategoryExtendWarrantyBySysNo(int sysNo)
        {
            var result = ObjectFactory<CategoryExtendWarrantyProcessor>.Instance.GetCategoryExtendWarrantyBySysNo(sysNo);
            return result;
        }


        /// <summary>
        /// 创建分类延保
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
         [ProductInfoChange]
        public CategoryExtendWarranty CreatetCategoryExtendWarranty(CategoryExtendWarranty entity)
        {
            var result = ObjectFactory<CategoryExtendWarrantyProcessor>.Instance.CreatetCategoryExtendWarranty(entity);
            return result;
        }


        /// <summary>
        /// 修改分类延保
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
         [ProductInfoChange]
        public CategoryExtendWarranty UpdateCategoryExtendWarranty(CategoryExtendWarranty entity)
        {
            var result = ObjectFactory<CategoryExtendWarrantyProcessor>.Instance.UpdateCategoryExtendWarranty(entity);
            return result;
        }

        /// <summary>
        /// 根据SysNO获取分类延保排出品牌信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public CategoryExtendWarrantyDisuseBrand GetCategoryExtendWarrantyDisuseBrandBySysNo(int sysNo)
        {
            var result = ObjectFactory<CategoryExtendWarrantyProcessor>.Instance.GetCategoryExtendWarrantyDisuseBrandBySysNo(sysNo);
            return result;
        }


        /// <summary>
        /// 创建分类延保排出品牌
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
         [ProductInfoChange]
        public CategoryExtendWarrantyDisuseBrand CreatetCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrand entity)
        {
            var result = ObjectFactory<CategoryExtendWarrantyProcessor>.Instance.CreatetCategoryExtendWarrantyDisuseBrand(entity);
            return result;
        }


        /// <summary>
        /// 修改分类延保排出品牌
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [ProductInfoChange]
        public CategoryExtendWarrantyDisuseBrand UpdateCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrand entity)
        {
            var result = ObjectFactory<CategoryExtendWarrantyProcessor>.Instance.UpdateCategoryExtendWarrantyDisuseBrand(entity);
            return result;
        }

    }
}
