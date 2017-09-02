using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.UserControls.ProductGroup;
using ECCentral.Service.IM.Restful.ResponseMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Text;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductGroupMaintainProductList : UserControl
    {
        public ProductGroupMaintainProductListVM VM
        {
            get { return DataContext as ProductGroupMaintainProductListVM; }
        }

        public ProductGroupMaintainProductList()
        {
            InitializeComponent();
        }

        private void ChbSelectAllProductGroupRowClick(object sender, RoutedEventArgs e)
        {
            var ckb = sender as CheckBox;
            if (ckb != null && ckb.IsChecked.HasValue)
            {
                VM.ProductGroupProductVMList.ForEach(product => product.IsChecked = ckb.IsChecked.Value);
            }
        }

        private void HyperlinkProductIDClick(object sender, RoutedEventArgs e)
        {
            var currentVM = dgProductGroupProductList.SelectedItem as ProductGroupProductVM;

            if (currentVM != null)
                CPApplication.Current.CurrentPage.Context.Window.Navigate(String.Format(ConstValue.IM_ProductMaintainUrlFormat, currentVM.ProductSysNo), null, true);
        }

        private void BtnBatchRemoveProductClick(object sender, RoutedEventArgs e)
        {
            if (VM.ProductGroupProductVMList.Count(product => product.IsChecked) == 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请选择要删除的商品！");
                return;
            }

            var list = new List<ProductGroupProductVM>();
            VM.ProductGroupProductVMList.ForEach(list.Add);

            list.ForEach(productGroupProduct =>
            {
                if (productGroupProduct.IsChecked)
                {
                    VM.ProductGroupProductVMList.Remove(productGroupProduct);
                }
            });

            dgProductGroupProductList.ItemsSource = VM.ProductGroupProductVMList;
        }

        private void BtnBatchAddProductClick(object sender, RoutedEventArgs e)
        {
            var ucPicker = new UCProductSearch { SelectionMode = SelectionMode.Multiple };
            ucPicker.DialogHandler = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("选择商品", ucPicker, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    if (args.Data != null)
                    {
                        var selectedProductList = args.Data as List<Basic.Components.UserControls.ProductPicker.ProductVM>;
                        new ProductGroupFacade().GetProductList(selectedProductList
                            .Select(product => product.SysNo.HasValue ? product.SysNo.Value : 0).ToList(), (o, a) =>
                        {
                            if (a.FaultsHandle())
                            {
                                return;
                            }

                            a.Result.ForEach(productInfo =>
                            {
                                if (!VM.ProductGroupProductVMList.Any(product => product.ProductSysNo == productInfo.SysNo))
                                {
                                    VM.ProductGroupProductVMList.Add(
                                        new ProductGroupFacade().ConvertProductInfoEntityToProductGroupProductVM(productInfo));
                                }
                            });
                            dgProductGroupProductList.ItemsSource = VM.ProductGroupProductVMList;
                        });
                    }
                }
            });
        }

        private void BtnBatchCreateSimilarItemClick(object sender, RoutedEventArgs e)
        {
            if (VM.MainPageVM.PropertyVM.ProductGroupSettings.All(setting => setting.ProductGroupProperty.SysNo == 0))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请至少选择一个分组属性", MessageType.Error);
                return;
            }
            var similarProductUC = new ProductGroupMaintainAddSimilarProduct();
            var window = CPApplication.Current.CurrentPage.Context.Window;
            similarProductUC.Dialog = window.ShowDialog(IM.Resources.ResProductGroupMaintain.Dialog_AddSimilarItem,
                                                        similarProductUC, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    dgProductGroupProductList.ItemsSource = VM.ProductGroupProductVMList;
                    if (args.Data is List<ErrorProduct>)
                    {
                        var errorProductList = args.Data as List<ErrorProduct>;
                        var result = new StringBuilder();
                        errorProductList.ForEach(p => result.AppendLine(p.ProductTitle + "新建失败：" + p.ErrorMsg));
                        if (result.Length > 0)
                        {
                            window.Alert(result.ToString(), MessageType.Error);
                        }
                    }
                }
            }, new Size(1000, 500));
        }
    }
}
