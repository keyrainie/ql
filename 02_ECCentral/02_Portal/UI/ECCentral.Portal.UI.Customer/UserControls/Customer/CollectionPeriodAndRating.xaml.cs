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
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CollectionPeriodAndRating : UserControl
    {
        private CustomerVM vm;
        public CollectionPeriodAndRating()
        {
            InitializeComponent();
        }

        private void btnSetPayDaysAndLimit_Click(object sender, RoutedEventArgs e)
        {
            txtAccountPeriodDays.ClearValidationError();
            txtTotalCreditLimit.ClearValidationError();
            vm = this.DataContext as CustomerVM;

            vm.AccountPeriodInfo.CustomerID = vm.BasicInfo.CustomerID;
            if (vm.AccountPeriodInfo.AccountPeriodDays.Value < 0)
            {
                txtAccountPeriodDays.Validation("账期天数不能小于0");

                return;
            }
            if (vm.AccountPeriodInfo.TotalCreditLimit.Value < 0)
            {
                txtTotalCreditLimit.Validation("账期额度不能小于0");
                return;
            }
            if (!ValidateRight())
                return;

            new CustomerFacade().SetCollectionPeriodAndRating(vm.AccountPeriodInfo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCollectionPeriodAndRating.Msg_SaveOk);
            });
        }
        private bool ValidateRight()
        {
            if (vm.AccountPeriodInfo.TotalCreditLimit >= 10000
               && !AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_AccountPerid_SetDaysMoney_OverTenThousand))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCollectionPeriodAndRating.rightMsg_NoRight_SetDaysMoney_OverTenThousand);
                return false;
            }
            else if (vm.AccountPeriodInfo.TotalCreditLimit < 10000
                && !AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_AccountPerid_SetDaysMoney_OverTenThousand)
                && !AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_AccountPerid_SetDaysMoney_SetAccountDaysMoney)
                )
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCollectionPeriodAndRating.rightMsg_NoRight_etDaysMoney_SetAccountDaysMoney);
                return false;
            }
            return true;
        }
    }
}
