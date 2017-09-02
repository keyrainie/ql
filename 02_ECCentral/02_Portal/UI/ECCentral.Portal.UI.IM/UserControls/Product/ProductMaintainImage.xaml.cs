using System.Windows;
using System.Windows.Controls;
using System.Linq;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Interface;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Portal.UI.IM.Views;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainImage : UserControl, ISave
    {

        public ProductMaintainImageVM VM
        {
            get { return DataContext as ProductMaintainImageVM; }
        }

        public ProductMaintainImage()
        {
            InitializeComponent();
        }

        private void ChbSelectAllClick(object sender, RoutedEventArgs e)
        {
            if (chbSelectAll.IsChecked.HasValue)
            {
                if (chbSelectAll.IsChecked.Value)
                {
                    foreach (var image in VM.ProductImageList)
                    {
                        image.IsShow = ProductResourceIsShow.Yes;
                    }
                }
                else
                {
                    foreach (var image in VM.ProductImageList)
                    {
                        image.IsShow = ProductResourceIsShow.No;
                    }
                }
            }
        }

        public void Save()
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (mainPage != null && mainPage is ProductMaintain)
            {
                if (VM.ProductImageList.All(image => image.IsShow != ProductResourceIsShow.Yes))
                {
                    mainPage.Context.Window.Alert("至少有一张图片前台显示！", MessageType.Error);
                    return;
                }

                new ProductFacade().UpdateProductImageInfo(
                    (mainPage as ProductMaintain).VM,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        mainPage.Context.Window.MessageBox.Show("商品图片信息更新成功", MessageBoxType.Success);
                        VM.ImageCount =
                            VM.ProductImageList.Count(image => image.IsShow == ProductResourceIsShow.Yes);
                    });
            }
        }
    }
}
