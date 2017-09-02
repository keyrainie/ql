using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
     [VersionExport(typeof(CategoryRequestApprovalAppService))]
   public  class CategoryRequestApprovalAppService
    {
        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="info"></param>
      
        public virtual void CategoryRequestAuditPass(CategoryRequestApprovalInfo info)
        {
            ObjectFactory<CategoryRequestApprovalProcessor>.Instance.CategoryRequestAuditPass(info);
        }

        /// <summary>
        /// 审核不通过
        /// </summary>
        /// <param name="info"></param>
       
        public virtual void CategoryRequestAuditNotPass(CategoryRequestApprovalInfo info)
        {
            ObjectFactory<CategoryRequestApprovalProcessor>.Instance.CategoryRequestAuditNotPass(info);
        }
        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="info"></param>
        public void CategoryRequestCanel(CategoryRequestApprovalInfo info)
        {
            ObjectFactory<CategoryRequestApprovalProcessor>.Instance.CategoryRequestCanel(info);
        }
         /// <summary>
         /// 创建
         /// </summary>
         /// <param name="info"></param>
        public void CreateCategoryRequest(CategoryRequestApprovalInfo info)
        {

            ObjectFactory<CategoryRequestApprovalProcessor>.Instance.CreateCategoryRequest(info);
        }
         /// <summary>
         /// 更新
         /// </summary>
         /// <param name="info"></param>
        public void UpdateCategoryRequest(CategoryRequestApprovalInfo info)
        {
            ObjectFactory<CategoryRequestApprovalProcessor>.Instance.UpdateCategoryRequest(info);
        }
    }
}
