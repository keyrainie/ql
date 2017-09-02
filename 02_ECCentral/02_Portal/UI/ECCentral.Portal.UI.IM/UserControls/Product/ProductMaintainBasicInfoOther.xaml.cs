using System.Windows.Controls;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;

namespace ECCentral.Portal.UI.IM.UserControls.Product
{

    public partial class ProductMaintainBasicInfoOther : UserControl
    {
        public ProductMaintainBasicInfoOtherVM VM
        {
            get { return DataContext as ProductMaintainBasicInfoOtherVM; }
        }

        public ProductMaintainBasicInfoOther()
        {
            InitializeComponent();
        }
    }
}
