using System;
using System.Collections.Generic;
using ECCentral.Portal.UI.Invoice.NeweggCN.Facades;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using ECCentral.Portal.UI.Invoice.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.UserControls
{
    public partial class UCBalanceRefundReferenceIDSetter : PopWindow
    {
        private BalanceRefundVM _refundVM;
        private BalanceRefundFacade _facade;

        public UCBalanceRefundReferenceIDSetter()
        {
            InitializeComponent();
            Loaded += new System.Windows.RoutedEventHandler(BalanceRefundReferenceIDSetter_Loaded);
        }

        private void BalanceRefundReferenceIDSetter_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Loaded -= new System.Windows.RoutedEventHandler(BalanceRefundReferenceIDSetter_Loaded);
            this.LayoutRoot.DataContext = _refundVM;
        }

        public UCBalanceRefundReferenceIDSetter(BalanceRefundVM refundVM, BalanceRefundFacade facade)
            : this()
        {
            _refundVM = refundVM;
            _facade = facade;
        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.LayoutRoot);
            if (flag)
            {
                _facade.BatchSetReferenceID(new List<int>() { _refundVM.SysNo.Value }, _refundVM.ReferenceID, (msg) => CloseDialog(DialogResultType.OK));
            }
        }

        private void btnClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CloseDialog();
        }
    }
}