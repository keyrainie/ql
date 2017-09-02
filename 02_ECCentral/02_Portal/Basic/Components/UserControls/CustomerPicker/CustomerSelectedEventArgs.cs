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

namespace ECCentral.Portal.Basic.Components.UserControls.CustomerPicker
{
    public class CustomerSelectedEventArgs:EventArgs
    {
        public CustomerSelectedEventArgs(CustomerVM selectedCustomer)
        {
            this.SelectedCustomer = selectedCustomer;
        }

        public CustomerVM SelectedCustomer { get; private set; }
    }
}
