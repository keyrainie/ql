using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
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

namespace ECCentral.Portal.UI.PO.Models.Settlement
{
    public class SettleOrderFinancePayItemVM:ModelBase
    {
        private bool? _IsChecked;
        public bool? IsChecked {
            get { return _IsChecked; }
            set { 
                this.SetValue("IsChecked",ref _IsChecked,value);
            }
        }
            
        public decimal? Amount { get; set; }

        public decimal? Tax { get; set; }
        public decimal? Cost { get; set; }

        public decimal? Tax17 { get; set; }
        public decimal? Cost17 { get; set; }
        public decimal? Tax13 { get; set; }
        public decimal? Cost13 { get; set; }
        public decimal? TaxOt { get; set; }
        public decimal? CostOt { get; set; }

        public DateTime? InDate { get; set; }
        public int? OrderSysNo { get; set; }
        public int? OrderType { get; set; }

        public PayableOrderType? FinancePayOrderType{get;set;}

        public int? FinancePaySysNo { get; set; }

        public string OrderTypeStr { get; set; }
        public int? VendorSysNo { get; set; }
        public string VendorName { get; set; }
    }
}
