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
        /// <summary>
        /// 得到所有厂商待审核数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/BrandRequest/GetAllBrandRequest", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetAllBrandRequest(BrandRequestQueryFilter query)
        {
            int TotalCount;
            var datatable = ObjectFactory<IBrandRequestDA>.Instance.GetAllBrandRequest(query, out TotalCount);
            return new QueryResult()
            {
                Data = datatable,
                TotalCount = TotalCount
            };

        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/BrandRequest/AuditBrandRequest", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void AuditBrandRequest(BrandRequestInfo info)
        {
            ObjectFactory<BrandRequestAppService>.Instance.AuditBrandRequest(info);
        }

       /// <summary>
       /// 提交审核 
       /// </summary>
       /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/BrandRequest/InsertBrandRequest", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public void InsertBrandRequest(BrandRequestInfo info)
        {
            ObjectFactory<BrandRequestAppService>.Instance.InsertBrandRequest(info);
        }
    }
}
