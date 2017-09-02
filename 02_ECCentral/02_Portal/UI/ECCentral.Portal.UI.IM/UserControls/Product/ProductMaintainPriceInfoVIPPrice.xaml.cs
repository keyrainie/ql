using System.Windows.Controls;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainPriceInfoVIPPrice : UserControl
    {
        public ProductMaintainPriceInfoVIPPriceVM VM
        {
            get { return DataContext as ProductMaintainPriceInfoVIPPriceVM; }
        }

        public ProductMaintainPriceInfoVIPPrice()
        {
            InitializeComponent();
        }
    }
}
