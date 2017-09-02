using System.Windows.Controls;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainPriceInfoRankPrice : UserControl
    {
        public ProductMaintainPriceInfoRankPriceVM VM
        {
            get { return DataContext as ProductMaintainPriceInfoRankPriceVM; }
        }

        public ProductMaintainPriceInfoRankPrice()
        {
            InitializeComponent();
        }
    }
}
