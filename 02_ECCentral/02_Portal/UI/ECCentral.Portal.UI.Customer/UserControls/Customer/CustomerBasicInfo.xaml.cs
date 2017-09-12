using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Resources;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using System;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CustomerBasicInfo : UserControl
    {
        public CustomerBasicVM viewModel
        {
            get
            {
                return this.DataContext as CustomerBasicVM;
            }
        }

        public CustomerBasicInfo()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(CustomerBasicInfo_Loaded);
        }

        void CustomerBasicInfo_Loaded(object sender, RoutedEventArgs e)
        {
            cmbBadCustomer.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_MaintainMaliceUser);
            cmbStatus.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_Abandon);
        }

        private void cmbBadCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0 && viewModel.IsEdit)
            {
                this.viewModel.BadCustomerMemoVisible = viewModel.IsBadCustomer != viewModel.OriginalIsBadCustomer ? System.Windows.Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void btnMaintainMaliceUser_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_MaintainMaliceUser))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerBasicInfo.rightMsg_NoRight_MaintainMaliceUser);
                return;
            }
            CustomerBasicVM vm = this.DataContext as CustomerBasicVM;
            bool flag = ValidationManager.Validate(this.gridBadCustomer);
            string path = txtMemo.GetBindingExpression(TextBox.TextProperty).ParentBinding.Path.Path;
            var error = vm.ValidationErrors.FirstOrDefault(p => p.MemberNames.Contains(path));
            if (error == null && vm.ValidationErrors.Count > 0)
            {
                vm.ValidationErrors.Clear();
            }
            if (flag)
            {
                new CustomerFacade().MaintainMaliceUser(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Refresh();
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerMaintain.Info_SaveSuccessfully);
                });
            }
        }

        private void cmbSociety_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbSociety.SelectedItem != null)
            {
                var customerVM = this.DataContext as CustomerBasicVM;
                if (customerVM != null)
                {
                    customerVM.SocietyID = Convert.ToInt32(this.cmbSociety.SelectedValue);
                }
            }
        }


    }
}
