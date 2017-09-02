using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class AgentInfo : UserControl
    {
        public AgentInfo()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_AgentInfo_Edit))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResAgentInfo.rightmsg_NoRight);
                return;
            }
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                AgentInfoVM vm = this.DataContext as AgentInfoVM;
                CustomerFacade facade = new CustomerFacade();
                if (vm.TransactionNumber.HasValue && vm.TransactionNumber > 0)
                {
                    facade.UpdateAgent(vm, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerMaintain.Info_SaveSuccessfully);
                    });
                }
                else
                {
                    facade.CreateAgent(vm, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerMaintain.Info_SaveSuccessfully);
                    });
                }
            }
        }
    }
}
