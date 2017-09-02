using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Portal.UI.IM.Views;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainPriceInfoAutoPrice : UserControl
    {
        public ProductMaintainPriceInfoAutoPriceVM VM
        {
            get { return DataContext as ProductMaintainPriceInfoAutoPriceVM; }
        }

        public ProductMaintainPriceInfoAutoPrice()
        {
            InitializeComponent();
        }

        private void ChbIsNotAutoAdjustPriceClick(object sender, RoutedEventArgs e)
        {
            VM.NotAutoAdjustPriceShow = VM.IsAutoAdjustPrice == IsAutoAdjustPrice.Yes ? "Collapsed" : "Visible";
        }

        private void BtnAutoAdjustPriceSaveClick(object sender, RoutedEventArgs e)
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (mainPage != null && mainPage is ProductMaintain)

                new ProductFacade().UpdateProductAutoPriceInfo(
                    (mainPage as ProductMaintain).VM,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        mainPage.Context.Window.MessageBox.Show("商品自动调价信息更新成功", MessageBoxType.Success);
                    });
        }
    }
}
