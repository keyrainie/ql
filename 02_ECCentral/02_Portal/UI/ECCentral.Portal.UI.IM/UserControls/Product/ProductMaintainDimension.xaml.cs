using System.Windows.Controls;
using ECCentral.Portal.Basic.Interface;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Portal.UI.IM.Views;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainDimension : UserControl, ISave, IBatchSave
    {

        public ProductMaintainDimensionVM VM
        {
            get { return DataContext as ProductMaintainDimensionVM; }
        }

        public ProductMaintainDimension()
        {
            InitializeComponent();
        }

        public void Save()
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (mainPage != null && mainPage is ProductMaintain)

                new ProductFacade().UpdateProductDimensionInfo(
                    (mainPage as ProductMaintain).VM,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        mainPage.Context.Window.MessageBox.Show("商品重量信息更新成功", MessageBoxType.Success);
                    });
        }

        public void BatchSave()
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (mainPage != null && mainPage is ProductMaintain)

                new ProductFacade().BatchUpdateProductDimensionInfo(
                    (mainPage as ProductMaintain).VM,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        mainPage.Context.Window.MessageBox.Show("商品重量信息批量更新成功", MessageBoxType.Success);
                    });
        }
    }
}
