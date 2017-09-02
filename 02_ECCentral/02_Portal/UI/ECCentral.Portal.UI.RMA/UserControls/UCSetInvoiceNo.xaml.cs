using System.Windows;
using System.Windows.Controls;

using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCSetInvoiceNo : UserControl
    {
        public IDialog Dialog { get; set; }

        public UCSetInvoiceNo()
        {
            InitializeComponent();
        }      

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {            
            this.Dialog.Close();
        }
    }
}
