using System.Windows.Controls;

using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainProductImageSingle : UserControl
    {
        public ProductMaintainProductImageSingleVM VM
        {
            get { return DataContext as ProductMaintainProductImageSingleVM; }
            set { DataContext = value; }
        }

        public ProductMaintainProductImageSingle()
        {
            InitializeComponent();
        }
    }
}
