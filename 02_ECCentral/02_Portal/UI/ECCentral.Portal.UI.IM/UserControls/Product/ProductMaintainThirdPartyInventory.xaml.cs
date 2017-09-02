using System;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Interface;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Portal.UI.IM.Views;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;


namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainThirdPartyInventory : UserControl, ISave
    {
        //public ProductMaintainThirdPartyInventoryVM VM
        //{
        //    get { return DataContext as ProductMaintainThirdPartyInventoryVM; }
        //}


        public ProductMaintainThirdPartyInventory()
        {
            //InitializeComponent();
        }

        public void Save()
        {
            //var mainPage = CPApplication.Current.CurrentPage;

            //if (mainPage != null && mainPage is ProductMaintain)

            //    new ProductFacade().UpdateProductThirdPartyInventory(
            //        (mainPage as ProductMaintain).VM,
            //        (obj, args) =>
            //        {
            //            if (args.FaultsHandle())
            //            {
            //                return;
            //            }
            //            mainPage.Context.Window.MessageBox.Show("库存同步合作信息更新成功", MessageBoxType.Success);
            //        });
        }

        private void HyperlinkInventoryClick(object sender, System.Windows.RoutedEventArgs e)
        {
            //var mainPage = CPApplication.Current.CurrentPage;
            //if (mainPage != null && mainPage is ProductMaintain)
            //{
            //    CPApplication.Current.CurrentPage.Context.Window.Navigate(String.Format(ConstValue.Inventory_VirtualRequestMaintainCreateFormat, (mainPage as ProductMaintain).VM.ProductSysNo), null, true);
            //}
        }

        private void CmbVirtualQtyRuleListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //VM.DirectVisible = VM.StockRules == StockRules.Direct ? "Collapsed" : "Visible";
            //VM.LimitCount = "0";
        }
    }
}
