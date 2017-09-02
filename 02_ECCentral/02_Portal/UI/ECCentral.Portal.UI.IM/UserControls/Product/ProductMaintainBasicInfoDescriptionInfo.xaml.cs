using System.Windows.Controls;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainBasicInfoDescriptionInfo : UserControl
    {
        public ProductMaintainBasicInfoDescriptionInfoVM VM
        {
            get { return DataContext as ProductMaintainBasicInfoDescriptionInfoVM; }
        }


        public ProductMaintainBasicInfoDescriptionInfo()
        {
            InitializeComponent();
        }
    }
}
