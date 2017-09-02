using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.IDataAccess
{
    public interface IConsignSettlementRulesDA
    {
        /// <summary>
        /// 根据Code获取规则信息
        /// </summary>
        /// <param name="settleRulesCode"></param>
        /// <returns></returns>
        ConsignSettlementRulesInfo GetConsignSettleRuleByCode(string settleRulesCode);

        /// <summary>
        /// 根据操作类型，更新代销商品规则
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        int UpdateConsignSettlementRulesInfo(ConsignSettlementRulesInfo entity, ConsignSettleRuleActionType actionType);

        /// <summary>
        /// 新建代销商品规则
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int CreateConsignSettlementRule(ConsignSettlementRulesInfo entity);

        /// <summary>
        /// 规则时间点重叠检查
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ConsignSettlementRulesInfo GetSettleRuleByItemVender(ConsignSettlementRulesInfo entity);

        /// <summary>
        /// 验证商品的有效性
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        int CheckConsignProduct(int productSysNo);

        /// <summary>
        /// 根据Id列表获取规则集合
        /// </summary>
        /// <param name="sysNos">Id列表</param>
        /// <returns>规则集合</returns>
        List<ConsignSettlementRulesInfo> GetSettleRuleListBySysNos(string sysNos);
    }
}
