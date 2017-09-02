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
using ECCentral.Portal.UI.Customer.Facades;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Customer.UserControls.PrePay
{
    public partial class UCPrepayRefund : UserControl
    {
        private PrepayRefundVM _refundVM;
        public IDialog Dialog;

        public UCPrepayRefund()
        {
            InitializeComponent();
        }
        public UCPrepayRefund(PrepayRefundVM vm)
        {
            _refundVM = vm;
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.LayoutRoot.DataContext = _refundVM;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.LayoutRoot);
            if (!_refundVM.HasValidationErrors)
            {
                new OtherDomainQueryFacade(CPApplication.Current.CurrentPage).Create(_refundVM, (obj, args) =>
                 {
                     if (args.FaultsHandle())
                     {
                         return;
                     }
                     CPApplication.Current.CurrentPage.Context.Window.Alert("操作成功！");
                     Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                     Dialog.Close();
                 });
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            Dialog.Close();
        }

        private void cmbRefundType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                KeyValuePair<RefundPayType?, string> selectedVal = (KeyValuePair<RefundPayType?, string>)e.AddedItems[0];
                if (selectedVal.Key.HasValue)
                {
                    if (selectedVal.Key.Value == RefundPayType.BankRefund)
                    {
                        tbBankName.IsEnabled = true;
                        tbPostAddress.IsEnabled = false;
                        _refundVM.PostAddress = string.Empty;
                        _refundVM.PostCode = string.Empty;
                        _refundVM.ReceiverName = string.Empty;
                    }
                    else if (selectedVal.Key.Value == RefundPayType.PostRefund)
                    {
                        tbPostAddress.IsEnabled = true;
                        tbBankName.IsEnabled = false;
                        _refundVM.BankName = string.Empty;
                        _refundVM.BranchBankName = string.Empty;
                        _refundVM.CardNumber = string.Empty;
                        _refundVM.CardOwnerName = string.Empty;
                    }
                }
                else
                {
                    tbPostAddress.IsEnabled = false;
                    tbBankName.IsEnabled = false;
                }
                _refundVM.ValidationErrors.Clear();
            }
        }
    }
}
