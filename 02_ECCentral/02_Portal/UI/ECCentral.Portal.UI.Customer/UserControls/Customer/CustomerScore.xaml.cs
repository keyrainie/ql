using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Customer.UserControls.PrePay;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CustomerScore : UserControl
    {
        public CustomerScore()
        {
            InitializeComponent();
        }

        private void btnAdustVIPRank_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_SetVIPRank))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerScoreInfo.rightMsg_NoRight_SetVIPRank);
                return;
            }
            ScoreVM vm = this.DataContext as ScoreVM;
            new CustomerFacade().ManaulSetVipRank(vm, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerMaintain.Info_AdjustSuccessfully);
            });
        }

        private void btnPrepayToBankAccount_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_PrepayToBank))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerScoreInfo.rigthMsg_NoRight_PrepayToBank);
                return;
            }
            ScoreVM vm = this.DataContext as ScoreVM;
            PrepayRefundVM prvm = new PrepayRefundVM();
            prvm.CustomerSysNo = vm.CustomerSysNo;
            prvm.CustomerName = vm.CustomerName;
            prvm.ReturnPrepayAmt = vm.ValidPrepayAmt;
            UCPrepayRefund uc = new UCPrepayRefund(prvm);
            uc.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResCustomerScoreInfo.title_CreateRefund, uc, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    vm.ValidPrepayAmt = 0;
                    this.DataContext = vm;
                }
            });

        }
    }
}
