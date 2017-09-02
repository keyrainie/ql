using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 免运费规则
    /// </summary>
    public class FreeShippingChargeRuleInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 免运费条件金额类型
        /// </summary>
        public FreeShippingAmountSettingType? AmountSettingType { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public FreeShippingAmountSettingStatus? Status { get; set; }

        /// <summary>
        /// 免运费条件的门槛金额
        /// </summary>
        public decimal? AmountSettingValue { get; set; }

        /// <summary>
        /// 支付类型
        /// </summary>
        public List<SimpleObject> PayTypeSettingValue { get; set; }

        /// <summary>
        /// 配送区域
        /// </summary>
        public List<SimpleObject> ShipAreaSettingValue { get; set; }

        /// <summary>
        /// 免运费商品
        /// </summary>
        public List<SimpleObject> ProductSettingValue { get; set; }

        /// <summary>
        /// 是否是整网模式
        /// </summary>
        public bool IsGlobal { get; set; }

        /// <summary>
        /// 规则描述
        /// </summary>
        public string Description { get; set; }
    }
}
