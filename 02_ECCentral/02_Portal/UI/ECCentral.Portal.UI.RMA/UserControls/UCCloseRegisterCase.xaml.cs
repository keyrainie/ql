using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCCloseRegisterCase : UserControl
    {
        public IDialog Dialog { get; set; }

        public UCCloseRegisterCase()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCCloseRegisterCase_Loaded);            
        }

        void UCCloseRegisterCase_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCCloseRegisterCase_Loaded);
            var vm = this.DataContext as RegisterVM;
            //if (vm.SellerInfo.SellerOperationInfo == "Other")
            //{
            //    btnOK.IsEnabled = true;
            //}
            //else if (vm.SellerInfo.SellerOperationInfo == "Refund")
            //{
            //    if (vm.BasicInfo.ProcessType == BizEntity.RMA.ProcessType.MET && (vm.BasicInfo.InvoiceType.HasValue && vm.BasicInfo.InvoiceType == BizEntity.Invoice.InvoiceType.MET))
            //    {
            //        btnOK.IsEnabled = true;
            //    }
            //}
        }       

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as RegisterVM;
            new RegisterFacade(CPApplication.Current.CurrentPage).CloseCase(vm.SysNo.Value, (obj, args) =>
            {
                vm.BasicInfo.Status = args.Result.BasicInfo.Status;
                vm.BasicInfo.CloseTime = args.Result.BasicInfo.CloseTime;
                vm.BasicInfo.CloseUserName = args.Result.BasicInfo.CloseUserName;
                vm.BasicInfo.CloseUserSysNo = args.Result.BasicInfo.CloseUserSysNo;

                this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.OK };
                this.Dialog.Close();
            });
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.Cancel };
            this.Dialog.Close();
        }
    }
}
