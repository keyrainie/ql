using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(PostPayProcessor), Version = "2.0.0.0")]
    public class NECN_PostPayProcessor : PostPayProcessor
    {
        protected override void PreCheckForCreate(PostPayInfo entity, SOIncomeRefundInfo refundInfo, SOBaseInfo soBaseInfo, bool isForceCheck)
        {
            if (isForceCheck && refundInfo.RefundPayType != null)
            {
                if (refundInfo.RefundPayType == RefundPayType.CashRefund)
                {
                    //只有货到付款(OZZO奥硕物流)和现金支付才可以选择现金退款
                    if (!CanCashRefund(soBaseInfo.PayTypeSysNo.Value))
                    {
                        ThrowBizException("PostPay_CashRefundNotAllowed");
                    }
                }
            }
            base.PreCheckForCreate(entity, refundInfo, soBaseInfo, isForceCheck);
        }

        /// <summary>
        /// 是否可以现金退款
        /// </summary>
        /// <param name="payTypeSysNo"></param>
        /// <returns></returns>
        private bool CanCashRefund(int payTypeSysNo)
        {
            //是否可以支持现金退款，原来是直接写死在代码中，现在通过配置来解决。
            string cfg = AppSettingManager.GetSetting("Invoice", "PostPayCanCashRefundPayType");
            if (!string.IsNullOrEmpty(cfg))
            {
                String[] payTypeSysNos = cfg.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return payTypeSysNos.Contains(payTypeSysNo.ToString());
            }
            return false;
        }
    }
}