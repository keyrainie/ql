using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.Views;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.UI.RMA.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCRegisterRevertInfo : UserControl
    {
        public RegisterMaintain Page { get; set; }
        /// <summary>
        /// 是否完成数据加载：用于加载数据时，屏蔽事件中的清空值操作
        /// </summary>
        public bool IsLoadCompleted = false;

        public UCRegisterRevertInfo()
        {
            InitializeComponent();
        }

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            var revertVM = this.DataContext as RegisterRevertVM;
            var vm = new RegisterVM { SysNo = revertVM.SysNo, RevertInfo = revertVM };
            if (vm.RevertInfo.IsApproved)
            {
                new RegisterFacade(this.Page).ApproveRevertAudit(vm, (obj, args) =>
                {
                    revertVM.RevertStatus = args.Result.RevertInfo.RevertStatus;
                    revertVM.RevertAuditUserName = args.Result.RevertInfo.RevertAuditUserName;
                    revertVM.RevertAuditTime = args.Result.RevertInfo.RevertAuditTime;

                    this.Page.SetButtonStatus();

                    this.Page.Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
                });
            }
            else
            {
                new RegisterFacade(this.Page).RejectRevertAudit(vm, (obj, args) =>
                {
                    revertVM.RevertStatus = args.Result.RevertInfo.RevertStatus;
                    revertVM.RevertAuditUserName = args.Result.RevertInfo.RevertAuditUserName;
                    revertVM.RevertAuditTime = args.Result.RevertInfo.RevertAuditTime;

                    this.Page.SetButtonStatus();
                    this.Page.Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
                });
            }
        }

        private void cmbNewProductStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            var vm = (sender as ComboBox).DataContext as RegisterRevertVM;
            if (vm != null)
            {
                txtRevertProductID.Visibility = vm.NewProductStatus == RMANewProductStatus.OtherProduct ? Visibility.Collapsed : Visibility.Visible;
                ucPPRevertProductID.Visibility = vm.NewProductStatus == RMANewProductStatus.OtherProduct ? Visibility.Visible : Visibility.Collapsed;
                if (vm.RevertStatus == null)
                {
                    cmbStock.IsEnabled = vm.NewProductStatus != RMANewProductStatus.Origin;
                    cmbSecondHand.IsEnabled = vm.NewProductStatus == RMANewProductStatus.SecondHand;

                    ucPPRevertProductID.IsEnabled = vm.NewProductStatus == RMANewProductStatus.OtherProduct;
                    if (vm.NewProductStatus == RMANewProductStatus.NewProduct || vm.NewProductStatus == RMANewProductStatus.SecondHand)
                    {
                        if (IsLoadCompleted)
                        {
                            vm.RevertStockSysNo = 51;
                            vm.RevertProductSysNo = null;
                            vm.RevertProductID = null;
                        }
                        cmbStock.IsEnabled = false;
                    }
                    else
                    {
                        if (IsLoadCompleted)
                        {
                            vm.RevertStockSysNo = null;
                            vm.RevertProductSysNo = null;
                            vm.RevertProductID = null;
                        }
                        cmbStock.IsEnabled = true;
                    }
                }
            }
        }

        public void SetButtonStatus()
        {
            RegisterRevertVM vm = this.DataContext as RegisterRevertVM;
            btnAudit.IsEnabled = vm.RevertStatus == RMARevertStatus.WaitingAudit
                && AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_CanRevertAudit);
        }
    }
}
