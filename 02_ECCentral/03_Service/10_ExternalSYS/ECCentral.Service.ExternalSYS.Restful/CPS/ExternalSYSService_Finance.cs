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
        [WebInvoke(UriTemplate = "/Finance/GetAllFinance", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetAllFinance(FinanceQueryFilter query)
        {
         
            int totalCount;
            var dataTable = ObjectFactory<IFinanceDA>.Instance.GetAllFinancee(query, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 更新确认结算金额
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/Finance/UpdateCommisonConfirmAmt", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateCommisonConfirmAmt(FinanceInfo info)
        {
            ObjectFactory<FinanceAppService>.Instance.UpdateCommisonConfirmAmt(info);
        }
    }
}
