using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.UI.Invoice.NeweggCN.Facades;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using ECCentral.Portal.UI.Invoice.UserControls;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.UserControls
{
    public partial class UCBalanceRefundMaintain : PopWindow
    {
        private BalanceRefundVM _refundVM;
        private BalanceRefundFacade _facade;
        private string _mode;

        public UCBalanceRefundMaintain()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(BalanceRefundEditor_Loaded);
        }

        public UCBalanceRefundMaintain(BalanceRefundVM refundVM)
            : this()
        {
            _refundVM = refundVM;
            _mode = "View";
            this.BaseInfo.SetChildControlAvailably(false);
            this.btnSave.Visibility = Visibility.Collapsed;
        }

        public UCBalanceRefundMaintain(BalanceRefundVM refundVM, BalanceRefundFacade facade)
            : this()
        {
            _refundVM = refundVM;
            _facade = facade;
            _mode = "Edit";
            this.BaseInfo.SetChildControlAvailably(false, new List<UIElement>
            {
                cmbRefundType,tbNote
            });
            this.btnSave.Visibility = Visibility.Visible;
        }

        private void BalanceRefundEditor_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(BalanceRefundEditor_Loaded);

            this.LayoutRoot.DataContext = _refundVM;
            _refundVM.ValidationErrors.Clear();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.LayoutRoot);
            if (flag && _facade != null)
            {
                _facade.Update(_refundVM, () => CloseDialog(DialogResultType.OK));
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog();
        }

        private void cmbRefundType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_mode == "Edit" && e.AddedItems.Count > 0)
            {
                KeyValuePair<RefundPayType?, string> selectedVal = (KeyValuePair<RefundPayType?, string>)e.AddedItems[0];
                if (selectedVal.Key.HasValue)
                {
                    if (selectedVal.Key.Value == RefundPayType.BankRefund)
                    {
                        this.BaseInfo.SetChildControlAvailably(false, new List<UIElement>
                        {
                            cmbRefundType,tbNote,tbBankName,tbBranchBankName,tbCardNumber,tbCardOwnerName
                        });
                    }
                    else if (selectedVal.Key.Value == RefundPayType.PostRefund)
                    {
                        this.BaseInfo.SetChildControlAvailably(false, new List<UIElement>
                        {
                            cmbRefundType,tbNote,tbPostAddress,tbPostCode,tbReceiverName
                        });
                    }
                }
            }
        }
    }
}