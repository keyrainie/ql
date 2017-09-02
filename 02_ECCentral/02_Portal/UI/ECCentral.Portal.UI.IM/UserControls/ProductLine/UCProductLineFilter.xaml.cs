using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class UCProductLineFilter : UserControl
    {
        public UCProductLineFilter()
        {
            InitializeComponent();
        }

        private void chkEmptyCategory_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            this.conditionContainer.IsEnabled = !chk.IsChecked.Value;            
        }
    }
}
