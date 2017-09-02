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
        #region 综合报表
        /// <summary>
        /// EIMS单据查询
        /// </summary>
        /// <param name="filter">条件集合</param>
        /// <returns>结果集合</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryEIMSInvoice", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryEIMSInvoice(EIMSInvoiceQueryFilter filter)
        {
            return QueryList<EIMSInvoiceQueryFilter>(filter, ObjectFactory<IComprehensiveReportDA>.Instance.EIMSInvoiceQuery);
        }

        /// <summary>
        /// 合同与对应单据查询
        /// </summary>
        /// <param name="filter">条件集合</param>
        /// <returns>结果集合</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryUnbilledRuleList", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryUnbilledRuleList(UnbilledRuleListQueryFilter filter)
        {
            return QueryList<UnbilledRuleListQueryFilter>(filter, ObjectFactory<IComprehensiveReportDA>.Instance.UnbilledRuleListQuery);
        }

        /// <summary>
        /// 综合报表查询
        /// </summary>
        /// <param name="filter">条件集合</param>
        /// <returns>结果集合</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryComprehensive", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryComprehensive(EIMSComprehensiveQueryFilter filter)
        {
            return QueryList<EIMSComprehensiveQueryFilter>(filter, ObjectFactory<IComprehensiveReportDA>.Instance.ComprehensiveQuery);
        }
        #endregion
    }
}
