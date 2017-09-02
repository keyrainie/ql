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
    public partial class ProductChannelProductBatchEditDetail : UserControl
    {
        #region 属性
        private ProductChannelMemberFacade _facade;
        #endregion

        #region Property
        public IDialog Dialog { get; set; }
        public List<ProductChannelMemberPriceInfo> ProductChannelMemberPriceInfos { get; set; }
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
        public ProductChannelProductBatchEditDetail()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new ProductChannelMemberVM();
        }
        #endregion

        #region 按钮事件
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ProductChannelMemberVM;
            if (vm == null || !ValidationManager.Validate(this)) return;
            if (String.IsNullOrEmpty(vm.MemberPrice) && String.IsNullOrEmpty(vm.MemberPricePercent))
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(
                "优惠价和优惠比例必须输入一项!.", MessageBoxType.Warning);
                return;
            }
            if (!String.IsNullOrEmpty(vm.MemberPrice) && !String.IsNullOrEmpty(vm.MemberPricePercent))
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(
                "优惠价和优惠比例只能输入一项!.", MessageBoxType.Warning);
                return;
            }
            if (cbIsPriceSave.IsChecked == true && !String.IsNullOrEmpty(vm.MemberPrice))
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(
                "只有优惠比例才能换算为绝对价格!.", MessageBoxType.Warning);
                return;
            }
            foreach (ProductChannelMemberPriceInfo item in ProductChannelMemberPriceInfos)
            {
                if (!String.IsNullOrEmpty(vm.MemberPrice))
                    item.MemberPrice = decimal.Parse(vm.MemberPrice);
                else
                {
                    item.MemberPricePercent = decimal.Parse(vm.MemberPricePercent);
                    if (cbIsPriceSave.IsChecked == true)
                    {
                        item.MemberPrice =
                            item.CurrentPrice * (1 - item.MemberPricePercent / 100);
                        item.MemberPricePercent = null;
                    }
                }
            }

            Facade.UpdateProductChannelMemberPrices(ProductChannelMemberPriceInfos, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    CloseDialog(DialogResultType.OK);
                }
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
        #endregion
    }
}
