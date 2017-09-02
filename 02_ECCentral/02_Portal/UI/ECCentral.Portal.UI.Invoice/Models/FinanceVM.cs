using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class FinanceVM : ModelBase
    {
        private const string decimalFormat = "###,###,###0.00";

        public int SysNo { get; set; }

        #region 未/勾选按供应商分组
        /// <summary>
        /// 单据ID
        /// </summary>
        public int OrderID { get; set; }

        public DateTime? ETP { get; set; }

        public string ETPDisplay
        {
            get
            {
                return this.ETP.HasValue ? this.ETP.Value.ToLongDateString() : string.Empty;
            }
        }
        public string ETPDisplayForExport
        {
            get
            {
                return this.ETP.HasValue ? this.ETP.Value.ToShortDateString() : string.Empty;
            }
        }

        public string OrderIDLink
        {
            get
            {
                string result = string.Empty;
                return result;
            }
            set { }
        }

        /// <summary>
        /// 批次号
        /// </summary>
        public int BatchNumber { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        public PayableOrderType? OrderType { get; set; }

        /// <summary>
        /// 单据时间
        /// </summary>
        public DateTime? OrderDate { get; set; }
        public string OrderDateString
        {
            get
            {
                if (OrderDate != null)
                {
                    return OrderDate.Value.ToLongDateString();
                }

                return string.Empty;
            }
            set { }
        }

        /// <summary>
        /// 应付时间
        /// </summary>
        public DateTime? ShouldPayDate { get; set; }

        /// <summary>
        /// 应付
        /// </summary>
        public decimal PayableAmt { get; set; }

        /// <summary>
        /// 已付
        /// </summary>
        public decimal AlreadyP { get; set; }

        public decimal OrderAmt { get; set; }

        public string OrderAmtString
        {
            get
            {
                return this.OrderAmt.ToString(decimalFormat);
            }
        }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public int VendorSysNo { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 供应商帐期
        /// </summary>
        public string VendorPayType { get; set; }


        /// <summary>
        /// 归属PMSysNo
        /// </summary>
        public int PMUserNo { get; set; }

        /// <summary>
        /// 归属PM
        /// </summary>
        public string PMName { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderStatus { get; set; }

        /// <summary>
        /// 发票状态
        /// </summary>
        public string InvoiceStatus { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        public string CreateTimeStringForExport
        {
            get
            {
                if (CreateTime != null)
                {
                    return CreateTime.Value.ToShortDateString();
                }

                return string.Empty;
            }
            set { }
        }

        /// <summary>
        /// 审核状态
        /// </summary>
        public PayableAuditStatus AuditStatus { get; set; }
        //public string AuditStatusString
        //{
        //    get
        //    {
        //        string result = "";

        //        if (AuditStatus == PayableAuditStatus.NotAudit)
        //        {
        //            result = "待审核";
        //        }
        //        else if (AuditStatus == PayableAuditStatus.WaitFNAudit)
        //        {
        //            result = "待财务审核";
        //        }
        //        else if (AuditStatus == PayableAuditStatus.Audited)
        //        {
        //            result = "已审核";
        //        }

        //        return result;
        //    }
        //    set { }
        //}

        /// <summary>
        /// 审核人
        /// </summary>
        public string AuditUser { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime { get; set; }

        public decimal ZXAmt { get; set; }

        public decimal KCAmt { get; set; }

        public int RMACount { get; set; }

        public string ZXAmtString
        {
            get
            {
                return ZXAmt.ToString(decimalFormat);
            }
            set { }
        }

        /// <summary>
        /// Memo
        /// </summary>
        public string Memo { get; set; }

        public string NewMemo { get; set; }

        //付款结算公司
        public int PaySettleCompany { get; set; }

        /// <summary>
        /// 扩展字段，用于辅助分段审核时存放前端的验证信息
        /// </summary>
        public string Tag { get; set; }
        #endregion

        #region 勾选按供应商分组特有的

        public string AccountID { get; set; }

        public string BankName { get; set; }

        public decimal NotDEIMSAmt { get; set; }
        public string NotDEIMSAmtString { get { return NotDEIMSAmt.ToString(decimalFormat); } set { } }

        public decimal PayAmtUndue { get; set; }
        public string PayAmtUndueString { get { return PayAmtUndue.ToString(decimalFormat); } set { } }

        public decimal PayAmtMature { get; set; }
        public string PayAmtMatureString { get { return PayAmtMature.ToString(decimalFormat); } set { } }

        public decimal PayAmtLocked { get; set; }
        public string PayAmtLockedString { get { return PayAmtLocked.ToString(decimalFormat); } set { } }

        public decimal PayAmtLeft { get; set; }
        public string PayAmtLeftString { get { return PayAmtLeft.ToString(decimalFormat); } set { } }

        public string C1Name { get; set; }
        public string C1NameStr { get { return null == C1Name || string.Empty == C1Name ? "" : C1Name.Substring(0, C1Name.Length - 1); } set { } }

        public string IsConsign { get; set; }

        public string DetailOrderSysNo { get; set; }
        public string DetailOrderSysNoStr { get { return null == DetailOrderSysNo || string.Empty == DetailOrderSysNo ? "" : DetailOrderSysNo.Substring(0, DetailOrderSysNo.Length - 1); } set { } }
        /// <summary>
        /// 采购单扣减
        /// </summary>
        public decimal R2 { get; set; }

        /// <summary>
        /// 帐扣
        /// </summary>
        public decimal R4 { get; set; }

        /// <summary>
        /// 代销结算单扣减
        /// </summary>
        public decimal R3 { get; set; }

        /// <summary>
        /// 票扣
        /// </summary>
        public decimal R0 { get; set; }
        /// <summary>
        /// 现金
        /// </summary>
        public decimal R1 { get; set; }

        /// <summary>
        /// 总记录条数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// TotalPayableAmt
        /// </summary>
        public decimal TotalPayableAmt { get; set; }

        public decimal PendingInvoiceAmount { get; set; }

        public decimal EndBalanceAccrued { get; set; }

        //public string ReceiveByPODisplay
        //{
        //    get
        //    {
        //        return this.ReceiveByPO.ToString(decimalFormat);
        //    }
        //}

        //public string ReceiveByAcctDisplay
        //{
        //    get
        //    {
        //        return this.ReceiveByAcct.ToString(decimalFormat);
        //    }
        //}

        //public string ReceiveByConsignDisplay
        //{
        //    get
        //    {
        //        return this.ReceiveByConsign.ToString(decimalFormat);
        //    }
        //}

        //public string ReceiveByInvoiceDisplay
        //{
        //    get
        //    {
        //        return this.ReceiveByInvoice.ToString(decimalFormat);
        //    }
        //}
        #endregion

        private bool m_IsCheck;

        public bool IsCheck
        {
            get { return m_IsCheck; }
            set { SetValue("IsCheck", ref m_IsCheck, value); }
        }

        #region 合计已到应付
        public double TotalPayAmt { get; set; }

        public string TotalPayAmtText
        {
            get
            {
                return TotalPayAmt.ToString(decimalFormat);
            }
        }
        #endregion

        public string OrderIDDisplay
        {
            get
            {
                switch (this.OrderType)
                {
                    case PayableOrderType.PO:                 
                        return string.Format("{0}-{1}", this.OrderID, this.BatchNumber.ToString().PadLeft(2, '0'));
                    default:
                        return this.OrderID.ToString();
                }
            }
        }

        public string PaySettleCompanyDisplay
        {
            get
            {
                return ECCentral.Portal.Basic.Utilities.EnumConverter.GetDescription(this.PaySettleCompany, typeof(ECCentral.BizEntity.PO.PaySettleCompany));
            }
        }

    }
}
