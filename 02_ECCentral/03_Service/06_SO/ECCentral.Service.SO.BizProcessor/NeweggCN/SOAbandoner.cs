using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage;

namespace ECCentral.Service.SO.BizProcessor
{
    /// <summary>
    /// 作废订单。属性 Parameter 说明 ：
    /// Parameter[0] : bool , 表示订单作废后订单中商品是否立即返还库存,默认为false；
    /// Parameter[1] : bool , 如果有有效的收款记录是否生成负收款单后再作废订单
    /// Parameter[2] : ECCentral.BizEntity.Invoice.SOIncomeRefundInfo , 负收款信息。
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { SOActionFactory.ThirdPartSOFilter, "Abandon" })]
    public class ThirdPartSOAbandoner : SOAbandoner
    {
        #region 作废订单，返还订单相关金额

        /// <summary>
        /// 取消订单的使用优惠券，取消订单商品优惠券的分摊，重新设置商品价格。
        /// 注意：此方法还没有保存到数据库中，是通过 SaveCurrentSO()来保存
        /// </summary>
        protected override void CancelCoupon()
        {
            //第三方订单不会使用优惠券，所以不用取消
        }


        /// <summary>
        /// 取消订单支付
        /// </summary>
        protected override void CancelSOPay()
        {
            if (NeedAbandonSOIncome(CurrentSO.BaseInfo.SpecialSOType))
            {
                AbandonSOIncome();
            }
            else
            {
                base.CancelSOPay();
            }
        }

        /// <summary>
        /// 1. 当与渠道订单属于帐期结算，可以作废收款单
        /// 2. 淘宝订单属于商城自己收款，所以可以不用作废收款单
        /// </summary>
        /// <param name="spcialSOType"></param>
        /// <returns></returns>
        private bool NeedAbandonSOIncome(SpecialSOType? specialSOType)
        {
            string[] spcialSOTypeList = (AppSettingManager.GetSetting("SO", "ThirdPart_AbandonOrderWithSOIncome") ?? string.Empty).Split(new char[','], StringSplitOptions.RemoveEmptyEntries);
            return spcialSOTypeList.Contains(specialSOType.ToString());
        }

        #endregion

        #region 创建财务负收款并作废订单

        protected override void CreateAOAndAbandonSO(SOIncomeInfo soIncomeInfo, SOIncomeRefundInfo soIncomeRefundInfo)
        {
            // 下面类型订单不用创建负收款单
            if (!NeedCreateAO(CurrentSO.BaseInfo.SpecialSOType))
            {
                Abandon();
            }
            else
            {
                base.CreateAOAndAbandonSO(soIncomeInfo, soIncomeRefundInfo);
            }
        }

        private bool NeedCreateAO(SpecialSOType? specialSOType)
        {
            string[] spcialSOTypeList = (AppSettingManager.GetSetting("SO", "ThirdPart_AbandonOrderNoCreateAO") ?? string.Empty).Split(new char[','], StringSplitOptions.RemoveEmptyEntries);
            return !spcialSOTypeList.Contains(specialSOType.ToString());
        }
        #endregion
    }

}