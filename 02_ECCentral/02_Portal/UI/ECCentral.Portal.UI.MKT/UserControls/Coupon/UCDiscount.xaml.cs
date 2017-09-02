using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Coupon
{
    public partial class UCDiscount : UserControl
    {
        bool isLoaded = false;
        private RadioButton rdoPre=null;

        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public UCDiscount()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCDiscount_Loaded);
        }

        void UCDiscount_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                return;
            }
                        
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            if (vm.IsDiscountAmount.HasValue && vm.IsDiscountAmount.Value)
            {
                rdoPre = this.rdoDiscountAmount;                
            }
            if (vm.IsDiscountPercent.HasValue && vm.IsDiscountPercent.Value)
            {
                rdoPre = this.rdoDiscountPercent;
            }
            if (vm.IsDiscountSubtract.HasValue && vm.IsDiscountSubtract.Value)
            {
                rdoPre = this.rdoDiscountSubtract;
            }
            if (vm.IsDiscountFinal.HasValue && vm.IsDiscountFinal.Value)
            {
                rdoPre = this.rdoDiscountFinal;
            }
            if (rdoPre == null)
            {                
                rdoPre = this.rdoDiscountAmount;
            }

            //SetDiscountInputType(rdoPre, true);
            SetControlVisible(false);

            if (vm.IsOnlyViewMode)
            {
                if (vm.PriceDiscountRule != null && vm.PriceDiscountRule.Count > 0)
                {
                    this.dgDiscountProduct.Visibility = System.Windows.Visibility.Visible;
                }
                if (vm.OrderAmountDiscountRule.OrderAmountDiscountRank != null && vm.OrderAmountDiscountRule.OrderAmountDiscountRank.Count > 0)
                {
                    this.dgDiscountAmount.Visibility = System.Windows.Visibility.Visible;
                }
                OperationControlStatusHelper.SetControlsStatus(expanderDiscount, true);
            }

            isLoaded = true;
        }
        
        private void rdoDiscountAmount_Click(object sender, RoutedEventArgs e)
        {
            SetDiscountInputType((RadioButton) sender, false);
        }        

        private void rdoDiscountPercent_Click(object sender, RoutedEventArgs e)
        {
            SetDiscountInputType((RadioButton)sender, false);
        }
        
        private void rdoDiscountSubtract_Click(object sender, RoutedEventArgs e)
        {
            SetDiscountInputType((RadioButton)sender, true);
        }

        private void rdoDiscountFinal_Click(object sender, RoutedEventArgs e)
        {
            SetDiscountInputType((RadioButton)sender, true);           
        }

        private void SetDiscountInputType(RadioButton rdoCurrent, bool isOnlyProductDiscount)
        {
            tblockPersent.Visibility = System.Windows.Visibility.Collapsed;
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            if (isOnlyProductDiscount)
            {
                if (vm.OrderAmountDiscountRule != null
                    && vm.OrderAmountDiscountRule.OrderAmountDiscountRank != null
                    && vm.OrderAmountDiscountRule.OrderAmountDiscountRank.Count > 0)
                {
                    CurrentWindow.Confirm("已存在订单级别的折扣设置数据，如果切换为商品级别的折扣设置，将清空订单级别的折扣设置，请确认是否切换？", (obj, args) =>
                    {
                        if (args.DialogResult == DialogResultType.OK)
                        {
                            vm.OrderAmountDiscountRule.OrderAmountDiscountRank = new ObservableCollection<OrderAmountDiscountRankViewModel>();
                            dgDiscountAmount.ItemsSource = vm.OrderAmountDiscountRule.OrderAmountDiscountRank;
                            rdoPre = rdoCurrent;

                            SetControlVisible(isOnlyProductDiscount);
                        }
                        else
                        {
                            rdoPre.IsChecked = true;
                            RaiseRdoClickEvent(rdoPre.Name);
                        }
                    });
                }
                else
                {
                    rdoPre = rdoCurrent;
                    SetControlVisible(isOnlyProductDiscount);
                }
            }
            else
            {               
                if (vm.PriceDiscountRule != null && vm.PriceDiscountRule.Count > 0)
                {
                    CurrentWindow.Confirm("已存在商品级别的折扣设置数据，如果切换为订单级别的折扣设置，将清空商品级别的折扣设置，请确认是否切换？", (obj, args) =>
                    {
                        if (args.DialogResult == DialogResultType.OK)
                        {
                            vm.PriceDiscountRule = new ObservableCollection<PSPriceDiscountRuleViewModel>();
                            this.dgDiscountProduct.ItemsSource = vm.PriceDiscountRule;
                            rdoPre = rdoCurrent;

                            if (this.rdoDiscountPercent.IsChecked ?? false)
                            {
                                tblockPersent.Visibility = System.Windows.Visibility.Visible;
                            }

                            SetControlVisible(isOnlyProductDiscount);
                        }
                        else
                        {
                            rdoPre.IsChecked = true;
                            RaiseRdoClickEvent(rdoPre.Name);
                        }
                    });
                }
                else
                {
                    //折扣百分比
                    if (this.rdoDiscountPercent.IsChecked ?? false)
                    {
                        tblockPersent.Visibility = System.Windows.Visibility.Visible;
                    }
                    SetControlVisible(isOnlyProductDiscount);
                    rdoPre = rdoCurrent;
                }               
            }           
        }

        private void SetControlVisible(bool isOnlyProductDiscount)
        {
            tbAmountLimit.Text = string.Empty;
            tbProductAmount.Text = string.Empty;
            tbAmountValue.Text = string.Empty;
            tbProductQty.Text = string.Empty;

            lblAmountLimit.Visibility = !isOnlyProductDiscount ? Visibility.Visible : System.Windows.Visibility.Collapsed;
            tbAmountLimit.Visibility = !isOnlyProductDiscount ? Visibility.Visible : System.Windows.Visibility.Collapsed;
            lblAmountValue.Visibility = !isOnlyProductDiscount ? Visibility.Visible : System.Windows.Visibility.Collapsed;
            tbAmountValue.Visibility = !isOnlyProductDiscount ? Visibility.Visible : System.Windows.Visibility.Collapsed;
            dgDiscountAmount.Visibility = !isOnlyProductDiscount ? Visibility.Visible : System.Windows.Visibility.Collapsed;

            lblProductAmount.Visibility = !isOnlyProductDiscount ? Visibility.Collapsed : System.Windows.Visibility.Visible;
            tbProductAmount.Visibility = !isOnlyProductDiscount ? Visibility.Collapsed : System.Windows.Visibility.Visible;
            lblProductQty.Visibility = !isOnlyProductDiscount ? Visibility.Collapsed : System.Windows.Visibility.Visible;
            tbProductQty.Visibility = !isOnlyProductDiscount ? Visibility.Collapsed : System.Windows.Visibility.Visible;
            dgDiscountProduct.Visibility = !isOnlyProductDiscount ? Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        public void RaiseRdoClickEvent(string strRdoName)
        {
            switch (strRdoName)
            { 
                case "rdoDiscountAmount":
                    rdoDiscountAmount_Click(rdoDiscountAmount, new RoutedEventArgs());
                    break;
                case "rdoDiscountPercent":
                    rdoDiscountPercent_Click(rdoDiscountPercent, new RoutedEventArgs());
                    break;
                case "rdoDiscountSubtract":
                    rdoDiscountSubtract_Click(rdoDiscountSubtract, new RoutedEventArgs());
                    break;
                case  "rdoDiscountFinal":
                    rdoDiscountFinal_Click(rdoDiscountFinal, new RoutedEventArgs());
                    break;
                default:
                    break;            
            }
        }

        private void btnAddDiscountRule_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;

            #region 金额类型折扣的规则判断
            if (vm.IsDiscountAmount.Value || vm.IsDiscountPercent.Value)
            {
                decimal dAmountLimit = 0m; decimal dAmountValue = 0m;
                if (decimal.TryParse(tbAmountLimit.Text.Trim(), out dAmountLimit)
                    && decimal.TryParse(tbAmountValue.Text.Trim(), out dAmountValue))
                {
                    if (vm.PriceDiscountRule != null && vm.PriceDiscountRule.Count > 0)
                    {
                        this.CurrentWindow.Alert("已经存在单个限定商品的折扣规则，请先删除限定商品的折扣规则！");
                        return;
                    }

                    if (dAmountLimit < 0 || dAmountValue < 0)
                    {
                        this.CurrentWindow.Alert("金额和数量必须输入，并且必须为大于或等于0的数字!");
                        return;
                    }

                    if (vm.IsDiscountPercent.Value && dAmountValue > 100)
                    {
                        this.CurrentWindow.Alert("折扣百分比模式下，数值必须为0到100之间!");
                        return;
                    }

                    if (vm.IsDiscountAmount.Value)
                    {
                        if (dAmountLimit < dAmountValue)
                        {
                            this.CurrentWindow.Alert("折扣金额模式下，折扣数值不能比限定金额大!");
                            return;
                        }
                    }

                    if (vm.OrderAmountDiscountRule != null
                        && vm.OrderAmountDiscountRule.OrderAmountDiscountRank != null
                        && vm.OrderAmountDiscountRule.OrderAmountDiscountRank.Count > 0)
                    {
                        bool existDouble = false;
                        for (int i = 0; i < vm.OrderAmountDiscountRule.OrderAmountDiscountRank.Count; i++)
                        {
                            if (vm.OrderAmountDiscountRule.OrderAmountDiscountRank[i].OrderMinAmount == dAmountLimit)
                            {
                                existDouble = true;
                                break;
                            }

                        }
                        if (existDouble)
                        {
                            this.CurrentWindow.Alert("相同的限定金额只能设置1条！");
                            return;

                        }
                    }

                    if (vm.OrderAmountDiscountRule.OrderAmountDiscountRank == null) vm.OrderAmountDiscountRule.OrderAmountDiscountRank = new ObservableCollection<OrderAmountDiscountRankViewModel>();

                    if (vm.IsDiscountPercent.HasValue && vm.IsDiscountPercent.Value)
                    {
                        dAmountValue = dAmountValue / 100;
                    }

                    vm.OrderAmountDiscountRule.OrderAmountDiscountRank.Add(new OrderAmountDiscountRankViewModel()
                    {
                        DiscountType = vm.IsDiscountAmount.Value ? PSDiscountTypeForOrderAmount.OrderAmountDiscount : PSDiscountTypeForOrderAmount.OrderAmountPercentage,
                        
                        DiscountValue = dAmountValue,
                        OrderMinAmount = dAmountLimit
                    });
                    dgDiscountAmount.ItemsSource = vm.OrderAmountDiscountRule.OrderAmountDiscountRank;                   
                }
                else
                {
                    this.CurrentWindow.Alert("金额和数量必须输入，并且必须为大于或等于0的数字!");
                    tbAmountLimit.Text = decimal.TryParse(tbAmountLimit.Text.Trim(), out dAmountLimit) ? tbAmountLimit.Text.Trim() : "";
                    tbAmountValue.Text = decimal.TryParse(tbAmountValue.Text.Trim(), out dAmountValue) ? tbAmountValue.Text.Trim() : "";
                    return;
                }
            }
            #endregion

            #region 单个商品直减和最终售价的规则判断
            if (vm.IsDiscountSubtract.Value || vm.IsDiscountFinal.Value)
            {
                if (vm.ProductRangeType != CouponsProductRangeType.LimitProduct)
                {
                    this.CurrentWindow.Alert("直减和最终售价必须限定为指定商品！");
                    return;
                }
                if (vm.ProductCondition.RelProducts == null
                    || vm.ProductCondition.RelProducts.ProductList == null
                    || vm.ProductCondition.RelProducts.ProductList.Count != 1)
                {
                    this.CurrentWindow.Alert("直减和最终售价时，商品范围只能包含一个商品！");
                    return;
                }

                decimal productAmount = 0m; int productQty = 0;
                if (decimal.TryParse(tbProductAmount.Text.Trim(), out productAmount) 
                    && int.TryParse(tbProductQty.Text.Trim(), out productQty))
                {
                    if (vm.OrderAmountDiscountRule.OrderAmountDiscountRank != null && vm.OrderAmountDiscountRule.OrderAmountDiscountRank.Count > 0)
                    {
                        this.CurrentWindow.Alert("已存在不同折扣类型的数据，请先删除其它折扣类型数据！");
                        return;
                    }

                    if (productAmount < 0 || productQty < 0)
                    {
                        this.CurrentWindow.Alert("金额和最大数量必须为大于或等于0的数字!");
                        return;
                    }

                    if (vm.PriceDiscountRule != null && vm.PriceDiscountRule.Count >= 1)
                    {
                        this.CurrentWindow.Alert("商品级别的折扣(包括直减和最终售价)最终只能有1条，现已经存在1条折扣规则，如需要添加新的规则，请先删除现有折扣规则!");
                        return;
                    }


                    if (vm.PriceDiscountRule == null) vm.PriceDiscountRule = new ObservableCollection<PSPriceDiscountRuleViewModel>();
                    vm.PriceDiscountRule.Add(new PSPriceDiscountRuleViewModel()
                    {
                        DiscountType = vm.IsDiscountFinal.Value ? PSDiscountTypeForProductPrice.ProductPriceFinal : PSDiscountTypeForProductPrice.ProductPriceDiscount,
                        DiscountValue = productAmount,
                        MaxQty = productQty
                    });
                    dgDiscountProduct.ItemsSource = vm.PriceDiscountRule;                   
                }
                else
                {
                    this.CurrentWindow.Alert("限定金额和最大数量必须输入大于或等于0的数字，并且最大数量必须是整数!");                    
                    tbProductAmount.Text = decimal.TryParse(tbProductAmount.Text.Trim(), out productAmount) ? tbProductAmount.Text : "";
                    tbProductQty.Text = int.TryParse(tbProductQty.Text.Trim(), out productQty) ? productQty.ToString() : "";
                    return;
                }
            }
            #endregion
        }

        private void btnRemoveDiscountAmount_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            OrderAmountDiscountRankViewModel item = dgDiscountAmount.SelectedItem as OrderAmountDiscountRankViewModel;
            vm.OrderAmountDiscountRule.OrderAmountDiscountRank.Remove(item);
            //dgDiscountAmount.ItemsSource = vm.OrderAmountDiscountRule.OrderAmountDiscountRank;
        }

        private void btnRemoveDiscountProduct_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            PSPriceDiscountRuleViewModel item = dgDiscountProduct.SelectedItem as PSPriceDiscountRuleViewModel;
            vm.PriceDiscountRule.Remove(item);
            //dgDiscountAmount.ItemsSource = vm.PriceDiscountRule;
        }               
    }
}
