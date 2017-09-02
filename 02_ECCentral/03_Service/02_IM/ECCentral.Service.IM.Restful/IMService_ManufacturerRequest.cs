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
        [WebInvoke(UriTemplate = "/ManufacturerRequest/GetAllManufacturerRequest", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetAllManufacturerRequest(ManufacturerRequestQueryFilter query)
        {
            int TotalCount;
            var datatable = ObjectFactory<IManufacturerRequestDA>.Instance.GetAllManufacturerRequest(query,out TotalCount);
            return new QueryResult() 
            {
                Data=datatable,
                TotalCount=TotalCount
            };

        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/ManufacturerRequest/AuditManufacturerRequest", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void AuditManufacturerRequest(ManufacturerRequestInfo info)
        {
            ObjectFactory<ManufacturerRequestAppService>.Instance.AuditManufacturerRequest(info);
        }
         /// <summary>
         /// 生产商提交审核
         /// </summary>
         /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/ManufacturerRequest/InsertManufacturerRequest", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public void InsertManufacturerRequest(ManufacturerRequestInfo info)
        {
            ObjectFactory<ManufacturerRequestAppService>.Instance.InsertManufacturerRequest(info);
        }
    }
}
