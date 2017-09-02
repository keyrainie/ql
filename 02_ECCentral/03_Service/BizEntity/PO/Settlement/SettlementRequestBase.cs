using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.IM;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 结算单通用基类(代销结算单，代收结算单)
    /// </summary>
    [Serializable]
    [DataContract]
    public class SettleRequestBase : IIdentity, ICompany
    {

        public SettleRequestBase()
        {
            VendorInfo = new VendorInfo();
            SourceStockInfo = new StockInfo();
        }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }
        /// <summary>
        /// 单据编号
        /// </summary>
        [DataMember]
        public string ReferenceSysNo { get; set; }

        /// <summary>
        [DataMember]
        public VendorInfo VendorInfo { get; set; }
        /// 供应商信息
        /// </summary>

        /// <summary>
        /// 源渠道仓库
        /// </summary>
        [DataMember]
        public StockInfo SourceStockInfo { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        [DataMember]
        public PurchaseOrderTaxRate? TaxRateData { get; set; }

        /// <summary>
        /// 货币类型
        /// </summary>
        [DataMember]
        public int? CurrencyCode { get; set; }

        /// <summary>
        /// 结算总金额
        /// </summary>
        [DataMember]
        public decimal? TotalAmt { get; set; }

        /// <summary>
        /// 结算单状态
        /// </summary>
        [DataMember]
        public SettleStatus? Status { get; set; }

        /// <summary>
        /// 调整单编号
        /// </summary>
        [DataMember]
        public string SettleBalanceSysNo { get; set; }

        /// <summary>
        /// 创建时成本
        /// </summary>
        [DataMember]
        public decimal? CreateCostTotalAmt { get; set; }

        /// <summary>
        /// 差额
        /// </summary>
        [DataMember]
        public decimal? Difference { get; set; }

        /// <summary>
        /// 创建日期从
        /// </summary>
        [DataMember]
        public DateTime? CreateDateFrom { get; set; }

        /// <summary>
        /// 创建日期到 
        /// </summary>
        [DataMember]
        public DateTime? CreateDateTo { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [DataMember]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        [DataMember]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 创建人系统编号
        /// </summary>
        [DataMember]
        public int? CreateUserSysNo { get; set; }

        /// <summary>
        /// 创建人显示名称
        /// </summary>
        [DataMember]
        public string InUser { get; set; }

        /// <summary>
        /// 结算所属PM信息
        /// </summary>
        [DataMember]
        public ProductManagerInfo PMInfo { get; set; }

        /// <summary>
        /// 毛利总金额
        /// </summary>
        [DataMember]
        public decimal? RateMarginCount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Memo { get; set; }

        /// <summary>
        /// 记录
        /// </summary>
        [DataMember]
        public string Note { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        [DataMember]
        public UserInfo AuditUser { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        [DataMember]
        public DateTime? AuditDate { get; set; }

        /// <summary>
        /// 出库人
        /// </summary>
        [DataMember]
        public UserInfo OutStockUser { get; set; }

        /// <summary>
        /// 出库日期
        /// </summary>
        [DataMember]
        public DateTime? OutStockDate { get; set; }

        /// <summary>
        /// 结算单编号
        /// </summary>
        [DataMember]
        public string SettleID { get; set; }

        /// <summary>
        /// 结算人
        /// </summary>
        [DataMember]
        public UserInfo SettleUser { get; set; }

        /// <summary>
        /// 结算日期
        /// </summary>
        [DataMember]
        public DateTime? SettleDate { get; set; }


        [DataMember]
        public DateTime? OutStockRefundDateFrom {get; set;}


        [DataMember]
        public DateTime? OutStockRefundDateTo {get;set;}

        [DataMember]
        public VendorIsToLease? LeaseType { get; set; }

    }
}
