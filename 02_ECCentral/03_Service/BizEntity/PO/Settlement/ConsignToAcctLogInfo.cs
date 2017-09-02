using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 代销转财务记录
    /// </summary>
    [Serializable]
    [DataContract]
    public class ConsignToAcctLogInfo : ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? LogSysNo { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        [DataMember]
        public int? VendorSysNo { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        [DataMember]
        public int? StockSysNo { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        [DataMember]
        public string StockName { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public int? OrderSysNo { get; set; }

        /// <summary>
        /// 代销转财务类型
        /// </summary>
        [DataMember]
        public ConsignToAccountType? ConsignToAccType { get; set; }

        /// <summary>
        /// 代销转财务记录状态
        /// </summary>
        [DataMember]
        public ConsignToAccountLogStatus? ConsignToAccStatus { get; set; }

        /// <summary>
        /// 结算类型
        /// </summary>
        [DataMember]
        public SettleType? SettleType { get; set; }

        /// <summary>
        /// 结算百分比
        /// </summary>
        [DataMember]
        public decimal? SettlePercentage { get; set; }


        /// <summary>
        /// 订单数量
        /// </summary>
        [DataMember]
        public int? OrderCount { get; set; }

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
        ///  商品名称
        /// </summary>
        [DataMember]
        public string ProductName { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        [DataMember]
        public int? ProductQuantity { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        [DataMember]
        public decimal? SalePrice { get; set; }

        /// <summary>
        /// 创建时成本
        /// </summary>
        [DataMember]
        public decimal? CreateCost { get; set; }

        /// <summary>
        /// 结算
        /// </summary>
        [DataMember]
        public decimal? Cost { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        [DataMember]
        public decimal? CountMany { get; set; }

        /// <summary>
        /// 代销成本
        /// </summary>
        [DataMember]
        public decimal? SettleCost { get; set; }

        /// <summary>
        /// 发放积分
        /// </summary>
        [DataMember]
        public int? Point { get; set; }

        /// <summary>
        /// 毛利
        /// </summary>
        [DataMember]
        public decimal? RateMargin { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        [DataMember]
        public decimal? TotalAmt { get; set; }

        /// <summary>
        /// 毛利总额
        /// </summary>
        [DataMember]
        public decimal? RateMarginTotal { get; set; }

        /// <summary>
        /// 最低佣金限额
        /// </summary>
        [DataMember]
        public decimal? MinCommission { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Note { get; set; }

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
        /// 公司编号
        /// </summary>
        [DataMember]
        public string StoreCompanyCode { get; set; }

        /// <summary>
        /// 单据出库时间
        /// </summary>
        [DataMember]
        public DateTime? OutStockTime { get; set; }

        [DataMember]
        public decimal? FoldCost { get; set; }


        /// <summary>
        /// 代销（1） 还是 代收代付  （4）
        /// </summary>
        [DataMember]
        public int? IsConsign { get; set; }
    }
}
