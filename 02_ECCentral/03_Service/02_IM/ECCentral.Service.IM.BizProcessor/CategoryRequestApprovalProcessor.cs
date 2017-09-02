using System;
using System.Collections.Generic;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor.IMAOP;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using System.Transactions;
using ECCentral.Service.EventMessage.IM;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(CategoryRequestApprovalProcessor))]
    public class CategoryRequestApprovalProcessor
    {
        private readonly ICategoryRequestApprovalDA categoryRequestApprovalDA = ObjectFactory<ICategoryRequestApprovalDA>.Instance;
        private readonly CategoryProcessor categoryBP = ObjectFactory<CategoryProcessor>.Instance;
        private readonly ICategoryDA categoryDA = ObjectFactory<ICategoryDA>.Instance;

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="info"></param>
         [ProductInfoChange]
        public virtual void CategoryRequestAuditPass(CategoryRequestApprovalInfo info)
        {
             // bug 2834
            //if (categoryRequestApprovalDA.CheckCategoryUser(info.SysNo))
            //{
            //    throw new BizException("审核人和创建人不能是同一个人!");
            //}
            info.Status =CategoryAuditStatus.CategoryAuditPass;
            using (TransactionScope scope = new TransactionScope())
            {
                //审核通过时要同步Category
                SynchronousCategory(info);
                categoryRequestApprovalDA.ActiveCategoryRequest(info);

                //审核通过之后发送消息
                EventPublisher.Publish<CategoryAuditMessage>(new CategoryAuditMessage()
                {
                    AuditUserSysNo = ServiceContext.Current.UserSysNo,
                    RequestSysNo = info != null && info.SysNo.HasValue ? info.SysNo.Value : 0,
                    CategorySysNo = info != null ? info.CategorySysNo : 0
                });

                scope.Complete();
            }    
        }

        /// <summary>
        /// 审核不通过
        /// </summary>
        /// <param name="info"></param>
         [ProductInfoChange]
        public virtual void CategoryRequestAuditNotPass(CategoryRequestApprovalInfo info)
        {
            //if (categoryRequestApprovalDA.CheckCategoryUser(info.SysNo))
            //{
            //    throw new BizException("审核人和创建人不能是同一个人!");
            //}
            info.Status = CategoryAuditStatus.CategoryAuditNotPass;

            using (TransactionScope scope = new TransactionScope())
            {
                categoryRequestApprovalDA.ActiveCategoryRequest(info);

                //审核不通过之后发送消息
                EventPublisher.Publish<CategoryRejectMessage>(new CategoryRejectMessage()
                {
                    RejectUserSysNo = ServiceContext.Current.UserSysNo,
                    RequestSysNo = info != null && info.SysNo.HasValue ? info.SysNo.Value : 0,
                    CategorySysNo = info != null ? info.CategorySysNo : 0
                });

                scope.Complete();
            }
         }
        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="info"></param>
         [ProductInfoChange]
        public virtual void CategoryRequestCanel(CategoryRequestApprovalInfo info)
        {
            // if (categoryRequestApprovalDA.CheckCategoryUser(info.SysNo))
            //{
            //    throw new BizException("审核人和创建人不能是同一个人!");
            //}
            info.Status = CategoryAuditStatus.CategoryAuditCanel;

            using (TransactionScope scope = new TransactionScope())
            {
                categoryRequestApprovalDA.ActiveCategoryRequest(info);

                //取消审核之后发送消息
                EventPublisher.Publish<CategoryCancelMessage>(new CategoryCancelMessage()
                {
                    CancelUserSysNo = ServiceContext.Current.UserSysNo,
                    RequestSysNo = info != null && info.SysNo.HasValue ? info.SysNo.Value : 0,
                    CategorySysNo = info != null ? info.CategorySysNo : 0
                });

                scope.Complete();
            }
        }
      

        /// <summary>
        /// 同步Category
        /// </summary>
        /// <param name="info"></param>
         private void SynchronousCategory(CategoryRequestApprovalInfo info)
        {
            if (info.OperationType == OperationType.Create)
            {
                int id = categoryDA.CreateCategorySequence();
                CategoryInfo category = new CategoryInfo()
                {
                    ParentSysNumber = info.ParentSysNumber,
                    SysNo = id,
                    CategoryName = new LanguageContent() { Content = info.CategoryName },
                    Status = info.CategoryStatus,
                    OperationType = info.OperationType,
                    CategoryID = Convert.ToString(id),
                    CompanyCode = info.CompanyCode,
                    LanguageCode = info.LanguageCode,
                    C3Code = info.C3Code
                };
                info.CategorySysNo = id;

                categoryBP.CeateCategoryByType(info.CategoryType, category);
            }
            if (info.OperationType == OperationType.Update)
            {
                CategoryInfo category = new CategoryInfo()
                {
                    ParentSysNumber = info.ParentSysNumber,
                    SysNo = info.CategorySysNo,
                    CategoryName = new LanguageContent() { Content = info.CategoryName },
                    Status = info.CategoryStatus,
                    OperationType = info.OperationType,
                    CompanyCode = info.CompanyCode,
                    LanguageCode = info.LanguageCode,
                    C3Code = info.C3Code
                };
                categoryBP.UpdateCategoryByType(info.CategoryType, category);
            }

        }
    
        [ProductInfoChange]
        public void CreateCategoryRequest(CategoryRequestApprovalInfo info)
        {
            //判重
            if (!categoryRequestApprovalDA.IsExistsCategoryRequest(info))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Category", "CategoryAuditing"));
            }

            //先创建Category的SysNo
            if (info.CategorySysNo < 0)
            {
                info.CategorySysNo = categoryDA.CreateCategorySequence();
            }

            using (TransactionScope scope = new TransactionScope())
            {
                categoryRequestApprovalDA.CreateCategoryRequest(info);

                //提交审核之后发送消息
                EventPublisher.Publish<CategoryAuditSubmitMessage>(new CategoryAuditSubmitMessage()
                {
                    SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                    RequestSysNo = info != null && info.SysNo.HasValue ? info.SysNo.Value : 0,
                    CategorySysNo = info != null ? info.CategorySysNo : 0
                });

                scope.Complete();
            }
        }

        public void UpdateCategoryRequest(CategoryRequestApprovalInfo info)
        {
            categoryRequestApprovalDA.UpdateCategoryRequest(info);
        }

    }
}
