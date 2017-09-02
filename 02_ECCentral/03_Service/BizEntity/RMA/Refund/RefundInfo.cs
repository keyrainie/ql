using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// 退款单信息
    /// </summary>
    public class RefundInfo : IIdentity, IWebChannel
    {
        public RefundInfo()
        {
            this.IncomeBankInfo = new SOIncomeRefundInfo();
            this.RefundItems = new List<RefundItemInfo>();
        }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public WebChannel WebChannel { get; set; }
        /// <summary>
        /// 所属公司代码
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 退款单号
        /// </summary>
        public string RefundID { get; set; }
        /// <summary>
        /// 销售单号
        /// </summary>
        public int? SOSysNo { get; set; }
        /// <summary>
        /// 顾客系统编号
        /// </summary>
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public int? AuditUserSysNo { get; set; }
        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime? RefundTime { get; set; }
        /// <summary>
        /// 退款人
        /// </summary>
        public int? RefundUserSysNo { get; set; }

        /// <summary>
        /// 补偿运费
        /// </summary>
        public decimal? CompensateShipPrice { get; set; }

        /// <summary>
        /// 积分支付比例
        /// </summary>
        public decimal? SOCashPointRate { get; set; }

        /// <summary>
        /// 初算退现金
        /// </summary>
        public decimal? OrgCashAmt { get; set; }

        /// <summary>
        /// 初算退礼品卡
        /// </summary>
        public decimal? OrgGiftCardAmt { get; set; }

        /// <summary>
        /// 退礼品卡
        /// </summary>
        public decimal? GiftCardAmt { get; set; }

        /// <summary>
        /// 初算退积分
        /// </summary>
        public int? OrgPointAmt { get; set; }

        /// <summary>
        /// 从账户扣积分
        /// </summary>
        public int? DeductPointFromAccount { get; set; }

        /// <summary>
        /// 本次退款扣积分
        /// </summary>
        public decimal? DeductPointFromCurrentCash { get; set; }

        /// <summary>
        /// 退现金
        /// </summary>
        public decimal? CashAmt { get; set; }

        /// <summary>
        /// 退积分
        /// </summary>
        public int? PointAmt { get; set; }

        /// <summary>
        /// 退款支付类型
        /// </summary>
        public RefundPayType? RefundPayType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public RMARefundStatus? Status { get; set; }

        /// <summary>
        /// 是否涉及现金
        /// </summary>
        public CashFlagStatus? CashFlag { get; set; }

        /// <summary>
        /// 财务备注
        /// </summary>
        public string FinanceNote { get; set; }
        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 发票所在地
        /// </summary>
        public string InvoiceLocation { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string SOInvoiceNo { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        public int? RefundReason { get; set; }
        /// <summary>
        /// 是否审核通过
        /// </summary>
        public bool? CheckIncomeStatus { get; set; }
        /// <summary>
        /// 退款单Item
        /// </summary>
        public List<RefundItemInfo> RefundItems { get; set; }

        /// <summary>
        /// 银行退款单信息
        /// </summary>
        public SOIncomeRefundInfo IncomeBankInfo { get; set; }

        /// <summary>
        /// 是否扣回价保
        /// </summary>
        public bool HasPriceprotectPoint { get; set; }

        /// <summary>
        /// 价保分数
        /// </summary>
        public int? PriceprotectPoint { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public int? CreateUserSysNo { get; set; }
    }
    /// <summary>
    /// 退款单件信息
    /// </summary>
    public class RegisterForRefund
    {
        /// <summary>
        /// 退款Item编号
        /// </summary>
        public int? ItemSysNo { get; set; }
        /// <summary>
        /// 单件编号
        /// </summary>
        public int? RegisterSysNo { get; set; }
        /// <summary>
        /// 发还状态
        /// </summary>
        public RMARevertStatus? RevertStatus { get; set; }
        /// <summary>
        /// 商品类型
        /// </summary>
        public SOProductType? ProductType { get; set; }
        /// <summary>
        /// 销售仓库
        /// </summary>
        public string SalesWarehouse { get; set; }
        /// <summary>
        /// 接收仓库
        /// </summary>
        public string ReceiveWarehouse { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        public decimal? Cost { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }
    }
}
