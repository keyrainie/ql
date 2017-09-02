using System.ServiceModel.Web;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.ExternalSYS.Restful
{
    public partial class ExternalSYSService
    {
        /// <summary>
        /// 查询系统日志列表
        /// </summary>
        /// <param name="filter">过滤条件集合</param>
        /// <returns>查询结果</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryVendorSystemLog", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVendorSystemLog(VendorSystemQueryFilter filter)
        {
            return QueryList<VendorSystemQueryFilter>(filter, ObjectFactory<IVendorSystemInfoDA>.Instance.LogQuery);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="log"></param>
        [WebInvoke(UriTemplate = "/ExternalSYS/WriteLog", Method = "POST")]
        public void WriteLog(VendorPortalLog log)
        {
            ObjectFactory<VendorPortalManagerAppService>.Instance.WriteLog(log);
        }
    }
}
