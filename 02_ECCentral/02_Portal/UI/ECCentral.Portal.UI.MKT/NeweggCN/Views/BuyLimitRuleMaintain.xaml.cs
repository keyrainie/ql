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
    public partial class BuyLimitRuleMaintain : PageBase
    {
        private bool _isEditing;
        private List<CheckBoxVM> cbList;
        public BuyLimitRuleMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            //绑定会员等级
            var customerRanks = EnumConverter.GetKeyValuePairs<CustomerRank>();
            cbList = new List<CheckBoxVM>(customerRanks.Count);
            foreach (var kv in customerRanks)
            {
                cbList.Add(new CheckBoxVM
                {
                    IsChecked = true,
                    Name = kv.Value,
                    SysNo = (int)kv.Key
                });
            }
            listMemberRanks.ItemsSource = cbList;

            this.lstStatus.ItemsSource = EnumConverter.GetKeyValuePairs<LimitStatus>();
            this.lstLimitType.ItemsSource = EnumConverter.GetKeyValuePairs<LimitType>();
            if (string.IsNullOrEmpty(this.Request.Param))
            {
                _isEditing = false;
                var vm = new BuyLimitRuleVM();
                vm.MinQty = "1";
                vm.MaxQty = "99999";
                vm.OrderTimes = "99999";
                this.DataContext = vm;
                this.Title = ResBuyLimitRule.PageTitle_Add;
            }
            else
            {
                _isEditing = true;
                this.Title = ResBuyLimitRule.PageTitle_Edit;
                new BuyLimitRuleFacade(this).Load(this.Request.Param, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    this.DataContext = args.Result.Convert<BuyLimitRule, BuyLimitRuleVM>((info, vm) =>
                    {
                        if (info.LimitType == LimitType.Combo)
                        {
                            vm.ComboSysNo = info.ItemSysNo.ToString();
                        }
                        else
                        {
                            vm.ProductSysNo = info.ItemSysNo.ToString();
                        }
                        //根据数据中会员等级设置值更新界面
                        if (!string.IsNullOrWhiteSpace(vm.MemberRanks))
                        {
                            var arr = vm.MemberRanks.Split(',');
                            foreach (var cb in cbList)
                            {
                                var found = arr.FirstOrDefault(item => cb.SysNo.ToString() == item);
                                if (found != null)
                                {
                                    cb.IsChecked = true;
                                }
                                else
                                {
                                    cb.IsChecked = false;
                                }
                            }
                        }
                    });
                });
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
                return;

            var viewModel = this.DataContext as BuyLimitRuleVM;
            if (viewModel.LimitType == LimitType.SingleProduct)
            {
                int productSysNo;
                if (!int.TryParse(viewModel.ProductSysNo, out productSysNo))
                {
                    Window.Alert("请输入正确的商品信息。");
                    return;
                }
            }
            else
            {
                int comboSysNo;
                if (!int.TryParse(viewModel.ComboSysNo, out comboSysNo))
                {
                    Window.Alert("请输入正确的套餐编号。");
                    return;
                }
            }
            //收集界面上会员等级设置
            string memberRanks = "";
            foreach (var cb in cbList)
            {
                if (cb.IsChecked)
                {
                    memberRanks += cb.SysNo.ToString() + ",";
                }
            }
            viewModel.MemberRanks = memberRanks.TrimEnd(',');
            if (viewModel.MemberRanks.Length == 0)
            {
                Window.Alert("会员等级至少要设置一项。");
                return;
            }
            if (_isEditing)
            {
                new BuyLimitRuleFacade(this).Update(viewModel, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    AlertAndBack(ResBuyLimitRule.Info_EditSuccess);
                });
            }
            else
            {
                new BuyLimitRuleFacade(this).Create(viewModel, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    AlertAndBack(ResBuyLimitRule.Info_AddSuccess);
                });
            }
        }

        private void lstLimitType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstLimitType.SelectedIndex == 0)
            {
                lblProduct.Visibility = Visibility.Visible;
                ucProductPicker.Visibility = Visibility.Visible;
                lblComboSysNo.Visibility = Visibility.Collapsed;
                txtComboSysNo.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblProduct.Visibility = Visibility.Collapsed;
                ucProductPicker.Visibility = Visibility.Collapsed;
                lblComboSysNo.Visibility = Visibility.Visible;
                txtComboSysNo.Visibility = Visibility.Visible;
            }
        }

        private void AlertAndBack(string msg)
        {
            Window.Close();
            Window.Navigate(ConstValue.MKT_BuyLimitRuleQuery, "refresh");
            Window.Alert(msg);
        }
    }
}
