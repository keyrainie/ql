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
using ECCentral.Portal.UI.Invoice.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Invoice.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Invoice.Resources;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCInvoiceNoEdit : PopWindow
    {
        public UCInvoiceNoEdit()
        {
            InitializeComponent();
        }

        private InvoiceVM m_CurInvoice;
        private InvoiceFacade m_InvoiceFacade;

        public UCInvoiceNoEdit(InvoiceVM invoiceVM)
            : this()
        {
            m_CurInvoice = invoiceVM;
            this.m_InvoiceFacade = new InvoiceFacade(this.CurrentPage);
            this.LayoutRoot.DataContext = invoiceVM;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.LayoutRoot);
            if (flag)
            {
                if (string.IsNullOrEmpty(this.tbReferenceID.Text))
                {
                    this.tbReferenceID.Validation(ResInvoiceQuery.Message_NullReferenceID);
                    this.tbReferenceID.RaiseValidationError();
                    return;
                }
                m_InvoiceFacade.UpdateSOInvoice(m_CurInvoice.OrderSysNo.Value, m_CurInvoice.InvoiceNo, m_CurInvoice.StockID, CPApplication.Current.CompanyCode);
                CloseDialog(DialogResultType.OK);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
    }
}