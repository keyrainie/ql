using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.Basic;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.Resources;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCRegisterBasicInfo : UserControl
    {        
        public UCRegisterBasicInfo()
        {
            InitializeComponent();            
        }       

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            RegisterVM vm = new RegisterVM { BasicInfo = btn.DataContext as RegisterBasicVM };            
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                new RegisterFacade(CPApplication.Current.CurrentPage).UpdateBasicInfo(vm, (obj, args) =>
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
                });
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var vm = btn.DataContext as RegisterBasicVM;                     
            var para = new Dictionary<string, string>();
            para.Add("SysNo", vm.SysNo.ToString());
            HtmlViewHelper.WebPrintPreview(ConstValue.DomainName_RMA, "PrintRegister", para);           
        }

        private void btnViewDunLog_Click(object sender, RoutedEventArgs e)
        {
            UCRegisterDunLog uc = new UCRegisterDunLog();
            uc.DataContext = this.Tag;

            IDialog dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResRegisterMaintain.PopTitle_Dunlog, uc, (obj, args) =>
            {

            });
            uc.Dialog = dialog;
        }

        private void cmbNextHandler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbNextHandler.SelectedValue != null)
            {
                RegisterVM vm = this.Tag as RegisterVM;
                ComboBox cmb = sender as ComboBox;
                var kv = (KeyValuePair<RMANextHandler?,string>)cmb.SelectedValue;
                if (kv.Key == RMANextHandler.Dun)
                {
                    btnViewDunLog.Visibility = System.Windows.Visibility.Visible;
                    new RegisterFacade(CPApplication.Current.CurrentPage).LoadRegisterDunLog(vm.SysNo.Value, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        vm.DunLogs = args.Result;                        
                    });
                }
            }
        }

        public void SetButtonStatus()
        {
            RegisterVM vm = this.Tag as RegisterVM;
            btnUpdate.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_CanUpdate);
            //btnPrint.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_can);
        }
    }
}
