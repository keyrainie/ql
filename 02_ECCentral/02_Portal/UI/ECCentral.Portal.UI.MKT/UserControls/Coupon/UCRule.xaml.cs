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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Collections.ObjectModel;

namespace ECCentral.Portal.UI.MKT.UserControls.Coupon
{
    public partial class UCRule : UserControl
    {
        bool isLoaded = false;
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }


        public UCRule()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCRule_Loaded);
        }

        void UCRule_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                return;
            }
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            if (vm.BindCondition.Value == CouponsBindConditionType.None)
            {
                //控制发放的优惠券有效期的显示
                gridValidPeriod.Visibility = System.Windows.Visibility.Collapsed;

            }
            else
            {
                gridValidPeriod.Visibility = System.Windows.Visibility.Visible;
            }

            if (vm.ValidPeriod == CouponsValidPeriodType.CustomPeriod)
            {
                spCustomDateRange.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                spCustomDateRange.Visibility = System.Windows.Visibility.Collapsed;
            }

            datepickerBindingDate.IsEnabled = vm.IsAutoBinding.Value;
             
            if (vm.IsOnlyViewMode)
            {
                OperationControlStatusHelper.SetControlsStatus(expanderRule, true);
                OperationControlStatusHelper.SetControlsStatus(expanderBind, true);
            }
            ShowUiControls(vm);
            isLoaded = true;
        }

        private void cmbValidPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            if (vm.ValidPeriod == CouponsValidPeriodType.CustomPeriod)
            {
                spCustomDateRange.Visibility = System.Windows.Visibility.Visible;
                
            }
            else
            {
                spCustomDateRange.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void chkIsAutoBinding_Checked(object sender, RoutedEventArgs e)
        {
            datepickerBindingDate.IsEnabled = true;
        }

        private void chkIsAutoBinding_Unchecked(object sender, RoutedEventArgs e)
        {
            datepickerBindingDate.IsEnabled = false;
        }

        private void cmbBindCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            if (vm.IsExistThrowInTypeCouponCode.HasValue && vm.IsExistThrowInTypeCouponCode.Value)
            {
                if (vm.BindCondition.Value != CouponsBindConditionType.None)
                {
                    CurrentWindow.Alert("已经存在投放型优惠券，因此触发条件只能是不限！如果需要修改触发条件，请先删除所有的投放型优惠券！");

                    vm.BindCondition  = CouponsBindConditionType.None;
                }
            }
            ShowUiControls(vm);
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
                    CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;

                    if (vm.BindRule == null) vm.BindRule = new CouponBindRuleViewModel();
                    if (vm.BindRule.RelProducts == null) vm.BindRule.RelProducts = new RelProductViewModel();
                    if (vm.BindRule.RelProducts.ProductList == null)
                        vm.BindRule.RelProducts.ProductList = new ObservableCollection<RelProductAndQtyViewModel>();
                    List<ProductVM> selectedList = args.Data as List<ProductVM>;
                    foreach (ProductVM product in selectedList)
                    {
                        var pr = vm.BindRule.RelProducts.ProductList.FirstOrDefault(p => p.ProductSysNo == product.SysNo);
                        if (pr != null)
                        {
                            continue;
                        }
                        //if (product.Status != BizEntity.IM.ProductStatus.Active)
                        //{
                        //    CurrentWindow.Alert(string.Format("商品{0}必须为上架商品!", product.ProductID));
                        //    continue;
                        //}

                        RelProductAndQtyViewModel item = new RelProductAndQtyViewModel();
                        item.ProductSysNo = product.SysNo;
                        item.ProductID = product.ProductID;
                        item.ProductName = product.ProductName;
                        //获取商品的毛利率
                        //new CouponsFacade(CPApplication.Current.CurrentPage).GetCouponGrossMarginRate(item.ProductSysNo.Value, (s, a) =>
                        //{
                        //    item.GrossMarginRate = a.Result;
                        //});

                        item.IsChecked = false;
                        vm.BindRule.RelProducts.ProductList.Add(item);
                    }
                    dgProduct.ItemsSource = vm.BindRule.RelProducts.ProductList;
                }
            });
        }

        private void btnRemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            if (vm.BindRule.RelProducts.ProductList != null)
            {
                List<RelProductAndQtyViewModel> removedList = new List<RelProductAndQtyViewModel>();
                foreach (RelProductAndQtyViewModel sim in vm.BindRule.RelProducts.ProductList)
                {
                    if (sim.IsChecked.Value)
                    {
                        removedList.Add(sim);
                    }
                }
                if (removedList.Count == 0)
                {
                    this.CurrentWindow.Alert("请先至少选中一条记录!");
                }
                else
                {
                    removedList.ForEach(f => vm.BindRule.RelProducts.ProductList.Remove(f));
                    this.dgProduct.ItemsSource = vm.BindRule.RelProducts.ProductList;
                }
            }
        }

        private void cmbProductRangeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            ShowUiControls(vm);
        }

        private void ShowUiControls(CouponsInfoViewModel vm)
        {
            if (vm.BindCondition.Value == CouponsBindConditionType.None || vm.BindCondition.Value == CouponsBindConditionType.Get)
            {
                //控制发放的优惠券有效期的显示
                gridValidPeriod.Visibility = System.Windows.Visibility.Collapsed;

            }
            else
            {
                gridValidPeriod.Visibility = System.Windows.Visibility.Visible;
            }

            if (vm.BindCondition.Value == CouponsBindConditionType.SO)
            {
                //控制发放的优惠券有效期的显示
                tlAmountLimit.Visibility = System.Windows.Visibility.Visible;
                tbAmountLimit.Visibility = System.Windows.Visibility.Visible;
                tlProductRangeType.Visibility = System.Windows.Visibility.Visible;
                cmbProductRangeType.Visibility = System.Windows.Visibility.Visible;
                if (vm.BindRule.ProductRangeType == ProductRangeType.All)
                {
                    expanderBindRulesProduct.Visibility = System.Windows.Visibility.Collapsed;

                }
                else
                {
                    expanderBindRulesProduct.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                tlAmountLimit.Visibility = System.Windows.Visibility.Collapsed;
                tbAmountLimit.Visibility = System.Windows.Visibility.Collapsed;
                tlProductRangeType.Visibility = System.Windows.Visibility.Collapsed;
                cmbProductRangeType.Visibility = System.Windows.Visibility.Collapsed;
                expanderBindRulesProduct.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridCheckBoxAllProduct_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = ((CheckBox)sender).IsChecked.Value;
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            if (vm.BindRule!=null && vm.BindRule.RelProducts !=null && vm.BindRule.RelProducts.ProductList != null)
            {
                foreach (var sim in vm.BindRule.RelProducts.ProductList)
                {
                    sim.IsChecked = isChecked;
                }
            }
        }


    }
}
