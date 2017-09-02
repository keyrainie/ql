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
        #region 收款报表
        /// <summary>
        /// 年度收款报查询
        /// </summary>
        /// <param name="filter">条件集合</param>
        /// <returns>结果集合</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryReceiveByYear", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryReceiveByYear(ReceivedReportQueryFilter filter)
        {
            return QueryList<ReceivedReportQueryFilter>(filter, ObjectFactory<IReceivedReportDA>.Instance.ReceiveByYearQuery);
        }

        /// <summary>
        /// 供应商对账单查询
        /// </summary>
        /// <param name="filter">条件集合</param>
        /// <returns>结果集合</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryReceiveByVendor", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryReceiveByVendor(ReceivedReportQueryFilter filter)
        {
            return QueryList<ReceivedReportQueryFilter>(filter, ObjectFactory<IReceivedReportDA>.Instance.ReceiveByVendorQuery);
        }

        /// <summary>
        /// 对应账单（单据）查询
        /// </summary>
        /// <param name="filter">条件集合</param>
        /// <returns>结果集合</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryARReceive", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryARReceive(ReceivedReportQueryFilter filter)
        {
            return QueryList<ReceivedReportQueryFilter>(filter, ObjectFactory<IReceivedReportDA>.Instance.ARReceiveQuery);
        }

        /// <summary>
        /// 账单明细查询
        /// </summary>
        /// <param name="filter">条件集合</param>
        /// <returns>结果集合</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryARReceiveDetials", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryARReceiveDetials(ReceivedReportQueryFilter filter)
        {
            return QueryList<ReceivedReportQueryFilter>(filter, ObjectFactory<IReceivedReportDA>.Instance.ARReceiveDetialsQuery);
        }
        #endregion
    }
}
