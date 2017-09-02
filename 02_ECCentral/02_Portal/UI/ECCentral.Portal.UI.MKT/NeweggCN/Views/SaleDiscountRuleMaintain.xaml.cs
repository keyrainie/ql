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
using System.Windows.Navigation;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.NeweggCN.Facades;
using ECCentral.Portal.UI.MKT.NeweggCN.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.NeweggCN.Models;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.MKT.UserControls;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.NeweggCN.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url, NeedAccess = false)]
    public partial class SaleDiscountRuleMaintain : PageBase
    {
        private bool _isEditing;
        public SaleDiscountRuleMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.lstStatus.ItemsSource = EnumConverter.GetKeyValuePairs<SaleDiscountRuleStatus>();
            this.listRuleType.ItemsSource = EnumConverter.GetKeyValuePairs<SaleDiscountRuleType>();
            if (string.IsNullOrEmpty(this.Request.Param))
            {
                _isEditing = false;
                var vm = new SaleDiscountRuleVM();
                vm.MinQty = "1";
                vm.MaxQty = "99999";
                vm.MinAmt = "1";
                vm.MaxAmt = "99999";
                vm.BeginDate = DateTime.Now;
                this.DataContext = vm;
                this.Title = ResSaleDiscountRule.PageTitle_Add;
                ControlByLimitType();
            }
            else
            {
                _isEditing = true;
                this.Title = ResSaleDiscountRule.PageTitle_Edit;
                new SaleDiscountRuleFacade(this).Load(this.Request.Param, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    var result = args.Result.Convert<SaleDiscountRule, SaleDiscountRuleVM>();
                    ControlByRuleType(result.RuleType);
                    ucCategoryPicker.LoadCategoryCompleted += (cs, ca) =>
                    {
                        if (result.C3SysNo > 0)
                        {
                            ucCategoryPicker.Category3SysNo = result.C3SysNo;
                        }
                    };
                    if (result.BrandSysNo <= 0)
                    {
                        ucBrandPicker.SelectedBrandSysNo = null;
                    }
                    if (result.ProductSysNo <= 0)
                    {
                        ucProductPicker.ProductSysNo = null;
                    }
                    //限定分类+品牌
                    if (result.C3SysNo > 0 && result.BrandSysNo > 0)
                    {
                        this.rbLimitCategoryBrand.IsChecked = true;
                    }
                    //限定分类
                    else if (result.C3SysNo > 0)
                    {
                        this.rbLimitCategory.IsChecked = true;
                    }
                    //限定品牌
                    else if (result.BrandSysNo > 0)
                    {
                        this.rbLimitBrand.IsChecked = true;
                    }
                    //限定商品组
                    else if (result.ProductSysNo > 0)
                    {
                        this.rbLimitProduct.IsChecked = true;
                    }
                    ControlByLimitType();
                    this.DataContext = result;
                });
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
                return;

            var viewModel = this.DataContext as SaleDiscountRuleVM;
            viewModel.C3SysNo = ucCategoryPicker.ChooseCategory3SysNo;
            if (this.rbLimitCategory.IsChecked == true && !viewModel.IsC3SysNoValid())
            {
                viewModel.BrandSysNo = null;
                viewModel.ProductSysNo = null;
                Window.Alert("请设置商品三级分类。");
                return;
            }

            if (this.rbLimitBrand.IsChecked == true && !viewModel.IsBrandSysNoValid())
            {
                viewModel.C3SysNo = null;
                viewModel.ProductSysNo = null;
                Window.Alert("请设置商品品牌。");
                return;
            }
            if (this.rbLimitProduct.IsChecked == true && !viewModel.IsProductSysNoValid())
            {
                viewModel.C3SysNo = null;
                viewModel.BrandSysNo = null;
                Window.Alert("请设置商品。");
                return;
            }
            if (this.rbLimitCategoryBrand.IsChecked == true)
            {
                viewModel.ProductSysNo = null;
                if (!viewModel.IsC3SysNoValid())
                {
                    Window.Alert("请设置商品三级分类。");
                    return;
                }
                if (!viewModel.IsBrandSysNoValid())
                {
                    Window.Alert("请设置商品品牌。");
                    return;
                }
            }

            if (_isEditing)
            {
                new SaleDiscountRuleFacade(this).Update(viewModel, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    AlertAndBack(ResSaleDiscountRule.Info_EditSuccess);
                });
            }
            else
            {
                new SaleDiscountRuleFacade(this).Create(viewModel, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    AlertAndBack(ResSaleDiscountRule.Info_AddSuccess);
                });
            }
        }

        private void listRuleType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ControlByRuleType((SaleDiscountRuleType)listRuleType.SelectedIndex);
        }

        private void ControlByRuleType(SaleDiscountRuleType ruleType)
        {
            if (ruleType == SaleDiscountRuleType.AmountRule)
            {
                lblMinQty.Visibility = Visibility.Collapsed;
                txtMinQty.Visibility = Visibility.Collapsed;
                lblMaxQty.Visibility = Visibility.Collapsed;
                txtMaxQty.Visibility = Visibility.Collapsed;
                lblMinAmt.Visibility = Visibility.Visible;
                txtMinAmt.Visibility = Visibility.Visible;
                lblMaxAmt.Visibility = Visibility.Visible;
                txtMaxAmt.Visibility = Visibility.Visible;
            }
            else
            {
                lblMinQty.Visibility = Visibility.Visible;
                txtMinQty.Visibility = Visibility.Visible;
                lblMaxQty.Visibility = Visibility.Visible;
                txtMaxQty.Visibility = Visibility.Visible;
                lblMinAmt.Visibility = Visibility.Collapsed;
                txtMinAmt.Visibility = Visibility.Collapsed;
                lblMaxAmt.Visibility = Visibility.Collapsed;
                txtMaxAmt.Visibility = Visibility.Collapsed;
            }
        }

        private void ControlByLimitType()
        {
            ucCategoryPicker.Visibility = Visibility.Collapsed;
            ucCategoryPickerLabel.Visibility = Visibility.Collapsed;
            ucBrandPicker.Visibility = Visibility.Collapsed;
            ucBrandPickerLabel.Visibility = Visibility.Collapsed;
            ucProductPicker.Visibility = Visibility.Collapsed;
            ucProductPickerLabel.Visibility = Visibility.Collapsed;

            if (rbLimitCategory.IsChecked == true)
            {
                ucCategoryPicker.Visibility = Visibility.Visible;
                ucCategoryPickerLabel.Visibility = Visibility.Visible;
            }
            else if (rbLimitBrand.IsChecked == true)
            {
                ucBrandPicker.Visibility = Visibility.Visible;
                ucBrandPickerLabel.Visibility = Visibility.Visible;
            }
            else if (rbLimitCategoryBrand.IsChecked == true)
            {
                ucCategoryPicker.Visibility = Visibility.Visible;
                ucCategoryPickerLabel.Visibility = Visibility.Visible;
                ucBrandPicker.Visibility = Visibility.Visible;
                ucBrandPickerLabel.Visibility = Visibility.Visible;
            }
            else if (rbLimitProduct.IsChecked == true)
            {
                ucProductPicker.Visibility = Visibility.Visible;
                ucProductPickerLabel.Visibility = Visibility.Visible;
            }
        }

        private void AlertAndBack(string msg)
        {
            Window.Close();
            Window.Navigate(ConstValue.MKT_SaleDiscountRuleQuery, "refresh");
            Window.Alert(msg);
        }

        private void rbLimitType_Click(object sender, RoutedEventArgs e)
        {
            ControlByLimitType();
        }
    }
}
