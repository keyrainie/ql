using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.IDataAccess;

namespace ECCentral.Service.ExternalSYS.Restful
{
    public partial class ExternalSYSService
    {
        #region 应计报表
        /// <summary>
        /// 应计返利报表查询（周期）
        /// </summary>
        /// <param name="filter">条件集合</param>
        /// <returns>结果集合</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryAccruedByPeriod", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryAccruedByPeriod(AccruedQueryFilter filter)
        {
            return QueryList<AccruedQueryFilter>(filter, ObjectFactory<IAccruedReportDA>.Instance.AccruedByPeriod);
        }

        /// <summary>
        /// 应计返利报表查询（供应商）
        /// </summary>
        /// <param name="filter">条件集合</param>
        /// <returns>结果集合</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryAccruedByVendor", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryAccruedByVendor(AccruedQueryFilter filter)
        {
            return QueryList<AccruedQueryFilter>(filter, ObjectFactory<IAccruedReportDA>.Instance.AccruedByVendor);
        }

        /// <summary>
        /// 应计返利报表查询（合同）
        /// </summary>
        /// <param name="filter">条件集合</param>
        /// <returns>结果集合</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryAccruedByRule", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryAccruedByRule(AccruedQueryFilter filter)
        {
            return QueryList<AccruedQueryFilter>(filter, ObjectFactory<IAccruedReportDA>.Instance.AccruedByRule);
        }

        /// <summary>
        /// 应计返利报表查询（PM）
        /// </summary>
        /// <param name="filter">条件集合</param>
        /// <returns>结果集合</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryAccruedByPM", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryAccruedByPM(AccruedQueryFilter filter)
        {
            return QueryList<AccruedQueryFilter>(filter, ObjectFactory<IAccruedReportDA>.Instance.AccruedByPM);
        }
        #endregion
    }
}
