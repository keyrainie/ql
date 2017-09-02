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

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerAccountPeriodVM : ModelBase
    {
        public CustomerAccountPeriodVM()
        {
            accountPeriodDays = 0;
        }

        private bool isUseChequesPay;
        public bool IsUseChequesPay
        {
            get
            {
                return isUseChequesPay;
            }
            set
            {
                SetValue("IsUseChequesPay", ref isUseChequesPay, value);
            }
        }

        private bool isAllowComment;
        public bool IsAllowComment
        {
            get
            {
                return isAllowComment;
            }
            set
            {
                SetValue("IsAllowComment", ref isAllowComment, value);
            }
        }

        public decimal? ConfirmedTotalAmt { get; set; }
        public int? CustomerSysNo { get; set; }

        public string CustomerID { get; set; }

        private int? accountPeriodDays;
        public int? AccountPeriodDays
        {
            get
            {
                return accountPeriodDays;
            }
            set
            {
                SetValue("AccountPeriodDays", ref accountPeriodDays, value);
            }
        }

        private decimal? totalCreditLimit;
        public decimal? TotalCreditLimit
        {
            get
            {
                return totalCreditLimit;
            }
            set
            {
                SetValue("TotalCreditLimit", ref totalCreditLimit, value);
                AvailableCreditLimit = value;
            }
        }

        private decimal? availableCreditLimit;
        public decimal? AvailableCreditLimit
        {
            get
            {
                return availableCreditLimit;
            }
            set
            {
                SetValue("AvailableCreditLimit", ref availableCreditLimit, value);
            }
        }
    }
}
