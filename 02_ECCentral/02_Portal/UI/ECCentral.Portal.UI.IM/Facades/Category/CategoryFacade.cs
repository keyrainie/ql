using System;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class CategoryFacade
    {
        #region 构造函数和字段
        private readonly RestClient _restClient;
        const string CreateRelativeUrl = "/IMService/Category/CreateCategory";
        const string UPdateRelativeUrl = "/IMService/Category/UpdateCategory";
        const string GetRelativeUrl = "/IMService/Category/GetCategoryBySysNo";
        const string CreateCategoryRequestUrl = "/IMService/Category/CreateCategoryRequest";
        const string UpdateCategoryRequestUrl = "/IMService/Category/UpdateCategoryRequest";

        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");                
            }
        }

        public CategoryFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public CategoryFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        #region 函数
        /// <summary>
        /// 转换分类视图和分类实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private CategoryInfo CovertVMtoEntity(CategoryVM data)
        {
            CategoryInfo msg = data.ConvertVM<CategoryVM, CategoryInfo>((v, t) =>
                                                                            {
                                                                                t.CategoryName = new LanguageContent(v.CategoryName);
                                                                            });
            msg.SysNo = data.SysNo;
            return msg;
        }

        /// <summary>
        /// 创建分类
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateCategory(CategoryVM data, EventHandler<RestClientEventArgs<CategoryInfo>> callback)
        {
            _restClient.Create(CreateRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateCategory(CategoryRequestApprovalVM model, EventHandler<RestClientEventArgs<CategoryInfo>> callback)
        {
            CategoryRequestApprovalInfo info = new CategoryRequestApprovalInfo()
            {
                SysNo = model.SysNo,
                CategoryStatus = model.CategoryStatus,
                CategoryName = model.CategoryName,
                CategoryType = model.CategoryType,
                ParentSysNumber = model.ParentSysNumber,
                OperationType = model.OperationType,
                Reasons = model.Reansons,
                CategorySysNo=model.CategorySysNo
            };
            if (model.CategoryType == CategoryType.CategoryType3)
            {
                info.C3Code = model.C3Code;
            }
            _restClient.Update(UPdateRelativeUrl, info, callback);
        }

        /// <summary>
        /// 获取分类
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetCategoryBySysNo(int sysNo, EventHandler<RestClientEventArgs<CategoryInfo>> callback)
        {
            _restClient.Query(GetRelativeUrl, sysNo, callback);
        }

        public void CreateCategoryRequest(CategoryRequestApprovalVM model, EventHandler<RestClientEventArgs<CategoryInfo>> callback)
        {

            CategoryRequestApprovalInfo info = new CategoryRequestApprovalInfo() 
            {
                CategoryStatus=model.CategoryStatus,
                CategoryName=model.CategoryName,
                CategoryType=model.CategoryType,
                ParentSysNumber=model.ParentSysNumber,
                OperationType=model.OperationType,
                Reasons=model.Reansons,
                CategorySysNo=model.CategorySysNo,
                Status=CategoryAuditStatus.CategoryWaitAudit,
               CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = CPApplication.Current.LanguageCode,
                C3Code=model.C3Code
            };

            if (model.CategoryType == CategoryType.CategoryType3)
            {
                info.C3Code = model.C3Code;
            }
            _restClient.Create(CreateCategoryRequestUrl, info, callback);
        }

        public void UpdateCategoryRequest(CategoryRequestApprovalVM model, EventHandler<RestClientEventArgs<CategoryInfo>> callback)
        {
            CategoryRequestApprovalInfo info = new CategoryRequestApprovalInfo()
            {
                SysNo=model.SysNo,
                CategoryStatus = model.CategoryStatus,
                CategoryName = model.CategoryName,
                CategoryType = model.CategoryType,
                ParentSysNumber = model.ParentSysNumber,
                OperationType = model.OperationType,
                Reasons = model.Reansons,
            };

            if (model.CategoryType == CategoryType.CategoryType3)
            {
                info.C3Code = model.C3Code;
            }
            _restClient.Update(UpdateCategoryRequestUrl, info, callback);
        }

        #endregion
    }
}
