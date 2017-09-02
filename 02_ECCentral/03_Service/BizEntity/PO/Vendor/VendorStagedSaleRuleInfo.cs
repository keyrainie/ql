using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 供应商佣金 - 销售提成信息
    /// </summary>
    [XmlRoot("SalesRuleEntity")]
    public class VendorStagedSaleRuleEntity
    {

        /// <summary>
        /// 阶梯销售设置集合
        /// </summary>
        [XmlArray("Rules")]
        [XmlArrayItem("Rule")]
        public List<VendorStagedSaleRuleInfo> StagedSaleRuleItems { get; set; }

        /// <summary>
        /// 最小佣金限额
        /// </summary>
        [XmlElement("MinCommissionAmt")]
        public decimal? MinCommissionAmt { get; set; }

        /// <summary>
        /// 阶梯销售设置信息
        /// </summary>
        public class VendorStagedSaleRuleInfo
        {
            /// <summary>
            /// 序列编号
            /// </summary>
            public int? Order { get; set; }
            /// <summary>
            /// 开始金额
            /// </summary>
            public decimal? StartAmt { get; set; }
            /// <summary>
            /// 结束金额
            /// </summary>
            public decimal? EndAmt { get; set; }
            /// <summary>
            /// 金额百分比
            /// </summary>
            public decimal? Percentage { get; set; }
        }
    }

}
