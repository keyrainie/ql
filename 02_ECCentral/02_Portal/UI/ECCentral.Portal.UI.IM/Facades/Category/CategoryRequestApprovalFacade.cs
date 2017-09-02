using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.IM;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.IM.Models.Category;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class CategoryRequestApprovalFacade
    {
         private readonly RestClient restClient;
         private const string GetCategoryRequestApprovalListUrl = "/IMService/CategoryRequestApproval/GetCategoryRequestApprovalList";
         private const string CategoryRequestAuditPassUrl = "/IMService/CategoryRequestApproval/CategoryRequestAuditPass";
         private const string CategoryRequestAuditNotPassUrl = "/IMService/CategoryRequestApproval/CategoryRequestAuditNotPass";
         private const string CategoryRequestCanelUrl = "/IMService/CategoryRequestApproval/CategoryRequestCanel";
      
        /// <summary>
        /// IMService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

        public CategoryRequestApprovalFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public CategoryRequestApprovalFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);

        }
        public void GetCategoryRequestApprovalList(CategoryRequestApprovalQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CategoryRequestApprovalQueryFilter query = new CategoryRequestApprovalQueryFilter();
            query = model.ConvertVM<CategoryRequestApprovalQueryVM, CategoryRequestApprovalQueryFilter>();
            query.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            restClient.QueryDynamicData(GetCategoryRequestApprovalListUrl, query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void CategoryRequestAuditPass(CategoryRequestApprovalVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
             restClient.Update(CategoryRequestAuditPassUrl, ConvertModel(model), callback);
        }
        /// <summary>
        /// 审核不通过
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void CategoryRequestAuditNotPass(CategoryRequestApprovalVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update(CategoryRequestAuditNotPassUrl, ConvertModel(model), callback);
        }
        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void CategoryRequestCanel(CategoryRequestApprovalVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update(CategoryRequestCanelUrl, ConvertModel(model), callback);
        }
        /// <summary>
        /// 转换实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private CategoryRequestApprovalInfo ConvertModel(CategoryRequestApprovalVM model)
        {
            CategoryRequestApprovalInfo info = new CategoryRequestApprovalInfo();
            info.CategorySysNo = model.CategorySysNo;
            info.AuditUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.SysNo = model.SysNo;
            info.CategoryName = model.CategoryName;
            info.OperationType = model.OperationType;
            info.CategoryType = model.CategoryType;
            info.ParentSysNumber = model.ParentSysNumber;
            info.CategoryStatus = model.CategoryStatus;
            info.LanguageCode = CPApplication.Current.LanguageCode;
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.C3Code = model.C3Code;
            return info;
        }
    }
}
