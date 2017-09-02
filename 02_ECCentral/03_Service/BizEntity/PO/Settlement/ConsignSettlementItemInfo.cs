using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 代销结算单商品结算信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ConsignSettlementItemInfo : ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? ItemSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 结算单编号
        /// </summary>
        [DataMember]
        public int? SettleSysNo { get; set; }

        /// <summary>
        /// 结算规则编号
        /// </summary>
        [DataMember]
        public int? SettleRuleSysNo { get; set; }

        /// <summary>
        /// 结算规则名称
        /// </summary>
        [DataMember]
        public string SettleRuleName { get; set; }

        /// <summary>
        /// 结算价格
        /// </summary>
        [DataMember]
        public decimal? SettlePrice { get; set; }

        /// <summary>
        /// 供应商信息
        /// </summary>
        [DataMember]
        public VendorInfo VendorInfo { get; set; }

        /// <summary>
        /// 结算类型
        /// </summary>
        [DataMember]
        public SettleType? SettleType { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        [DataMember]
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string ProductName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public ValidStatus? Status { get; set; }

        /// <summary>
        /// 创建时成本
        /// </summary>
        [DataMember]
        public decimal? CreateCost { get; set; }

        /// <summary>
        /// 发放积分
        /// </summary>
        [DataMember]
        public int? Point { get; set; }

        /// <summary>
        /// 仓库
        /// </summary>
        [DataMember]
        public int? StockSysNo { get; set; }

        [DataMember]
        public string StockName { get; set; }
        /// <summary>
        /// 结算价格
        /// </summary>
        [DataMember]
        public decimal? Cost { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public int? Quantity { get; set; }

        /// <summary>
        /// 货币编码
        /// </summary>
        [DataMember]
        public int? CurrenyCode { get; set; }

        /// <summary>
        /// 佣金百分比
        /// </summary>
        [DataMember]
        public decimal? SettlePercentage { get; set; }

        [DataMember]
        public int? AcquireReturnPointType { get; set; }

        [DataMember]
        public decimal? AcquireReturnPoint { get; set; }

        /// <summary>
        /// 结算商品产出返利金额
        /// </summary>
        [DataMember]
        public decimal? ExpectGetPoint { get; set; }

        /// <summary>
        /// 代销转财务记录
        /// </summary>
        [DataMember]
        public ConsignToAcctLogInfo ConsignToAccLogInfo { get; set; }

        /// <summary>
        /// 代销转财务记录编码
        /// </summary>
        [DataMember]
        public int? POConsignToAccLogSysNo { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }

        public ConsignToAccStatus ConsignToAccStatus { get; set; }

        public DateTime CreateTime { get; set; }

        public Decimal FoldCost { get; set; }

        public Decimal MinCommission { get; set; }

        public Decimal RetailPrice { get; set; }

        public string VendorName { get; set; }

        public int VendorSysNo { get; set; }
    }
}
