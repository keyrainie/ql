using System;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.Common
{
    public class FreeShippingChargeRuleQueryFilter
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 截至日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 免运费条件金额类型
        /// </summary>
        public FreeShippingAmountSettingType? AmountSettingType { get; set; }

        /// <summary>
        /// 免运费规则状态
        /// </summary>
        public FreeShippingAmountSettingStatus? Status { get; set; }

        /// <summary>
        /// 门槛金额起始值
        /// </summary>
        public decimal? AmtFrom { get; set; }

        /// <summary>
        /// 门槛金额终止值
        /// </summary>
        public decimal? AmtTo { get; set; }

        /// <summary>
        /// 支付方式编号
        /// </summary>
        public int? PayTypeSysNo { get; set; }

        /// <summary>
        /// 配送区域编号
        /// </summary>
        public int? ShipAreaID { get; set; }
    }
}