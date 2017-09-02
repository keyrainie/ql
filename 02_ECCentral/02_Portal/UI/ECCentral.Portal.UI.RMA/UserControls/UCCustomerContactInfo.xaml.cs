using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;


namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCCustomerContactInfo : UserControl
    {        
        public UCCustomerContactInfo()
        {
            InitializeComponent();
        }

        private void btnViewContactInfo_Click(object sender, RoutedEventArgs e)
        {
            var register = this.DataContext as RegisterVM;           
            UCCustomerContactPop uc = new UCCustomerContactPop();
            uc.ContactInfo = register.OriginContactInfo;
            if (register.OriginContactInfo == null)
            {
                uc.ContactInfo = new CustomerContactVM { IsPop = true };
            }
            IDialog dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResRegisterMaintain.PopTitle_CustomerContact, uc, (obj, args) =>
            {

            });
            uc.Dialog = dialog;
        }

        public ComboBox RefundPayTypes { get { return cmbRefundPayType; } }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {            
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                RegisterVM register = this.DataContext as RegisterVM;
                var vm = (sender as Button).DataContext as CustomerContactVM;
                vm.RMARequestSysno = register.BasicInfo.RequestSysNo;
                CustomerContactFacade facade = new CustomerContactFacade(CPApplication.Current.CurrentPage);
                if (vm.SysNo.HasValue && vm.SysNo > 0)
                {
                    facade.Update(vm, (obj, args) =>
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
                    });
                }
                else
                {
                    facade.Create(vm, (obj, args) =>
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
                        vm.SysNo = args.Result.SysNo;
                    });
                }
            }
        }

        private void cmbRefundPayType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.gridRefund.Visibility = cmbRefundPayType.SelectedIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
