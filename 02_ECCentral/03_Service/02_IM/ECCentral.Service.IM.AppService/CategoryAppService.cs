
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{

    [VersionExport(typeof(CategoryAppService))]
    public class CategoryAppService
    {

        public virtual CategoryInfo AddCategory(CategoryInfo entity)
        {
            return ObjectFactory<CategoryProcessor>.Instance.AddCategory(entity);
        }
        public virtual void UpdateCategory(CategoryRequestApprovalInfo entity)
        {
             ObjectFactory<CategoryProcessor>.Instance.UpdateCategory(entity);
        }
        public virtual List<CategoryInfo> GetAllCategory()
        {
            return ObjectFactory<CategoryProcessor>.Instance.GetAllCategory();
        }
        public virtual CategoryInfo GetCategoryBySysNo(int sysNo)
        {
            return ObjectFactory<CategoryProcessor>.Instance.GetCategoryBySysNo(sysNo);
        }

        public virtual CategoryInfo GetCategory2BySysNo(int c2SysNo)
        {
            return ObjectFactory<CategoryProcessor>.Instance.GetCategory2BySysNo(c2SysNo);
        }

        public virtual CategoryInfo GetCategory3BySysNo(int c3SysNo)
        {
            return ObjectFactory<CategoryProcessor>.Instance.GetCategory3BySysNo(c3SysNo);
        }

        public CategoryInfo GetC1CategoryInfoByProductID(string productID)
        {
            return ObjectFactory<CategoryProcessor>.Instance.GetC1CategoryInfoByProductID(productID);
        }
    }
}
