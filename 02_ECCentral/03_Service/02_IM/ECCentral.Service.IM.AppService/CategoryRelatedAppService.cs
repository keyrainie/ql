using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
     [VersionExport(typeof(CategoryRelatedAppService))]
     public  class CategoryRelatedAppService
    {
        /// <summary>
        /// 创建新的相关类别
        /// </summary>
        /// <param name="info"></param>
        public virtual void CreateCategoryRelated(CategoryRelatedInfo info)
        {
            ObjectFactory<CategoryRelatedProcessor>.Instance.CreateCategoryRelated(info);
        }

        /// <summary>
        /// 批量删除相关类别
        /// </summary>
        /// <param name="sysnos"></param>
        public virtual void DeleteCategoryRelated(List<string> sysnos)
        {
            ObjectFactory<CategoryRelatedProcessor>.Instance.DeleteCategoryRelated(sysnos);
        }
    }
}
