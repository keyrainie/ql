using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.UI.RMA.Models;

using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCCustomerContactPop : UserControl
    {
        public IDialog Dialog { get; set; }
        public CustomerContactVM ContactInfo { get; set; }

        public UCCustomerContactPop()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(CustomerContactPop_Loaded);
        }        

        void CustomerContactPop_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(CustomerContactPop_Loaded);
            if (this.ContactInfo != null)
            {
                this.ContactInfo.IsPop = true;
            }
            this.DataContext = this;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {            
            this.Dialog.Close();
        }
    }
}
