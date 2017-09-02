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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerMaster : ModelBase
    {
        #region 基本信息
        public int? SysNo
        {
            get;
            set;
        }

        public string CustomerID
        {
            get;
            set;
        }

        public string CustomerName
        {
            get;
            set;
        }

        public string Phone
        {
            get;
            set;
        }

        public string CellPhone
        {
            get;
            set;
        }

        public CustomerRank? Rank
        {
            get;
            set;
        }

        public int? Point
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public decimal? Balance
        {
            get;
            set;
        }
        public CustomerType? CustomersType
        {
            get;
            set;
        }

        public Gender? gender;
        public Gender? Gender
        {
            get
            {
                return gender;
            }
            set { gender = value; }
        }
        public DateTime? RegisterTime
        {
            get;
            set;
        }

        public decimal? TotalSOMoney
        {
            get;
            set;
        }

        public decimal? ConfirmedTotalAmt
        {
            get;
            set;
        }

        public bool? IsVip
        {
            get;
            set;
        }
        #endregion
    }
}
