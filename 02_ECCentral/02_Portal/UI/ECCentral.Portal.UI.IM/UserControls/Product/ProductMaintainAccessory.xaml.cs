using System.Windows.Controls;
using ECCentral.Portal.Basic.Interface;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Portal.UI.IM.Views;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using System.Windows;
using ECCentral.Portal.Basic.Components.UserControls.Language;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainAccessory : UserControl, ISave, IBatchSave
    {
        public ProductMaintainAccessoryVM VM
        {
            get { return DataContext as ProductMaintainAccessoryVM; }
        }

        public IWindow MyWindow { get; set; }

        public ProductMaintainAccessory()
        {
            InitializeComponent();
        }

        private void hyperlinkAccessoryMultiLanguage_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectItem = this.dgProductAccessoryList.SelectedItem as dynamic;

            UCMultiLanguageMaintain item = new UCMultiLanguageMaintain(selectItem.SysNo, "ProductHasAccessories");

            item.Dialog = MyWindow.ShowDialog("设置配件多语言", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    //this.dgProductPropertyInfo.Bind();
                }
            }, new Size(750, 600));

        }

        public void Save()
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (mainPage != null && mainPage is ProductMaintain)

                new ProductFacade().UpdateProductAccessoryInfo(
                    (mainPage as ProductMaintain).VM,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        mainPage.Context.Window.MessageBox.Show("商品配件信息更新成功", MessageBoxType.Success);
                    });
        }

        public void BatchSave()
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (mainPage != null && mainPage is ProductMaintain)

                new ProductFacade().BatchUpdateProductAccessoryInfo(
                    (mainPage as ProductMaintain).VM,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        mainPage.Context.Window.MessageBox.Show("商品配件信息批量更新成功", MessageBoxType.Success);
                    });
        }
    }
}
