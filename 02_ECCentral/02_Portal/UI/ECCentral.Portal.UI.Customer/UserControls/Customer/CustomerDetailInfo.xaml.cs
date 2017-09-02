using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.Basic;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Customer.Resources;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CustomerDetailInfo : UserControl
    {
        public CustomerDetailInfo()
        {
            InitializeComponent();
        }

        private void btnViewRecommendedUser_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CommendInfo))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerDetailInfo.Msg_NoRights_View);
                return;
            }
            CustomerBasicVM vm = this.DataContext as CustomerBasicVM;
            string url = string.Format(ConstValue.CustomerMaintainUrlFormat, vm.RecommendedByCustomerSysNo);
            CPApplication.Current.CurrentPage.Context.Window.Navigate(url, null, true);
        }
    }
}
