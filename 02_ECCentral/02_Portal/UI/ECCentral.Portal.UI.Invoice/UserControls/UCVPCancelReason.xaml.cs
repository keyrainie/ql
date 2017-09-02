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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.Portal.UI.Invoice.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCVPCancelReason : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }
        public VPCancelReasonVM vm;
         
        public UCVPCancelReason(List<int> sysNoList)
        {
            this.DataContext = vm = new VPCancelReasonVM();
            vm.SysNoList = sysNoList;
            InitializeComponent();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
            this.Dialog.Close(true);
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.LayoutRoot);
            if (vm.HasValidationErrors)
            {
                return;
            }
            APInvoiceBatchUpdateReq request = new APInvoiceBatchUpdateReq();
            request.SysNoList = vm.SysNoList;
            request.VPCancelReason = vm.VPCancelReason;

            new InvoiceInputFacade(CPApplication.Current.CurrentPage).BatchVPCancel(request, (obj, args) =>
            {
                if (Dialog != null)
                {
                    Dialog.ResultArgs.Data = args;
                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    Dialog.Close();
                }
            });
        }
    }
}
