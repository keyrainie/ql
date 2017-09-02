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

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class EIMSInvoiceInfoEntityVM : ModelBase
    {
        public int SysNo { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        public string Index { get; set; }

        public int? VendorNumber { get; set; }

        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[.,\d]{1,10}$",ErrorMessage="ss")]
        public string InvoiceInputNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public decimal? InvoiceInputAmount { get; set; }

        public decimal? TaxRate { get; set; }

        public string TaxRateDes
        {
           get{
               if (TaxRate == Convert.ToDecimal(0.05))
                   return "5%";
               else if (TaxRate == Convert.ToDecimal(0.17))
                   return "17%";
               else if (TaxRate == Convert.ToDecimal(0.06))
                   return "6%";
               else
                   return string.Empty;
               }
            set { }
            
        }

        public DateTime? InvoiceInputDateTime { get; set; }

        public string InvoiceInputUser { get; set; }

        public DateTime? InvoiceEditDateTime { get; set; }

        public string InvoiceEditUser { get; set; }

        public string Memo { get; set; }

        public string CurrentUser { get; set; }

        public int Status { get; set; }

        public List<EIMSInvoiceInputExtendVM> EIMSInvoiceInputExtendList { get; set; }
    }
    public class EIMSInvoiceInputExtendVM : ModelBase
    {
        public int SysNo { get; set; }

        public string InvoiceNumber { get; set; }

        public int InvoiceInputSysNo { get; set; }

        public int Status { get; set; }
    }
}
