using System.Windows.Controls;
using ECCentral.Portal.UI.IM.Models;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductGroupMaintainPropertySetting : UserControl
    {
        public ProductGroupMaintainPropertySettingVM VM
        {
            get { return DataContext as ProductGroupMaintainPropertySettingVM; }
        }


        public ProductGroupMaintainPropertySetting()
        {
            InitializeComponent();
        }

        private void CmbGroupPropertyList1SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var m = cmbGroupPropertyList1.SelectedItem;
            if (m != null)
            {
                var protype = m.GetType().GetProperty("PropertyName");
                var value = (string)protype.GetValue(m, null);
                string name=value.IndexOf("_") != -1
                                                                   ? value.Substring(value.IndexOf("_") + 1)
                                                                   : value;
                VM.ProductGroupSettings[0].PropertyBriefName = name;
                VM.ProductGroupSettings[0].ProductGroupProperty.PropertyName = name;
            }
        }

        private void CmbGroupPropertyList2SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var m = cmbGroupPropertyList2.SelectedItem;
            if (m != null)
            {
                var protype = m.GetType().GetProperty("PropertyName");
                var value = (string)protype.GetValue(m, null);
                string name = value.IndexOf("_") != -1 ? value.Substring(value.IndexOf("_") + 1) : value;
                VM.ProductGroupSettings[1].PropertyBriefName = name;
                VM.ProductGroupSettings[1].ProductGroupProperty.PropertyName = name;
            }
        }
    }
}
