using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 供应商佣金信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class VendorCommissionInfo
    {
        [DataMember]
        public int? CommissionSysNo { get; set; }
        /// <summary>
        /// 店租佣金
        /// </summary>
        [DataMember]
        public decimal? RentFee { get; set; }

        /// <summary>
        /// 销售提成
        /// </summary>
        [DataMember]
        public VendorStagedSaleRuleEntity SaleRuleEntity { get; set; }

        /// <summary>
        /// 销售提成 - 阶梯设置(XML)
        /// </summary>
        [DataMember]
        public string StagedSaleRuleItemsXml { get; set; }

        /// <summary>
        /// 保底金额
        /// </summary>
        [DataMember]
        public decimal? GuaranteedAmt { get; set; }

        /// <summary>
        /// 订单提成
        /// </summary>
        [DataMember]
        public decimal? OrderCommissionAmt { get; set; }

        /// <summary>
        /// 配送费
        /// </summary>
        [DataMember]
        public decimal? DeliveryFee { get; set; }

        /// <summary>
        /// 供应商代理编号
        /// </summary>
        [DataMember]
        public int? VendorManufacturerSysNo { get;set;}
    }
}
