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

using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.UI.RMA.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCRefundDetail : UserControl
    {
        public UCRefundDetail()
        {
            InitializeComponent();
        }

        private void btnUpdateFinanceNote_Click(object sender, RoutedEventArgs e)
        {
            RefundVM vm = this.DataContext as RefundVM;
            if (string.IsNullOrEmpty(vm.FinanceNote))
            {
                List<string> members = new List<string>() { "FinanceNote" };
                vm.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(ResRefundMaintain.Validation_RequiredField, members));
                txtFinanceNote.Focus();
                return;
            }
            new RefundFacade(CPApplication.Current.CurrentPage).UpdateFinanceNote(vm, (obj, args) =>
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResRefundMaintain.Info_OperateSuccessfully);
            });
        }

        private void cmbRefundPayType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefundVM vm = this.DataContext as RefundVM;
            if (vm.RefundPayType == null)
            {
                return;
            }
            vm.ValidationErrors.Clear();
            switch (vm.RefundPayType)
            {
                case RefundPayType.BankRefund:
                    txtBankName.IsReadOnly = false;
                    txtBranchBankName.IsReadOnly = false;
                    txtCardNo.IsReadOnly = false;
                    txtCardOwnerName.IsReadOnly = false;

                    txtPostAddress.IsReadOnly = true;
                    txtPostCode.IsReadOnly = true;
                    txtReceiverName.IsReadOnly = true;
                    break;
                case RefundPayType.PostRefund:
                    txtPostAddress.IsReadOnly = false;
                    txtPostCode.IsReadOnly = false;
                    txtReceiverName.IsReadOnly = false;

                    txtBankName.IsReadOnly = true;
                    txtBranchBankName.IsReadOnly = true;
                    txtCardNo.IsReadOnly = true;
                    txtCardOwnerName.IsReadOnly = true;

                    break;
                case RefundPayType.NetWorkRefund:
                    txtBankName.IsReadOnly = false;

                    txtBranchBankName.IsReadOnly = true;
                    txtCardNo.IsReadOnly = true;
                    txtCardOwnerName.IsReadOnly = true;
                    txtPostAddress.IsReadOnly = true;
                    txtPostCode.IsReadOnly = true;
                    txtReceiverName.IsReadOnly = true;
                    break;
                default:
                    txtBankName.IsReadOnly = true;
                    txtBranchBankName.IsReadOnly = true;
                    txtCardNo.IsReadOnly = true;
                    txtCardOwnerName.IsReadOnly = true;
                    txtPostAddress.IsReadOnly = true;
                    txtPostCode.IsReadOnly = true;
                    txtReceiverName.IsReadOnly = true;
                    break;
            }
        }

        private void btnGetShipFee_Click(object sender, RoutedEventArgs e)
        {
             RefundVM vm = this.DataContext as RefundVM;
             new RefundFacade(CPApplication.Current.CurrentPage).GetShipFee(vm, (obj, args) =>
             {
                 UCShipFeeDetail uc = new UCShipFeeDetail();
                 uc.DataContext = args.Result;
                 IDialog dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResRefundMaintain.PopTitle_ShipFeeDetail, uc);
                 uc.Dialog = dialog;
             });
        }
    }
}
