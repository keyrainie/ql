using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCSaleIncomeEdit : PopWindow
    {
        private SaleIncomeVM m_CurSaleIncome;
        private SaleIncomeFacade m_SaleIncomeFacade;

        public enum ActionType
        {
            /// <summary>
            /// 设置凭证号
            /// </summary>
            SetReferenceID,
            /// <summary>
            /// 设置收款单金额
            /// </summary>
            SetInvoiceAmt
        }

        private ActionType m_ActionType;
        private ActionType CurrentActionType
        {
            set
            {
                m_ActionType = value;
                if (value == ActionType.SetReferenceID)
                {
                    tbIncomeAmt.IsEnabled = false;
                    tbReferenceID.IsEnabled = true;
                }
                else
                {
                    tbIncomeAmt.IsEnabled = true;
                    tbReferenceID.IsEnabled = false;
                }
            }
            get
            {
                return m_ActionType;
            }
        }

        public UCSaleIncomeEdit()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCSaleIncomeEdit_Loaded);
        }

        private void UCSaleIncomeEdit_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCSaleIncomeEdit_Loaded);
            m_SaleIncomeFacade = new SaleIncomeFacade(CurrentPage);
        }

        public UCSaleIncomeEdit(SaleIncomeVM saleIncomeInfoVM, ActionType actionType)
            : this()
        {
            this.CurrentActionType = actionType;

            this.m_CurSaleIncome = saleIncomeInfoVM;
            this.LayoutRoot.DataContext = saleIncomeInfoVM;
            this.m_CurSaleIncome.ValidationErrors.Clear();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.LayoutRoot);
            if (flag)
            {
                var sysNoList = new List<int>(new int[] { m_CurSaleIncome.SysNo.Value });
                if (CurrentActionType == ActionType.SetReferenceID)
                {
                    m_SaleIncomeFacade.BatchSetReferenceID(sysNoList, m_CurSaleIncome.ReferenceID,
                        msg => CloseDialog(DialogResultType.OK));
                }
                else
                {
                    m_SaleIncomeFacade.SetIncomeAmount(m_CurSaleIncome.SysNo.Value, m_CurSaleIncome.IncomeAmt.Value,
                        () => CloseDialog(DialogResultType.OK));
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
    }
}