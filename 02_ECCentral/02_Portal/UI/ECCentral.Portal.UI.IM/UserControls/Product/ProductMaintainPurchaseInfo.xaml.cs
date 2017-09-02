using System.Windows.Controls;
using ECCentral.Portal.Basic.Interface;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Portal.UI.IM.Views;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Windows;
using System.Collections.Generic;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainPurchaseInfo : UserControl, ISave
    {

        public ProductMaintainPurchaseInfoVM VM
        {
            get { return DataContext as ProductMaintainPurchaseInfoVM; }
        }

        public ProductMaintainPurchaseInfo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置ERP大类
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetERPCategory_Click(object sender, RoutedEventArgs e)
        {
            ProductERPCategorySearch newCtrl = new ProductERPCategorySearch();

            newCtrl.DialogHandler = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResProductMaintain.Button_ERPCategorySearch, newCtrl, (obj, args) =>
            {
                ProductERPCategoryVM addedVM = new ProductERPCategoryVM();
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    addedVM = args.Data as ProductERPCategoryVM;

                    this.VM.ERPProductID = addedVM.SP_ID;
                }
            });

        }

        public void Save()
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (mainPage != null && mainPage is ProductMaintain)

                new ProductFacade().UpdateProductPurchaseInfo(
                    (mainPage as ProductMaintain).VM,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        mainPage.Context.Window.MessageBox.Show("商品采购相关信息更新成功", MessageBoxType.Success);
                    });
        }
    }
}
