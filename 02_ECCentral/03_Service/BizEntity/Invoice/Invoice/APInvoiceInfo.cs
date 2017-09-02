using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.Invoice.Invoice
{
    /// <summary>
    /// 发票匹配审核信息
    /// </summary>
    public class APInvoiceInfo : IIdentity, ICompany
    {
        public APInvoiceInfo()
        {
            POItemList = new List<APInvoicePOItemInfo>();
            InvoiceItemList = new List<APInvoiceInvoiceItemInfo>();
        }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 销售方
        /// </summary>
        public int? VendorPortalSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// SAP导入时间
        /// </summary>
        public DateTime? DocDate
        {
            get;
            set;
        }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public int? StockSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? VendorSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName
        {
            get;
            set;
        }
        /// <summary>
        /// 税额
        /// </summary>
        public decimal? VendorTaxRate
        {
            get;
            set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo
        {
            get;
            set;
        }
        /// <summary>
        /// 差异备注
        /// </summary>
        public string DiffMemo
        {
            get;
            set;
        }
        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? PoNetAmt
        {
            get;
            set;
        }
        /// <summary>
        /// 单据税额
        /// </summary>
        public decimal? PoNetTaxAmt
        {
            get;
            set;
        }
        /// <summary>
        /// 发票金额
        /// </summary>
        public decimal? InvoiceAmt
        {
            get;
            set;
        }
        /// <summary>
        /// 发票税额
        /// </summary>
        public decimal? InvoiceTaxAmt
        {
            get;
            set;
        }
        /// <summary>
        /// 差异金额
        /// </summary>
        public decimal? DiffTaxAmt
        {
            get;
            set;
        }
        /// <summary>
        /// 差异处理方式
        /// </summary>
        public int? DiffTaxTreatmentType
        {
            get;
            set;
        }
        /// <summary>
        /// 状态
        /// </summary>
        public APInvoiceMasterStatus? Status
        {
            get;
            set;
        }
        /// <summary>
        /// 确定人
        /// </summary>
        public string ConfirmUser { get; set; }
        /// <summary>
        /// 确定时间
        /// </summary>
        public DateTime? ConfirmDate { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        public int? UpdateInvoiceUserSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get;
            set;
        }
        /// <summary>
        /// 所属公司代码
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// PO单据列表
        /// </summary>
        public List<APInvoicePOItemInfo> POItemList { get; set; }
        /// <summary>
        /// 发票单据列表
        /// </summary>
        public List<APInvoiceInvoiceItemInfo> InvoiceItemList { get; set; }

    }
    /// <summary>
    /// PO单据列表
    /// </summary>
    public class APInvoicePOItemInfo : IIdentity
    {
        /// <summary>
        /// 单据编号
        /// </summary>
        public int? PoNo
        {
            get;
            set;
        }

        /// <summary>
        /// PO仓库编号
        /// </summary>
        public int? PoStockSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// PO付款货币
        /// </summary>
        public int? PoCurrency
        {
            get;
            set;
        }
        /// <summary>
        /// po金额
        /// </summary>
        public decimal? PoAmt
        {
            get;
            set;
        }
        /// <summary>
        /// 对应EIMS号
        /// </summary>
        public int? EIMSNo
        {
            get;
            set;
        }
        /// <summary>
        /// 对应EIMS号列表
        /// </summary>
        public string EIMSNoList
        {
            get;
            set;
        }
        /// <summary>
        /// EIMS金额
        /// </summary>
        public decimal? EIMSAmt
        {
            get;
            set;
        }
        /// <summary>
        /// EIMS净额
        /// </summary>
        public decimal? EIMSNetAmt
        {
            get;
            set;
        }
        /// <summary>
        /// EIMS税额
        /// </summary>
        public decimal? EIMSNetTaxAmt
        {
            get;
            set;
        }
        /// <summary>
        /// PO净额
        /// </summary>
        public decimal? PoNetAmt
        {
            get;
            set;
        }
        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal? PaymentAmt
        {
            get;
            set;
        }
        /// <summary>
        /// PO入库时间
        /// </summary>
        public DateTime? PoBaselineDate
        {
            get;
            set;
        }
        /// <summary>
        /// PO付款方式
        /// </summary>
        public int? PoPaymentTerm
        {
            get;
            set;
        }
        /// <summary>
        /// 发票状态
        /// </summary>
        public APInvoiceItemStatus? Status
        {
            get;
            set;
        }
        /// <summary>
        /// 单据类型
        /// </summary>
        public PayableOrderType? OrderType
        {
            get;
            set;
        }
        /// <summary>
        /// 批号
        /// </summary>
        public int? BatchNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 税额
        /// </summary>
        public decimal? PayableTaxAmt
        {
            get;
            set;
        }
        /// <summary>
        /// 预计付款时间
        /// </summary>
        public DateTime? ETP
        {
            get;
            set;
        }

        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        #endregion IIdentity Members
    }

    /// <summary>
    /// 发票单据
    /// </summary>
    public class APInvoiceInvoiceItemInfo : IIdentity
    {
        /// <summary>
        /// 单据编号
        /// </summary>
        public int? PoNo
        {
            get;
            set;
        }
        /// <summary>
        /// 发票编号
        /// </summary>
        public string InvoiceNo
        {
            get;
            set;
        }
        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? InvoiceDate
        {
            get;
            set;
        }
        /// <summary>
        /// 发票货币类型
        /// </summary>
        public int? InvoiceCurrency
        {
            get;
            set;
        }
        /// <summary>
        /// 发票总金额
        /// </summary>
        public decimal InvoiceAmt
        {
            get;
            set;
        }
        /// <summary>
        /// 发票净额
        /// </summary>
        public decimal InvoiceNetAmt
        {
            get;
            set;
        }
        /// <summary>
        /// 发票税额
        /// </summary>
        public decimal InvoiceTaxAmt
        {
            get;
            set;
        }
        /// <summary>
        /// 状态
        /// </summary>
        public APInvoiceItemStatus? Status
        {
            get;
            set;
        }

        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        #endregion IIdentity Members
    }
}
