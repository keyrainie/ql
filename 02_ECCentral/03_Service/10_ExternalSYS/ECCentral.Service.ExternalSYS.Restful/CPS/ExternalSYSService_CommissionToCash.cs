using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.ExternalSYS.AppService;
namespace ECCentral.Service.ExternalSYS.Restful
{
    public partial class ExternalSYSService
    {
        /// <summary>
        /// 得到所有结算单
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommissionToCash/GetCommissionToCashByQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetCommissionToCashByQuery(CommissionToCashQueryFilter query)
        {

            int totalCount;
            var dataTable = ObjectFactory<ICommissionToCashDA>.Instance.GetCommissionToCashByQuery(query, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/CommissionToCash/AuditCommisonToCash", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void AuditCommisonToCash(CommissionToCashInfo info)
        {
            ObjectFactory<CommissionToCashAppService>.Instance.AuditCommisonToCash(info);
        }
         /// <summary>
        /// 更新实际支付金额
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/CommissionToCash/UpdateCommissionToCashPayAmt", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateCommissionToCashPayAmt(CommissionToCashInfo info)
        {
            ObjectFactory<CommissionToCashAppService>.Instance.UpdateCommissionToCashPayAmt(info);
        }
         /// <summary>
        /// 确认支付
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/CommissionToCash/ConfirmCommisonToCash", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void ConfirmCommisonToCash(CommissionToCashInfo info)
        {
            ObjectFactory<CommissionToCashAppService>.Instance.ConfirmCommisonToCash(info);
        }
    }
}
