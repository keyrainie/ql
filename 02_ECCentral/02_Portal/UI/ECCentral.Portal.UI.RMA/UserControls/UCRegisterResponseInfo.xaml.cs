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
    public partial class URegisterResponseInfo : UserControl
    {
        public URegisterResponseInfo()
        {
            InitializeComponent();
        }

        private void btnUpdateResponseInfo_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                RegisterVM vm = new RegisterVM { ResponseInfo = this.DataContext as RegisterResponseVM };
                new RegisterFacade(CPApplication.Current.CurrentPage).UpdateResponseInfo(vm, (obj, args) =>
                {
                    vm.ResponseInfo.ResponseTime = args.Result.ResponseInfo.ResponseTime;
                    vm.ResponseInfo.ResponseUserName = args.Result.ResponseInfo.ResponseUserName;
                    vm.ResponseInfo.ResponseDesc = args.Result.ResponseInfo.ResponseDesc;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
                });                     
            }
        }

        public void SetButtonStatus()
        {            
            btnUpdateResponseInfo.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_CanUpdateResponseInfo);
        }
    }
}