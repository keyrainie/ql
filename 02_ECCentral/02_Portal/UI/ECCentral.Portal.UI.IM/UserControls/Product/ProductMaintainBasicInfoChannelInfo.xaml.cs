using System.Windows.Controls;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainBasicInfoChannelInfo : UserControl
    {
        public ProductMaintainBasicInfoChannelInfoVM VM
        {
            get { return DataContext as ProductMaintainBasicInfoChannelInfoVM; }
        }

        public ProductMaintainBasicInfoChannelInfo()
        {
            InitializeComponent();
        }
    }
}
