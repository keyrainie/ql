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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.Common.Models;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Common.Facades;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Common.Resources;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.Basic.Components.UserControls.CategoryPicker;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Common.Views
{
    [View]
    public partial class ShipTypeProductAddNew :PageBase
    {
        public ShipTypeProductInfoVM _viewVM;
        public ShipTypeFacade _shipTypeProductFace;
        public List<ProductVM> ProductList
        {
            get;set;
        }
        public ShipTypeProductAddNew()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _viewVM = new ShipTypeProductInfoVM();
            _shipTypeProductFace = new ShipTypeFacade(this);
            grid_Field.DataContext = _viewVM;
            InitControle();
        }

        #region 弹出框处理事件
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var window = CPApplication.Current.CurrentPage.Context.Window;
            UCProductSearch ucProductSearch = new UCProductSearch();
            ucProductSearch.SelectionMode = SelectionMode.Multiple;
            ucProductSearch.DialogHandler = window.ShowDialog(ResProductPicker.Dialog_Title, ucProductSearch, OnProductDialogResult);

        }
        private void OnProductDialogResult(object sender, ResultEventArgs e)
        {
            if (e.DialogResult ==DialogResultType.OK)
            {
                  ProductList = e.Data as List<ProductVM>;
                  _viewVM.ListProductInfo = ProductList;
                  grid_Field.DataContext = _viewVM;

            }
        }

        private void AddProductType_Click(object sender, RoutedEventArgs e)
        {
            var window = CPApplication.Current.CurrentPage.Context.Window;
            UCCategoryQuery UCCategoryQuery = new UCCategoryQuery();
            UCCategoryQuery.DialogHandler = window.ShowDialog("商品类型查询", UCCategoryQuery, (s,args)=>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        _viewVM.ListCategoryInfo = args.Data as List<CategoryVM>;
                        grid_Field.DataContext = _viewVM;
                    }
                });
        }
        #endregion

        #region UI事件
        private void ItemRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewVM.ListProductInfo = null;
            _viewVM.ListCategoryInfo = null;
            switch (this._viewVM.ProductRange)
            {
                case ProductRange.Goods:
                    AddProduct.Visibility=Visibility.Visible;
                    AddProductType.Visibility=Visibility.Collapsed;
                    ItemList.Visibility = Visibility.Visible;
                    CategoryList.Visibility = Visibility.Collapsed;
                    break;
                case ProductRange.GoodsType:
                    AddProduct.Visibility=Visibility.Collapsed;
                    AddProductType.Visibility=Visibility.Visible;
                    CategoryList.Visibility = Visibility.Visible;
                    ItemList.Visibility = Visibility.Collapsed;
                    break;

            }
        }
        #endregion

        private void CreateNew_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.grid_Field))
            {
                switch (this._viewVM.ProductRange)
                {
                    case ProductRange.Goods:
                        if  (ItemList.Items.Count<=0)
                        {
                            Window.Alert("请选择商品!");
                            return;
                        }
                        _shipTypeProductFace.CreateShipTypeProduct(_viewVM, (s, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert("操作已成功!");
                        });
                        break;
                    case ProductRange.GoodsType:
                        if (CategoryList.Items.Count<=0)
                        {
                            Window.Alert("请选择商品类型！");
                            return;
                        }
                        _shipTypeProductFace.CreateShipTypeProduct(_viewVM, (s, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert("操作已成功!");
                        });
                        break;
                }
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _viewVM = new ShipTypeProductInfoVM();
            grid_Field.DataContext = _viewVM;
            InitControle();
        }

        #region 初始化控件
        private void InitControle()
        {
            ItemType.SelectedIndex = 0;
            ItemRange.SelectedIndex = 0;

            //商户:
            this.Merchant.ItemsSource = EnumConverter.GetKeyValuePairs<CompanyCustomer>(EnumConverter.EnumAppendItemType.All);
            this.Merchant.SelectedIndex = 0;
        }
        #endregion
    }
}
