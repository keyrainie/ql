using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CustomerBaseInfo : UserControl
    {
        public CustomerBaseInfo()
        {
            InitializeComponent();
        }

        public CustomerMaster customerInfo;
        public CustomerMaster CustomerInfo
        {
            protected get { return customerInfo; }
            set
            {
                customerInfo = value;
                if (customerInfo != null)
                {
                    this.DataContext = customerInfo;
                    tbkCustomerRank.Text = customerInfo.Rank.ToDescription();
                    tbkCustomerSex.Text = customerInfo.Gender.ToDescription();
                    tbkIsVip.Visibility = customerInfo.IsVip.HasValue && customerInfo.IsVip.Value ? Visibility.Visible : System.Windows.Visibility.Collapsed;
                }
            }
        }
    }
}
