using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.WCF;
using System.ServiceModel.Web;
using ECCentral.QueryFilter.PO;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.Service.PO.AppService;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {
        /// <summary>
        /// 代销商品规则设置查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlementRules/QueryConsignSettleRulesList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryConsignSettleRulesList(SettleRuleQueryFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IConsignSettleRulesQueryDA>.Instance.QueryConsignSettleRules(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        /// <summary>
        /// 新建规则
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlementRules/CreateConsignSettleRule", Method = "PUT")]
        public ConsignSettlementRulesInfo CreateConsignSettleRule(ConsignSettlementRulesInfo info)
        {
            return ObjectFactory<ConsignSettlementRulesAppService>.Instance.CreateConsignRule(info);
        }

        /// <summary>
        /// 更新规则
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlementRules/UpdateConsignSettleRule", Method = "PUT")]
        public ConsignSettlementRulesInfo UpdateConsignSettleRule(ConsignSettlementRulesInfo info)
        {
            return ObjectFactory<ConsignSettlementRulesAppService>.Instance.UpdateConsignRule(info);
        }

        /// <summary>
        ///  审核规则
        /// </summary>
        /// <param name="settleRuleCode"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlementRules/AuditConsignSettleRule", Method = "PUT")]
        public ConsignSettlementRulesInfo AuditConsignSettleRule(ConsignSettlementRulesInfo info)
        {
            return ObjectFactory<ConsignSettlementRulesAppService>.Instance.AuditConsignRule(info);
        }

        /// <summary>
        /// 终止规则
        /// </summary>
        /// <param name="settleRuleCode"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlementRules/StopConsignSettleRule", Method = "PUT")]
        public ConsignSettlementRulesInfo StopConsignSettleRule(string settleRuleCode)
        {
            return ObjectFactory<ConsignSettlementRulesAppService>.Instance.StopConsignRule(settleRuleCode);
        }

        /// <summary>
        /// 作废规则
        /// </summary>
        /// <param name="settleRuleCode"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlementRules/AbandonConsignSettleRule", Method = "PUT")]
        public ConsignSettlementRulesInfo AbandonConsignSettleRule(string settleRuleCode)
        {
            return ObjectFactory<ConsignSettlementRulesAppService>.Instance.AbandonConsignRule(settleRuleCode);
        }
    }
}
