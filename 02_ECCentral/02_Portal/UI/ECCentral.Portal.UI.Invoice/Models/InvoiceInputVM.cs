using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Media;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class InvoiceInputVM : ModelBase
    {
        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                base.SetValue("IsChecked", ref isChecked, value);
            }
        }
        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo
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
        /// 单据(S)
        /// </summary>
        public string PO_S
        {
            get;
            set;
        }
        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? POAmtSum
        {
            get;
            set;
        }
        /// <summary>
        /// EIMS金额
        /// </summary>
        public decimal? EIMSAmtSum
        {
            get;
            set;
        }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal? PaymentAmt
        {
            get;
            set;
        }
        /// <summary>
        /// 发票(S)
        /// </summary>
        public string Invoice_S
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
        /// 差异处理
        /// </summary>
        public InvoiceDiffType? DiffTaxTreatmentType
        {
            get;
            set;
        }

        /// <summary>
        /// 创建人
        /// </summary>
        public string InUserAdd
        {
            get;
            set;
        }

        /// <summary>
        /// 审核人
        /// </summary>
        public string ConfirmUser
        {
            get;
            set;
        }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? InDate
        {
            get;
            set;
        }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? ConfirmDate
        {
            get;
            set;
        }

        /// <summary>
        /// PO单金额
        /// </summary>
        public decimal? PayableAmt
        {
            get;
            set;
        }
        /// <summary>
        /// EIMS金额
        /// </summary>
        public decimal? PayableTaxAmt
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

        public SapImportedStatus? SapImportedStatus
        {
            get;
            set;
        }//   -- 导入状态

        public DateTime? SAPPostDate
        {
            get;
            set;
        }// --导入时间

        public string SAPDocNo
        {
            get;
            set;
        }//    --导入凭证号

        public string SapInFailedReason
        {
            get;
            set;
        }//--失败原因

        public int IsVendorPortal
        {
            get;
            set;
        }

        public string SapImportedStatusDesc
        {
            get
            {
                if (SapImportedStatus == null)
                {
                    return ResInvoiceInputQuery.Msg_SapImportUnhandled;
                }
                return SapImportedStatus.ToDescription();
            }
        }
        public string HyperlinkContent
        {
            get
            {
                return ((this.Status == APInvoiceMasterStatus.Origin || this.Status == APInvoiceMasterStatus.NeedAudit)
                    && IsVendorPortal == 0) ? ResInvoiceInputQuery.Hyperlink_Edit : ResInvoiceInputQuery.Hyperlink_View;
            }
        }
        public string HyperlinkCommandParameter
        {
            get
            {
                return (this.Status == APInvoiceMasterStatus.Origin || this.Status == APInvoiceMasterStatus.NeedAudit
                    && IsVendorPortal == 0) ? "Update" : "View";
            }
        }
        public SolidColorBrush StatusColor
        {
            get
            {
                if (this.Status == APInvoiceMasterStatus.AuditPass)
                {
                    return new SolidColorBrush(Colors.Green);
                }
                else
                {
                    return new SolidColorBrush(Colors.Black);
                }
            }
        }

       /// <summary>
       ///  付款结算公司
       /// </summary>
        public string PaySettleCompany { get; set; }
        public string PaySettleCompanyStr
        {
            get
            {
                return ECCentral.Portal.Basic.Utilities.EnumConverter.GetDescription(this.PaySettleCompany, typeof(ECCentral.BizEntity.PO.PaySettleCompany));
            }
        }
    }

    public class VPCancelReasonVM : ModelBase
    {
        public List<int> SysNoList
        {
            get;
            set;
        }
        private string vpCancelReason;
        [Validate(ValidateType.Required)]
        public string VPCancelReason
        {
            get
            {
                return vpCancelReason;
            }
            set
            {
                base.SetValue("VPCancelReason", ref vpCancelReason, value);
            }
        }
    }
}
