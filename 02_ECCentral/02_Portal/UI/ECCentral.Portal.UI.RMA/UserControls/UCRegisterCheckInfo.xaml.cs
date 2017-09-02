using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.Resources;
using ECCentral.Portal.Basic;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCRegisterCheckInfo : UserControl
    {
        public UCRegisterCheckInfo()
        {
            InitializeComponent();            
        }

        private void btnUpdateCheckInfo_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                RegisterVM vm = new RegisterVM { CheckInfo = this.DataContext as RegisterCheckVM };            
                new RegisterFacade(CPApplication.Current.CurrentPage).UpdateCheckInfo(vm, (obj, args) =>
                {
                    vm.CheckInfo.CheckTime = args.Result.CheckInfo.CheckTime;
                    vm.CheckInfo.CheckUserName = args.Result.CheckInfo.CheckUserName;
                    vm.CheckInfo.CheckDesc = args.Result.CheckInfo.CheckDesc;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
                });                
            }
        }

        public void SetButtonStatus()
        {
            btnUpdateCheckInfo.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_CanUpdateCheckInfo);            
        }
    }
}
