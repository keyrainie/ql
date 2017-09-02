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
    public partial class ShippingAddressInfo : UserControl
    {
        public ShippingAddressInfo()
        {
            InitializeComponent();
        }
        public event EventHandler OnShipingAddressUpdated;
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            ShippingAddressVM vm = (sender as HyperlinkButton).DataContext as ShippingAddressVM;
            ShippingAddressInfoDetail shipping = new ShippingAddressInfoDetail(vm);
            shipping.SetAllReadOnlyOrEnable();
            var window = CPApplication.Current.CurrentPage.Context.Window;
            IDialog dialog = window.ShowDialog(ResCustomerMaintain.Title_ShippingAddress, shipping, (obj, args) =>
            {
            });
            shipping.Dialog = dialog;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_AddressInfoEdit))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResShippingAddress.Msg_NoRights_Edit);
                return;
            }
            ShippingAddressVM vm = (sender as HyperlinkButton).DataContext as ShippingAddressVM;
            ShippingAddressInfoDetail shipping = new ShippingAddressInfoDetail(vm);
            var window = CPApplication.Current.CurrentPage.Context.Window;
            IDialog dialog = window.ShowDialog(ResCustomerMaintain.Title_ShippingAddress, shipping, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    OnShipingAddressUpdated(this, null);
                }
            });
            shipping.Dialog = dialog;
        }
    }

}
