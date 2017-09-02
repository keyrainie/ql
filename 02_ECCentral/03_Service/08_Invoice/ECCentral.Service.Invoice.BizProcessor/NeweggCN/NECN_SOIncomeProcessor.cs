using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(SOIncomeProcessor), Version = "2.0.0.0")]
    public class NECN_SOIncomeProcessor : SOIncomeProcessor
    {
        protected override void PreCheckForCancelConfirm(BizEntity.Invoice.SOIncomeInfo entity)
        {
            base.PreCheckForCancelConfirm(entity);

            if (entity.OrderType == SOIncomeOrderType.RO)
            {
                var refund = ExternalDomainBroker.GetRefundBySysNo(entity.OrderSysNo.Value);
                var soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(refund.SOSysNo.Value);

                string taobaoAcc = AppSettingManager.GetSetting("Invoice", "TaobaoAccount");
                if (soBaseInfo.SpecialSOType == ECCentral.BizEntity.SO.SpecialSOType.TaoBao && entity.ConfirmUserSysNo == Convert.ToInt32(taobaoAcc))
                {
                    //throw new BizException("淘宝订单的收款单不能取消确认");
                    ThrowBizException("SOIncome_TaobaoCanntCancelConfirm");
                }
            }
        }
    }
}