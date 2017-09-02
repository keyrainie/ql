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
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class OldChangeNewQueryVM : ModelBase
    {
        public string InUser { get; set; }

        public DateTime? InDate { get; set; }

        public string ConfirmUser { get; set; }

        public DateTime? ConfirmDate { get; set; }

        public int StatusCode { get; set; }

        public int? SysNo { get; set; }

        public string TradeInId { get; set; }

        [Validate(ValidateType.Required)]
        public int? SOSysNo { get; set; }

        public string SysNoList { get; set; }

        public string Licence { get; set; }

        public decimal Rebate { get; set; }

        public decimal ReviseRebate { get; set; }

        public string CustomerID { get; set; }

        public int CustomerSysNo { get; set; }

        public string CustomerName { get; set; }

        public string receivePhone { get; set; }

        public string ReceiveContact { get; set; }

        public string AreaInfo { get; set; }

        public string ReceiveAddress { get; set; }

        public string ShipTypeName { get; set; }

        public List<SOItemVM> SOItems { get; set; }

        public string BankName { get; set; }

        public string BranchBankName { get; set; }

        public string BankAccount { get; set; }

        public string Note { get; set; }

        public decimal OrderAmt { get; set; }

        public string NoteStr
        {
            get
            {
                if (string.IsNullOrEmpty(Note))
                    return "N/A";
                else
                    return this.Note;
            }
            set { }
        }

        public string RBNote { get; set; }

        public string ReferenceID { get; set; }

        public OldChangeNewStatus Status
        {
            get;
            set;
        }
        public string StatusStr
        {
            get
            {
                switch (Status)
                {
                    case OldChangeNewStatus.Abandon:
                        return "作废";
                    case OldChangeNewStatus.Audited:
                        return "审核通过";
                    case OldChangeNewStatus.Close:
                        return "关闭";
                    case OldChangeNewStatus.Origin:
                        return "初始";
                    case OldChangeNewStatus.Refund:
                        return "退款";
                    case OldChangeNewStatus.RefuseAudit:
                        return "拒绝审核";
                    case OldChangeNewStatus.SubmitAudit:
                        return "提交审核";
                    default:
                        return string.Empty;
                }
            }
            set { }
        }
        public string InDateStr
        {
            get
            {
                return InDate.HasValue ? InDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
            }
        }
        public string ConfirmDateStr
        {
            get
            {
                return ConfirmDate.HasValue ? ConfirmDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
            }
        }

        public decimal TotalRebate { get; set; }
        public decimal TotalReviseRebate { get; set; }
        public decimal TotalPassReviseRebate { get; set; }
        public decimal TotalReturnReviseRebate { get; set; }

        public string TotalAmountString { get; set; }
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

        public string CompanyCode
        {
            get;
            set;
        }


    }

    public class SOItemVM:ModelBase
    {
        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
