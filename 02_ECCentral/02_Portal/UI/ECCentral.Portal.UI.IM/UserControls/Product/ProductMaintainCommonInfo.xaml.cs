using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Portal.UI.IM.Views;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.IM.Models;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainCommonInfo : UserControl
    {

        public ProductMaintainCommonInfoVM VM
        {
            get { return DataContext as ProductMaintainCommonInfoVM; }
        }

        public ProductMaintainCommonInfo()
        {
            InitializeComponent();
        }

        private void CkbSelectAllRowClick(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox != null && checkbox.IsChecked.HasValue)
            {
                if (checkbox.IsChecked.Value)
                {
                    VM.GroupProductList.ForEach(product => product.IsChecked = true);
                }
                else
                {
                    VM.GroupProductList.ForEach(product => product.IsChecked = false);
                }
            }
        }

        private void BtnBatchActiveClick(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemProductBatchActive))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("你无此操作权限");
                return;
            }

            List<int> batchOnSaleList =
                VM.GroupProductList.Where(p => p.IsChecked).Select(p => p.ProductSysNo).ToList();

            if (!batchOnSaleList.Any())
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请选择商品", MessageType.Error);
            }
            else
            {
                var facade = new ProductFacade(CPApplication.Current.CurrentPage);
                facade.ProductBatchOnSale(batchOnSaleList, (obj, args)
                    =>
                    {
                        if (!args.FaultsHandle())
                        {
                            var successCount = 0;

                            var result = new StringBuilder();

                            foreach (var p in VM.GroupProductList.Where(p => p.IsChecked).Where(p => !args.Result.ContainsKey(p.ProductSysNo)))
                            {
                                successCount++;
                                p.ProductStatus = ProductStatus.Active;
                                if (VM.ProductID == p.ProductID)
                                {
                                    ((ProductMaintain)CPApplication.Current.CurrentPage).VM.ProductMaintainBasicInfo.ProductMaintainBasicInfoStatusInfo.ProductStatus = ProductStatus.Active;
                                }
                            }

                            if (args.Result.Any())
                            {
                                foreach (var r in args.Result)
                                {
                                    result.AppendLine(r.Value);
                                }
                            }

                            result.AppendLine("更新成功，影响记录数" + successCount + "条");
                            if (args.Result.Any())
                            {
                                CPApplication.Current.CurrentPage.Context.
                                    Window.Alert(result.ToString());
                            }
                            else
                            {
                                CPApplication.Current.CurrentPage.Context.
                                    Window.MessageBox.Show(result.ToString().Trim(), MessageBoxType.Success);
                            }
                        }
                    });
            }
        }

        private void BtnProductGroupEditClick(object sender, RoutedEventArgs e)
        {
            if (VM.ProductGroupSysNo != 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Navigate(String.Format(ConstValue.IM_ProductGroupMaintainUrlFormat, VM.ProductGroupSysNo), null, true);
            }
        }


        private void BtnPreviewClick(object sender, RoutedEventArgs e)
        {
            //if (!String.IsNullOrEmpty(VM.ProductID))
            {
                //Ocean.20130514, Move to ControlPanelConfiguration
                if (CPApplication.Current.CurrentPage is PageBase)
                {
                    PageBase PeratPage = CPApplication.Current.CurrentPage as PageBase;
                    if (PeratPage.DataContext is ProductVM)
                    {
                        ProductVM pageVM = PeratPage.DataContext as ProductVM;
                        if (pageVM.ProductSysNo > 0)
                        {
                            string urlFormat = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebSiteProductPreviewUrl);
                            CPApplication.Current.CurrentPage.Context.Window.Navigate(String.Format(urlFormat, pageVM.ProductSysNo), null, true);
                        }
                    }
                }
                
            }
        }

        private void HyperlinkProductClick(object sender, RoutedEventArgs e)
        {
            var currentVM = dgRelatedProduct.SelectedItem as ProductMaintainCommonInfoGroupProductVM;

            if (currentVM != null)
                CPApplication.Current.CurrentPage.Context.Window.Navigate(String.Format(ConstValue.IM_ProductMaintainUrlFormat, currentVM.ProductSysNo), null, true);
        }

        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString() == "Enter")
            {
                var facade = new ProductFacade(CPApplication.Current.CurrentPage);
                facade.GetProductInfo(VM.ProductID, (obj, args) =>
                                                       {
                                                           if (args.Result == null)
                                                           {
                                                               CPApplication.Current.CurrentPage.Context.Window.Alert("该商品不存在", MessageType.Error);
                                                           }
                                                           else
                                                           {
                                                               CPApplication.Current.CurrentPage.Context.Window.Navigate(String.Format(ConstValue.IM_ProductMaintainUrlFormat, args.Result.SysNo), null, false);
                                                           }
                                                       });
            }
        }
    }
}
