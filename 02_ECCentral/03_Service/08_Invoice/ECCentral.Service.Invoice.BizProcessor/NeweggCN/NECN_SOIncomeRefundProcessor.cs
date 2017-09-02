using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(SOIncomeRefundProcessor), Version = "2.0.0.0")]
    public class NECN_SOIncomeRefundProcessor : SOIncomeRefundProcessor
    {
        /// <summary>
        /// 第三方平台订单审核RMA物流拒收
        /// </summary>
        /// <param name="entity"></param>
        protected override void AuditAutoRMAForThirdSO(BizEntity.Invoice.SOIncomeRefundInfo entity)
        {
            base.AuditAutoRMAForThirdSO(entity);

            //中蛋逻辑：如果是淘宝订单，则将状态更新为已审核状态。
            var soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(entity.SOSysNo.Value);
            if (soBaseInfo != null && soBaseInfo.SpecialSOType == ECCentral.BizEntity.SO.SpecialSOType.TaoBao)
            {
                entity.Status = RefundStatus.Audit;
            }
        }

        protected override void PreCheckForCreate(SOIncomeRefundInfo entity)
        {
            base.PreCheckForCreate(entity);

            //如果是“现金退款”，则调用SO服务验证订单的支付类型是不是可以选择“现金退款”
            if (entity.RefundPayType == RefundPayType.CashRefund)
            {
                //只有订单为货到"付款(OZZO奥硕物流)","现金支付","POS机刷卡"，"宅急送(货到付款)"才可以选择现金退款
                SOBaseInfo soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(entity.SOSysNo.Value);
                if (!CheckPayTypeCanCashRefund(soBaseInfo.PayTypeSysNo.Value))
                {
                    ThrowBizException("SOIncomeRefund_CashRefundNotAllowed");
                }
            }
        }

        protected override void PreCheckForUpdate(SOIncomeRefundInfo entity)
        {
            base.PreCheckForUpdate(entity);

            //如果是“现金退款”，则调用SO服务验证订单的支付类型是不是可以选择“现金退款”
            if (entity.RefundPayType == RefundPayType.CashRefund)
            {
                //只有订单为货到"付款(OZZO奥硕物流)","现金支付","POS机刷卡"，"宅急送(货到付款)"才可以选择现金退款
                SOBaseInfo soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(entity.SOSysNo.Value);
                if (!CheckPayTypeCanCashRefund(soBaseInfo.PayTypeSysNo.Value))
                {
                    ThrowBizException("SOIncomeRefund_CashRefundNotAllowed");
                }
            }
        }
    }
}