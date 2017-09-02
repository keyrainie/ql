using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.RMA.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCRegisterStatusInfo : UserControl
    {
        public UCRegisterStatusInfo()
        {
            InitializeComponent();
        }

        private void btnRefundDetail_Click(object sender, RoutedEventArgs e)
        {
            var basic = this.DataContext as RegisterBasicVM;
            CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ConstValue.RMA_RefundMaintainUrl, basic.RefundSysNo), null, true);
        }
    }
}
