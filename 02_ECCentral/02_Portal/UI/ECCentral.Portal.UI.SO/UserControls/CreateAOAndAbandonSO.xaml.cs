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
using ECCentral.Portal.UI.SO.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.SO.Facades;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class CreateAOAndAbandonSO : UserControl
    {
        string SZYTPrePayCardPayTypesString = string.Empty;
        public int SOSysNo
        {
            get;
            set;
        }
        public int? PayTypeSysNo { get; set; }
        private RefundInfoVM _refundVM;
        private RefundInfoVM RefundVM
        {
            get { return _refundVM; }
            set
            {
                _refundVM = value;
                this.DataContext = _refundVM;
            }
        }
        public IPage Page
        {
            get;
            set;
        }

        public IDialog Dialog
        {
            get;
            set;
        }
        public ECCentral.BizEntity.Invoice.SOIncomeInfo CurrentSOIncomeInfo { get; set; }

        public CreateAOAndAbandonSO()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(CreateAOAndAbandonSO_Loaded);
        }

        void CreateAOAndAbandonSO_Loaded(object sender, RoutedEventArgs e)
        {
            RefundVM = new RefundInfoVM
            {
                RefundPayType = BizEntity.Invoice.RefundPayType.NetWorkRefund
            };
            new OtherDomainQueryFacade(Page).GetRefundReaons((reasonList) =>
            {
                cbReason.ItemsSource = reasonList;
            });

            btnOK.IsEnabled = CurrentSOIncomeInfo != null;
            if (btnOK.IsEnabled)
            {
                RefundVM.RefundCashAmt = CurrentSOIncomeInfo.OrderAmt.Value.ToString("N2");
            }          
        }

        private void SetControlsIsReadonly()
        {
            txtBankName.IsReadOnly = true;
            txtBranchBankName.IsReadOnly = true;
            txtCardNumber.IsReadOnly = true;
            txtCardOwnerName.IsReadOnly = true;
            txtPostAddress.IsReadOnly = true;
            txtPostCode.IsReadOnly = true;
            txtCashReceiver.IsReadOnly = true;
            if (RefundVM.RefundPayType.HasValue)
            {
                switch (RefundVM.RefundPayType.Value)
                {
                    case BizEntity.Invoice.RefundPayType.BankRefund:

                        txtBankName.IsReadOnly = false;
                        txtBranchBankName.IsReadOnly = false;
                        txtCardNumber.IsReadOnly = false;
                        txtCardOwnerName.IsReadOnly = false;
                        break;
                    case BizEntity.Invoice.RefundPayType.PostRefund:
                        txtPostAddress.IsReadOnly = false;
                        txtPostCode.IsReadOnly = false;
                        txtCashReceiver.IsReadOnly = false;
                        break;
                    case BizEntity.Invoice.RefundPayType.NetWorkRefund:
                        txtBankName.IsReadOnly = false;
                        break;
                }
            }
        }

        private void RefundPayType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetControlsIsReadonly();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Page.Context.Window.Confirm(ResSO.Msg_ConfirmVoidOrder, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    new SOFacade(Page).CreateAOAndAbandonSO(SOSysNo, cbIsReturnInventory.IsChecked.Value, RefundVM, (vm) =>
                    {
                        if (Saved != null)
                        {
                            Saved(vm);
                        }
                    });
                    if (Dialog != null)
                    {
                        Dialog.Close();
                    }
                }
            });
        }

        public event Action<SOVM> Saved;


    }
}
