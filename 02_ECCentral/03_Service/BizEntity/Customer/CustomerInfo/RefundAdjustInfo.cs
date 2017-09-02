using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 补偿退款单单信息
    /// </summary>
    public class RefundAdjustInfo : IIdentity, ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 所属公司代码
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// RMA申请单系统编号
        /// </summary>
        public int? RequestSysNo { get; set; }
        /// <summary>
        /// 原始销售单系统编号
        /// </summary>
        public int? SOSysNo { get; set; }
        /// <summary>
        /// 退款单号
        /// </summary>
        public string RefundID { get; set; }
        /// <summary>
        /// RMA申请单号
        /// </summary>
        public string RequestID { get; set; }
        /// <summary>
        /// 补偿退款单类型
        /// </summary>
        public RefundAdjustType? AdjustOrderType { get; set; }
        /// <summary>
        /// 顾客编号
        /// </summary>
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 顾客账号
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// 补偿退款单创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 补偿单审核通过时间
        /// </summary>
        public DateTime? RefundTime { get; set; }
        /// <summary>
        /// 创建人系统编号
        /// </summary>
        public int? CreateUserSysNo { get; set; }
        /// <summary>
        /// 退款人系统编号
        /// </summary>
        public int? RefundUserSysNo { get; set; }
        /// <summary>
        /// 补偿现金金额
        /// </summary>
        public decimal? CashAmt { get; set; }
        /// <summary>
        /// 补偿礼品卡金额
        /// </summary>
        public decimal? GiftCardAmt { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public RefundAdjustStatus? Status { get; set; }
        /// <summary>
        /// 补偿说明
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 退款类型
        /// </summary>
        public RefundPayType? RefundPayType { get; set; }
        /// <summary>
        /// 退积分
        /// </summary>
        public int? PointAmt { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string Action { get; set; }

        #region 银行信息

        /// <summary>
        /// 开户银行
        /// </summary>
        public string BankName
        {
            get;
            set;
        }

        /// <summary>
        /// 支行
        /// </summary>
        public string BranchBankName
        {
            get;
            set;
        }

        /// <summary>
        /// 银行卡号
        /// </summary>
        public string CardNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 持卡人
        /// </summary>
        public string CardOwnerName
        {
            get;
            set;
        }

        #endregion 银行信息
    }
}

