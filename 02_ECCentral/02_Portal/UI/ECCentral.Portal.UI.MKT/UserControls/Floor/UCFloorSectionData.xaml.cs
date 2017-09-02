using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.MKT.Floor;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models.Floor;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Floor
{
    public partial class UCFloorSectionData : UserControl
    {
        public FloorSectionVM CurrentVM { get; set; }

        private FloorFacade ServiceFacade { get; set; }

        public List<FloorSectionProductVM> ProductListVM
        {
            get { return productResult.ItemsSource as List<FloorSectionProductVM>; }
            set { productResult.ItemsSource = value; }
        }

        public List<FloorSectionBannerVM> BannerListVM
        {
            get { return BannerResult.ItemsSource as List<FloorSectionBannerVM>; }
            set { BannerResult.ItemsSource = value; }
        }

        public List<FloorSectionBrandVM> BrandListVM
        {
            get { return BrandResult.ItemsSource as List<FloorSectionBrandVM>; }
            set { BrandResult.ItemsSource = value; }
        }

        public List<FloorSectionLinkVM> TextLinkListVM
        {
            get { return TextLinkResult.ItemsSource as List<FloorSectionLinkVM>; }
            set { TextLinkResult.ItemsSource = value; }
        }

        public IPage Page
        {
            get { return CPApplication.Current.CurrentPage; }
        }

        private IWindow CurrentWindow
        {
            get { return CPApplication.Current.CurrentPage.Context.Window; }
        }


        public UCFloorSectionData()
        {
            InitializeComponent();
            ProductListVM = new List<FloorSectionProductVM>();
            BannerListVM = new List<FloorSectionBannerVM>();
            BrandListVM = new List<FloorSectionBrandVM>();
            TextLinkListVM = new List<FloorSectionLinkVM>();
            Loaded += new RoutedEventHandler(UCFloorSectionData_Loaded);
        }

        private void UCFloorSectionData_Loaded(object sender, RoutedEventArgs e)
        {
            ServiceFacade = new FloorFacade(CPApplication.Current.CurrentPage);
            this.Loaded -= UCFloorSectionData_Loaded;
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVM == null)
            {
                CurrentWindow.Alert(ResFloorMaintain.Info_SectionChecked);
                return;
            }
            UCProductForSection ucProduct = new UCProductForSection();
            ucProduct.Dialog = CurrentWindow.ShowDialog(ResFloorMaintain.Head_ProductSection, ucProduct, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var result = args.Data as FloorSectionProductVM;
                    FloorSectionItem sectionItem = LoadSectionItem(result, FloorItemType.Product);
                    sectionItem.ItemProudct = result.ConvertVM<FloorSectionProductVM, FloorItemProduct>();

                    ServiceFacade.CreateFloorSectionItem(sectionItem, (s, objArgs) =>
                    {
                        if (objArgs.FaultsHandle()) return;
                        result.SysNo = objArgs.Result;
                        ProductListVM.Add(result);
                        productResult.ItemsSource = ProductListVM;
                    });
                }
            });
        }

        private void BtnBatchAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVM == null)
            {
                CurrentWindow.Alert(ResFloorMaintain.Info_SectionChecked);
                return;
            }
            UCProductSearch ucProduct = new UCProductSearch();
            ucProduct.SelectionMode = SelectionMode.Multiple;
            ucProduct.DialogHandler = CurrentWindow.ShowDialog(ResFloorMaintain.Head_ProductAdd, ucProduct, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    List<ProductVM> products = args.Data as List<ProductVM>;
                    if (products != null && products.Count > 0)
                    {
                        List<FloorSectionItem> sectionItems = new List<FloorSectionItem>();
                        List<FloorSectionProductVM> proVMItems = new List<FloorSectionProductVM>();

                        products.ForEach(p =>
                        {
                            FloorSectionProductVM vm = EntityConverter<ProductVM, FloorSectionProductVM>.Convert(p, (s, t) =>
                            {
                                t.Priority = 0;
                                t.ItemPosition = 0;
                                t.IsSelfPage = 0;
                                t.ProductTitle = s.ProductName;
                                t.ProductPrice = s.CurrentPrice;
                                t.ProductID = s.ProductID;
                                t.ProductSysNo = s.SysNo.Value;
                            });
                            FloorSectionItem sectionItem = LoadSectionItem(vm, FloorItemType.Product);
                            sectionItem.ItemProudct = vm.ConvertVM<FloorSectionProductVM, FloorItemProduct>();
                            sectionItems.Add(sectionItem);
                            proVMItems.Add(vm);
                        });

                        ServiceFacade.BtnBatchCreateFloorSectionItem(sectionItems, (s, objArgs) =>
                        {
                            if (objArgs.FaultsHandle()) return;
                            for (int i = 0; i < objArgs.Result.Count; i++)
                            {
                                proVMItems[i].SysNo = objArgs.Result[i];
                                ProductListVM.Add(proVMItems[i]);
                                productResult.ItemsSource = ProductListVM;
                            }
                        });
                    }
                }
            });
        }

        private void ButtonProductEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var proVM = btnEdit.DataContext as FloorSectionProductVM;
            UCProductForSection ucProduct = new UCProductForSection(proVM.DeepCopy());
            ucProduct.Dialog = CurrentWindow.ShowDialog(ResFloorMaintain.Head_ProductSection, ucProduct, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var result = args.Data as FloorSectionProductVM;
                    FloorSectionItem sectionItem = result.ConvertVM<FloorSectionProductVM, FloorSectionItem>((s, t) =>
                    {
                        t.ItemProudct = result.ConvertVM<FloorSectionProductVM, FloorItemProduct>();
                    });

                    ServiceFacade.UpdateFloorSectionItem(sectionItem, (s, objArgs) =>
                    {
                        if (objArgs.FaultsHandle()) return;
                        for (int i = 0; i < ProductListVM.Count; i++)
                        {
                            if (ProductListVM[i].SysNo == proVM.SysNo)
                            {
                                ProductListVM[i] = result.DeepCopy();
                                productResult.ItemsSource = ProductListVM;
                                break;
                            }
                        }
                    });
                }
            });
        }

        private void ButtonProductDelete_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var tempVM = btnEdit.DataContext as FloorSectionProductVM;
            CurrentWindow.Confirm(ResFloorMaintain.Info_ConfirmDelete, (diaObj, diaArgs) =>
            {
                if (diaArgs.DialogResult == DialogResultType.OK)
                {
                    ServiceFacade.DeleteFloorSectionItem(tempVM.SysNo.Value, (s, objArgs) =>
                    {
                        if (objArgs.FaultsHandle()) return;
                        ProductListVM.Remove(tempVM);
                        productResult.ItemsSource = ProductListVM;
                    });
                }
            });
        }

        private void BtnAddBanner_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVM == null)
            {
                CurrentWindow.Alert(ResFloorMaintain.Info_SectionChecked);
                return;
            }
            UCBannerForSection ucBanner = new UCBannerForSection();
            ucBanner.Dialog = CurrentWindow.ShowDialog(ResFloorMaintain.Head_BannerSection, ucBanner, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var result = args.Data as FloorSectionBannerVM;
                    FloorSectionItem sectionItem = LoadSectionItem(result, FloorItemType.Banner);
                    sectionItem.ItemBanner = result.ConvertVM<FloorSectionBannerVM, FloorItemBanner>();
                    ServiceFacade.CreateFloorSectionItem(sectionItem, (s, objArgs) =>
                    {
                        if (objArgs.FaultsHandle()) return;
                        result.SysNo = objArgs.Result;
                        BannerListVM.Add(result);
                        BannerResult.ItemsSource = BannerListVM;
                    });
                }
            });
        }

        private void ButtonBannerEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var tempVM = btnEdit.DataContext as FloorSectionBannerVM;
            UCBannerForSection ucBanner = new UCBannerForSection(tempVM.DeepCopy());
            ucBanner.Dialog = CurrentWindow.ShowDialog(ResFloorMaintain.Head_ProductSection, ucBanner, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var result = args.Data as FloorSectionBannerVM;
                    FloorSectionItem sectionItem = result.ConvertVM<FloorSectionBannerVM, FloorSectionItem>((s, t) =>
                    {
                        t.ItemBanner = result.ConvertVM<FloorSectionBannerVM, FloorItemBanner>();
                    });

                    ServiceFacade.UpdateFloorSectionItem(sectionItem, (s, objArgs) =>
                    {
                        if (objArgs.FaultsHandle()) return;
                        for (int i = 0; i < BannerListVM.Count; i++)
                        {
                            if (BannerListVM[i].SysNo == tempVM.SysNo)
                            {
                                BannerListVM[i] = result.DeepCopy();
                                BannerResult.ItemsSource = BannerListVM;
                                break;
                            }
                        }
                    });
                }
            });
        }

        private void ButtonBannerDelete_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var tempVM = btnEdit.DataContext as FloorSectionBannerVM;
            CurrentWindow.Confirm(ResFloorMaintain.Info_ConfirmDelete, (diaObj, diaArgs) =>
            {
                if (diaArgs.DialogResult == DialogResultType.OK)
                {
                    ServiceFacade.DeleteFloorSectionItem(tempVM.SysNo.Value, (s, objArgs) =>
                    {
                        if (objArgs.FaultsHandle()) return;
                        BannerListVM.Remove(tempVM);
                        BannerResult.ItemsSource = BannerListVM;
                    });
                }
            });
        }

        private void BtnAddBrand_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVM == null)
            {
                CurrentWindow.Alert(ResFloorMaintain.Info_SectionChecked);
                return;
            }
            UCBrandForSection ucBrand = new UCBrandForSection();
            ucBrand.Dialog = CurrentWindow.ShowDialog(ResFloorMaintain.Head_BrandSection, ucBrand, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var result = args.Data as FloorSectionBrandVM;
                    FloorSectionItem sectionItem = LoadSectionItem(result, FloorItemType.Brand);
                    sectionItem.ItemBrand = result.ConvertVM<FloorSectionBrandVM, FloorItemBrand>();
                    ServiceFacade.CreateFloorSectionItem(sectionItem, (s, objArgs) =>
                    {
                        if (objArgs.FaultsHandle()) return;
                        result.SysNo = objArgs.Result;
                        BrandListVM.Add(result);
                        BrandResult.ItemsSource = BrandListVM;
                    });
                }
            });
        }

        private void ButtonBrandEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var tempVM = btnEdit.DataContext as FloorSectionBrandVM;
            UCBrandForSection ucBrand = new UCBrandForSection(tempVM.DeepCopy());
            ucBrand.Dialog = CurrentWindow.ShowDialog(ResFloorMaintain.Head_BrandSection, ucBrand, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var result = args.Data as FloorSectionBrandVM;
                    FloorSectionItem sectionItem = result.ConvertVM<FloorSectionBrandVM, FloorSectionItem>((s, t) =>
                    {
                        t.ItemBrand = EntityConverter<FloorSectionBrandVM, FloorItemBrand>.Convert(result);
                    });
                    ServiceFacade.UpdateFloorSectionItem(sectionItem, (s, objArgs) =>
                    {
                        if (objArgs.FaultsHandle()) return;
                        for (int i = 0; i < BrandListVM.Count; i++)
                        {
                            if (BrandListVM[i].SysNo == tempVM.SysNo)
                            {
                                BrandListVM[i] = result.DeepCopy();
                                BrandResult.ItemsSource = BrandListVM;
                                break;
                            }
                        }
                    });
                }
            });
        }

        private void ButtonBrandDelete_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var tempVM = btnEdit.DataContext as FloorSectionBrandVM;
            CurrentWindow.Confirm(ResFloorMaintain.Info_ConfirmDelete, (diaObj, diaArgs) =>
            {
                if (diaArgs.DialogResult == DialogResultType.OK)
                {
                    ServiceFacade.DeleteFloorSectionItem(tempVM.SysNo.Value, (s, objArgs) =>
                     {
                         if (objArgs.FaultsHandle()) return;
                         BrandListVM.Remove(tempVM);
                         BrandResult.ItemsSource = BrandListVM;
                     });
                }
            });
        }

        private void BtnAddTextLink_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVM == null)
            {
                CurrentWindow.Alert(ResFloorMaintain.Info_SectionChecked);
                return;
            }
            UCLinkForSection ucTextLink = new UCLinkForSection();
            ucTextLink.Dialog = CurrentWindow.ShowDialog(ResFloorMaintain.Head_TextLinkSection, ucTextLink, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var result = args.Data as FloorSectionLinkVM;
                    FloorSectionItem sectionItem = LoadSectionItem(result, FloorItemType.TextLink);
                    sectionItem.ItemTextLink = result.ConvertVM<FloorSectionLinkVM, FloorItemTextLink>();
                    ServiceFacade.CreateFloorSectionItem(sectionItem, (s, objArgs) =>
                    {
                        if (objArgs.FaultsHandle()) return;
                        result.SysNo = objArgs.Result;
                        TextLinkListVM.Add(result);
                        TextLinkResult.ItemsSource = TextLinkListVM;
                    });
                }
            });
        }

        private void ButtonTextLinkEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var tempVM = btnEdit.DataContext as FloorSectionLinkVM;
            UCLinkForSection ucBrand = new UCLinkForSection(tempVM.DeepCopy());
            ucBrand.Dialog = CurrentWindow.ShowDialog(ResFloorMaintain.Head_TextLinkSection, ucBrand, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var result = args.Data as FloorSectionLinkVM;
                    FloorSectionItem sectionItem = result.ConvertVM<FloorSectionLinkVM, FloorSectionItem>((s, t) =>
                    {
                        t.ItemTextLink = EntityConverter<FloorSectionLinkVM, FloorItemTextLink>.Convert(result);
                    });
                    ServiceFacade.UpdateFloorSectionItem(sectionItem, (s, objArgs) =>
                    {
                        if (objArgs.FaultsHandle()) return;
                        for (int i = 0; i < TextLinkListVM.Count; i++)
                        {
                            if (TextLinkListVM[i].SysNo == tempVM.SysNo)
                            {
                                TextLinkListVM[i] = result.DeepCopy();
                                TextLinkResult.ItemsSource = TextLinkListVM;
                                break;
                            }
                        }
                    });
                }
            });
        }

        private void ButtonTextLinkDelete_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var tempVM = btnEdit.DataContext as FloorSectionLinkVM;
            CurrentWindow.Confirm(ResFloorMaintain.Info_ConfirmDelete, (diaObj, diaArgs) =>
            {
                if (diaArgs.DialogResult == DialogResultType.OK)
                {
                    ServiceFacade.DeleteFloorSectionItem(tempVM.SysNo.Value, (s, objArgs) =>
                 {
                     TextLinkListVM.Remove(tempVM);
                     TextLinkResult.ItemsSource = TextLinkListVM;
                 });
                }
            });
        }

        private FloorSectionItem LoadSectionItem(FloorSectionItemVM result, FloorItemType itemType)
        {
            FloorSectionItem sectionItem = new FloorSectionItem()
            {
                FloorMasterSysNo = result.FloorMasterSysNo = CurrentVM.FloorMasterSysNo,
                FloorSectionSysNo = result.FloorSectionSysNo = CurrentVM.SysNo.Value,
                ItemType = result.ItemType = itemType,
                Priority = result.Priority,
                ItemPosition = result.ItemPosition,
                IsSelfPage = result.IsSelfPage,
            };
            return sectionItem;
        }
    }
}
