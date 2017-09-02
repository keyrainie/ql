using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using System;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class ValueAddedTaxInvoice : UserControl
    {
        public ValueAddedTaxInvoice()
        {
            InitializeComponent();
        }
        public event EventHandler OnVATUpdated;
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            ValueAddedTaxInfoVM vm = (sender as HyperlinkButton).DataContext as ValueAddedTaxInfoVM;
            ValueAddedTaxInvoiceDetail vat = new ValueAddedTaxInvoiceDetail(vm);
            vat.SetAllReadOnlyOrEnable();
            var window = CPApplication.Current.CurrentPage.Context.Window;
            IDialog dialog = window.ShowDialog(ResCustomerMaintain.Title_ValueAddedTaxInfo, vat, (obj, args) =>
            {
            });
            vat.Dialog = dialog;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_TaxInfoEdit))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResValueAddedTaxInfo.Msg_NoRights_Edit);
                return;
            }
            ValueAddedTaxInfoVM vm = (sender as HyperlinkButton).DataContext as ValueAddedTaxInfoVM;
            ValueAddedTaxInvoiceDetail vat = new ValueAddedTaxInvoiceDetail(vm);
            var window = CPApplication.Current.CurrentPage.Context.Window;
            IDialog dialog = window.ShowDialog(ResCustomerMaintain.Title_ValueAddedTaxInfo, vat, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    OnVATUpdated(this, null);
                }
            });
            vat.Dialog = dialog;
        }


    }
}
