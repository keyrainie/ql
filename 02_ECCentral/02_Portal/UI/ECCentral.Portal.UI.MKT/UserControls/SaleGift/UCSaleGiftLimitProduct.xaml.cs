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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using System.Text;

namespace ECCentral.Portal.UI.MKT.UserControls.SaleGift
{
    public partial class UCSaleGiftLimitProduct : UserControl
    {
        bool isLoaded = false;
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public UCSaleGiftLimitProduct()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCSaleGiftLimitProduct_Loaded);
        }

        void UCSaleGiftLimitProduct_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                return;
            }

            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;

            if (vm.Type.Value == SaleGiftType.Vendor || vm.Type.Value == SaleGiftType.Single)
            {
                dgProductOnly.Columns[9].Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                dgProductOnly.Columns[9].Visibility = System.Windows.Visibility.Visible;     
            }

            dgProductOnly.ItemsSource = vm.ProductOnlyList;
            dgProductOnly.Bind();

            isLoaded = true;
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            UCProductSearch ucPicker = new UCProductSearch();
            ucPicker.SelectionMode = SelectionMode.Multiple;
            ucPicker.DialogHandler = CurrentWindow.ShowDialog("选择商品", ucPicker, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    if (args.Data == null) return;
                    SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
                    List<ProductVM> selectedList = args.Data as List<ProductVM>;

                    StringBuilder message = new StringBuilder();

                    foreach (ProductVM product in selectedList)
                    {
                        //if (product.Status.Value != BizEntity.IM.ProductStatus.Active)
                        //{
                        //    message.AppendLine(string.Format("商品{0}必须为上架商品！",product.ProductID));
                        //    continue;
                        //}

                        SaleGift_RuleSettingViewModel settingVM = new SaleGift_RuleSettingViewModel();
                        settingVM.IsChecked = false;

                        settingVM.RelProduct.ProductSysNo = product.SysNo;
                        settingVM.RelProduct.ProductID = product.ProductID;
                        settingVM.RelProduct.ProductName = product.ProductName;
                        settingVM.RelProduct.AvailableQty = product.AvailableQty;
                        settingVM.RelProduct.ConsignQty = product.ConsignQty;
                        settingVM.RelProduct.VirtualQty = product.OnlineQty - (product.AvailableQty + product.ConsignQty);
                        settingVM.RelProduct.UnitCost = product.UnitCost;
                        settingVM.RelProduct.CurrentPrice = product.CurrentPrice;
                        //获取商品的毛利
                        //new CouponsFacade(CPApplication.Current.CurrentPage).GetCouponGrossMarginRate(product.SysNo.Value, (s, a) =>
                        //{
                        //    settingVM.RelProduct.GrossMarginRate = a.Result;
                        //});                        
                        settingVM.RelProduct.MinQty = "1";

                        if (vm.ProductOnlyList.FirstOrDefault(f => f.RelProduct.ProductSysNo == settingVM.RelProduct.ProductSysNo) != null)
                        {
                           message.AppendLine(string.Format("商品{0}已经存在!", product.ProductID));
                            continue;
                        }
                        
                        settingVM.Type = SaleGiftSaleRuleType.Item;
                        settingVM.ComboType = AndOrType.And;

                        vm.ProductOnlyList.Add(settingVM);
                    }

                    if (message.Length > 0)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(message.ToString(), MessageType.Warning);
                    }

                    dgProductOnly.ItemsSource = vm.ProductOnlyList;                    
                }
            });
        }

        private void btnRemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            List<SaleGift_RuleSettingViewModel> needRemoveList = new List<SaleGift_RuleSettingViewModel>();
            if (vm.ProductOnlyList.Count > 0)
            {
                foreach (SaleGift_RuleSettingViewModel setting in vm.ProductOnlyList)
                {
                    if (setting.IsChecked)
                    {
                        needRemoveList.Add(setting);
                    }
                }
                needRemoveList.ForEach(f => vm.ProductOnlyList.Remove(f));
                dgProductOnly.ItemsSource = vm.ProductOnlyList;
                dgProductOnly.Bind();
            }
        }

        private void chkDGProductCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            CheckBox chk = (CheckBox)sender;
            foreach (var item in vm.ProductOnlyList)
            {
                item.IsChecked = chk.IsChecked.Value;
            }            
            //dgProductOnly.ItemsSource = vm.ProductOnlyList;            
        }
    }
}
