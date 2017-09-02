//************************************************************************
// 用户名				泰隆优选
// 系统名				三级分类指标管理
// 子系统名		        三级分类指标管理业务实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;
using System.Collections.Generic;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(CategorySettingAppService))]
    public class CategorySettingAppService
    {

        /// <summary>
        /// 根据三级分类获取三级指标
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public CategorySetting GetCategorySettingBySysNo(int SysNO)
        {
            var result = ObjectFactory<CategorySettingProcessor>.Instance.GetCategorySettingBySysNo(SysNO);
            return result;
        }

        /// <summary>
        /// 保存基本指标信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        public CategoryBasic UpdateCategoryBasic(CategoryBasic categoryBasicInfo)
        {
            var result = ObjectFactory<CategorySettingProcessor>.Instance.UpdateCategoryBasic(categoryBasicInfo);
            return result;
        }
        public void UpdateCategory2Basic(CategoryBasic categoryBasicInfo)
        {
            ObjectFactory<CategorySettingProcessor>.Instance.UpdateCategory2Basic(categoryBasicInfo);
        }

        /// <summary>
        /// 保存RMA信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        public CategoryRMA UpdateCategoryRMA(CategoryRMA categoryBasicInfo)
        {
            var result = ObjectFactory<CategorySettingProcessor>.Instance.UpdateCategoryRMA(categoryBasicInfo);
            return result;
        }

        /// <summary>
        /// 保存毛利率信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        public CategoryMinMargin UpdateCategoryMinMargin(CategoryMinMargin categoryBasicInfo)
        {
            var result = ObjectFactory<CategorySettingProcessor>.Instance.UpdateCategoryMinMargin(categoryBasicInfo);
            return result;
        }

        /// <summary>
        /// 类别2更新毛利率
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        public void UpdateCategory2MinMargin(CategoryMinMargin categoryBasicInfo)
        {
            ObjectFactory<CategorySettingProcessor>.Instance.UpdateCategory2MinMargin(categoryBasicInfo);
        }
        /// <summary>
        /// 批量保存三级类别
        /// </summary>
        /// <param name="categoryMinMarginList"></param>
        public void UpdateCategory3MinMarginBat(List<CategoryMinMargin> categoryMinMarginList)
        {
            ObjectFactory<CategorySettingProcessor>.Instance.UpdateCategory3MinMarginBat(categoryMinMarginList);
         }
        public void UpdateCategory2MinMarginBat(List<CategoryMinMargin> categoryMinMarginList)
        {
            ObjectFactory<CategorySettingProcessor>.Instance.UpdateCategory2MinMarginBat(categoryMinMarginList);
        }
        public  void UpdateCategoryProductMinCommission(CategoryBasic categoryBasicInfo)
        {
            ObjectFactory<CategorySettingProcessor>.Instance.UpdateCategoryProductMinCommission(categoryBasicInfo);
        }
        public void UpdateCategory2ProductMinCommission(List<CategoryBasic> categoryBasicList)
        {
            ObjectFactory<CategorySettingProcessor>.Instance.UpdateCategory2ProductMinCommission(categoryBasicList);
        }
        public CategorySetting GetCategorySettingByCategory2SysNo(int SysNo)
        {
            return ObjectFactory<CategorySettingProcessor>.Instance.GetCategorySettingByCategory2SysNo(SysNo);
        }
    }


}
