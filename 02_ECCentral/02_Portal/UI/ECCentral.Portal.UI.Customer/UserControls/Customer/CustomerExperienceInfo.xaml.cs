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

using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CustomerExperienceInfo : UserControl
    {
        public CustomerExperienceInfo()
        {
            InitializeComponent();
        }

        private void btnAdjustTotalSOMoney_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_Experience_ManualAdd))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResExperienceValueInfo.rightMsg_NoRight_ManualAdd);
                return;
            }
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                ExperienceVM vm = this.DataContext as ExperienceVM;
                vm.Type = ExperienceLogType.ManualSetTotalSOMoney;
                int? amount = 0;
                try
                {
                    amount = int.Parse(vm.Amount);
                    vm.Amount = amount.ToString();
                }
                catch
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerMaintain.Info_ExperienceFailed);
                    return;
                }
                new CustomerFacade().ManualAdjustUserExperience(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    vm.TotalSOMoney = vm.TotalSOMoney + int.Parse(vm.Amount);
                    vm.Memo = string.Empty;
                    vm.ValidationErrors.Clear();
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerMaintain.Info_AdjustSuccessfully);
                    CPApplication.Current.CurrentPage.Context.Window.Refresh();
                });
            }
        }
    }
}
