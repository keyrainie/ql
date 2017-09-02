using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.QueryFilter.IM;
using System.Data;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        [WebInvoke(UriTemplate = "/CategoryRequestApproval/GetCategoryRequestApprovalList", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetCategoryRequestApprovalList(CategoryRequestApprovalQueryFilter query)
        {
             int totalCount;
            var dataTable = ObjectFactory<ICategoryRequestApprovalDA>.Instance.GetCategoryRequestApprovalList(query, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount=totalCount
            };

        }
    
        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/CategoryRequestApproval/CategoryRequestAuditPass", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public virtual void CategoryRequestAuditPass(CategoryRequestApprovalInfo info)
        {
            ObjectFactory<CategoryRequestApprovalAppService>.Instance.CategoryRequestAuditPass(info);
        }

        /// <summary>
        /// 审核不通过
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/CategoryRequestApproval/CategoryRequestAuditNotPass", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public virtual void CategoryRequestAuditNotPass(CategoryRequestApprovalInfo info)
        {
            ObjectFactory<CategoryRequestApprovalAppService>.Instance.CategoryRequestAuditNotPass(info);
        }
        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/CategoryRequestApproval/CategoryRequestCanel", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void CategoryRequestCanel(CategoryRequestApprovalInfo info)
        {
            ObjectFactory<CategoryRequestApprovalAppService>.Instance.CategoryRequestCanel(info);
        }
    }
}
