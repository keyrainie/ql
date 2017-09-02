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

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class HeaderDataVM : ModelBase
    {
        public int? TransactionNumber { get; set; }

        public DateTime? PostingDate { get; set; }

        public string CompanyCode { get; set; }

        public string DocumentType { get; set; }

        public string GLAccount { get; set; }

        public Decimal? SAP_GLAmount { get; set; }

        public string FI_DOC { get; set; }

        public string LineItem { get; set; }

        public string FiscalYear { get; set; }

        public DateTime? InDate { get; set; }
    }

    public class CompanyCodeVM : ModelBase
    {
        public string SapCoCode { get; set; }
    }
}
