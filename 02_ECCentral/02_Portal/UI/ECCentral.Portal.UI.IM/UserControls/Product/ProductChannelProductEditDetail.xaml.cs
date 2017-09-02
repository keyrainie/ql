using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Windows.Media;
using System.Text.RegularExpressions;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductChannelProductEditDetail : UserControl
    {
        #region 属性
        private ProductChannelMemberFacade _facade;
        #endregion

        #region Property
        public Int32? SysNo { get; set; }
        public IDialog Dialog { get; set; }
        protected ProductChannelMemberFacade Facade
        {
            get
            {
                if (_facade == null)
                {
                    _facade = new ProductChannelMemberFacade();
                }
                return _facade;
            }
        }
        #endregion


        #region 初始化加载
        public ProductChannelProductEditDetail()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BindPage();
        }
        #endregion

        #region 查询绑定
        private void BindPage()
        {
            if (SysNo != null)
            {
                Facade.GetProductChannelMemberPriceBySysNo(SysNo.Value, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    if (args.Result == null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(
                            "无法渠道商品信息.", MessageBoxType.Warning);
                        return;
                    }
                    var vm = args.Result.Convert<ProductChannelMemberPriceInfo, ProductChannelMemberVM>();
                    vm.CurrentPrice = decimal.Parse(vm.CurrentPrice.ToString("0.00"));
                    vm.MemberPricePercent = !String.IsNullOrEmpty(vm.MemberPricePercent) 
                        ? vm.MemberPricePercent.Split('.')[0] : String.Empty;
                    DataContext = vm;
                });
            }
        }
        #endregion

        #region 按钮事件
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ProductChannelMemberVM;
            if (vm == null || !ValidationManager.Validate(this)) return;
            if (!String.IsNullOrEmpty(vm.MemberPrice) && !String.IsNullOrEmpty(vm.MemberPricePercent))
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(
                    "优惠价和优惠比例只能输入一项!.", MessageBoxType.Warning);
                return;
            }
            ProductChannelMemberPriceInfo info = new ProductChannelMemberPriceInfo()
            {
                SysNo = vm.SysNo,
                ProductSysNo = vm.ProductSysNo,
                ChannelName = vm.ChannelName,
                EditDate = DateTime.Now,
                EditUser = CPApplication.Current.LoginUser.LoginName,
                CompanyCode = CPApplication.Current.CompanyCode,
                StoreCompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = CPApplication.Current.LanguageCode
            };
            if (!String.IsNullOrEmpty(vm.MemberPrice))
            {
                info.MemberPrice = decimal.Parse(vm.MemberPrice);
            }
            else
            {
                info.MemberPricePercent = decimal.Parse(vm.MemberPricePercent);
            }
            Facade.UpdateProductChannelMemberPrice(info, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                CPApplication.Current.CurrentPage
                    .Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
                CloseDialog(DialogResultType.OK);
            });

        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }
        private void cbIsAppointInventory_Checked(object sender, RoutedEventArgs e)
        {
        }
        private void dplistStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void tb_MemberPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            Regex rgx = new Regex(@"^0$|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$");
            tb_Discount1.Text =
                string.IsNullOrEmpty(tb_MemberPrice.Text) || !rgx.IsMatch(tb_MemberPrice.Text)
                ? string.Empty : (decimal.Parse(tb_CurrentPrice.Text)
                - decimal.Parse(tb_MemberPrice.Text)).ToString("0.00");
        }

        private void tb_MemberPricePercent_LostFocus(object sender, RoutedEventArgs e)
        {
            Regex rgx = new Regex(@"^100$|^0$|^[1-9]\d{0,1}$");
            tb_Discount2.Text =
                   string.IsNullOrEmpty(tb_MemberPricePercent.Text) || !rgx.IsMatch(tb_MemberPricePercent.Text)
                    ? string.Empty
                    : ((decimal.Parse(tb_CurrentPrice.Text) * (1 - decimal.Parse(tb_MemberPricePercent.Text) / 100)))
                    .ToString("0.00");
        }
        #endregion
    }
}
