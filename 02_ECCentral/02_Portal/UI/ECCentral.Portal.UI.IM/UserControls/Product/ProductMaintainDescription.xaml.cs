using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.Basic.Interface;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Portal.UI.IM.Views;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;


namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainDescription : UserControl, ISave, IBatchSave
    {
        public ProductMaintainDescriptionVM VM
        {
            get { return DataContext as ProductMaintainDescriptionVM; }
        }

        public ProductMaintainDescription()
        {
            InitializeComponent();
        }

        public void Save()
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (mainPage != null && mainPage is ProductMaintain)

                new ProductFacade().UpdateProductDescriptionInfo(
                    (mainPage as ProductMaintain).VM,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        mainPage.Context.Window.MessageBox.Show("商品详细描述信息更新成功", MessageBoxType.Success);
                    });

        }

        public void BatchSave()
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (mainPage != null && mainPage is ProductMaintain)

                new ProductFacade().BatchUpdateProductDescriptionInfo(
                    (mainPage as ProductMaintain).VM,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        mainPage.Context.Window.MessageBox.Show("商品详细描述信息批量更新成功", MessageBoxType.Success);
                    });
        }

        private void hyperlinkView_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string previewDesc = this.tbProductDetailDescription.Text + "<br/>" + this.tbProductImageDescription.Text;

            HtmlViewHelper.ViewHtmlInBrowser("IM", "<div align=\"left\" style=\"overflow:auto;height:585px;width:790px\">" + previewDesc + "</div>", null, new Size(800, 600), false, false);        
        }
    }
}
