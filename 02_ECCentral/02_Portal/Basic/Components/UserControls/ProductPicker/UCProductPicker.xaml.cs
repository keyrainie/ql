using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.Basic.Components.UserControls.ProductPicker
{
    public partial class UCProductPicker : UserControl
    {
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public UCProductPicker()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //将UC内部依赖属性上的绑定传递到控件。
            var ProductIDBindingExp = this.GetBindingExpression(UCProductPicker.ProductIDProperty);
            if (ProductIDBindingExp != null && ProductIDBindingExp.ParentBinding != null)
            {
                txtProductID.SetBinding(TextBox.TextProperty, ProductIDBindingExp.ParentBinding);
            }
            var ProductSysNoBindingExp = this.GetBindingExpression(UCProductPicker.ProductSysNoProperty);
            if (ProductSysNoBindingExp != null && ProductSysNoBindingExp.ParentBinding != null)
            {
                txtProductSysNo.SetBinding(TextBox.TextProperty, ProductSysNoBindingExp.ParentBinding);
            }
            this.orginProductID = string.Empty;
        }

        /// <summary>
        /// 所选定商品的详细信息
        /// </summary>
        private ProductVM m_selectedProductInfo;
        public ProductVM SelectedProductInfo
        {
            get
            {
                return m_selectedProductInfo;
            }
        }
        /// <summary>
        /// 依赖属性：商品ID,支持Silverlight绑定等特性
        /// </summary>
        public string ProductID
        {
            get
            {
                string value = (string)GetValue(ProductIDProperty) ?? "";
                if (value.Trim().Length == 0)
                {
                    value = this.txtProductID.Text.Trim();
                }
                return value;
            }
            set
            {
                SetValue(ProductIDProperty, value);
            }
        }
        public static readonly DependencyProperty ProductIDProperty =
            DependencyProperty.Register("ProductID", typeof(string), typeof(UCProductPicker), new PropertyMetadata(null, (s, e) =>
                {
                    var uc = s as UCProductPicker;
                    uc.txtProductID.Text = (e.NewValue ?? "").ToString().Trim();
                }));

        /// <summary>
        /// 依赖属性：商品系统编号,支持Silverlight绑定等特性
        /// </summary>
        public string ProductSysNo
        {
            get
            {
                string value = (GetValue(ProductSysNoProperty) ?? "").ToString();
                if (value.Trim().Length == 0)
                {
                    value = this.txtProductSysNo.Text.Trim();
                }
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }
                return value;
            }
            set
            {
                SetValue(ProductSysNoProperty, value);
                if (value != null)
                {
                    LoadProductBySysNo();
                }
            }
        }
        public static readonly DependencyProperty ProductSysNoProperty =
            DependencyProperty.Register("ProductSysNo", typeof(string), typeof(UCProductPicker), new PropertyMetadata(null, (s, e) =>
                {
                    var uc = s as UCProductPicker;
                    uc.txtProductSysNo.Text = (e.NewValue ?? "").ToString().Trim();
                    if (e.NewValue == null)
                    {
                        uc.m_selectedProductInfo = null;
                        uc.orginProductID = string.Empty;
                    }
                }));

        /// <summary>
        /// 商品选中事件
        /// </summary>
        public event EventHandler<ProductSelectedEventArgs> ProductSelected;

        private void OnProductSelected(ProductVM selectedProduct)
        {
            var handler = ProductSelected;
            if (handler != null)
            {
                var args = new ProductSelectedEventArgs(selectedProduct);
                handler(this, args);
            }
        }

        #region ProductID event

        private string orginProductID = string.Empty;
        private void txtProductID_GotFocus(object sender, RoutedEventArgs e)
        {
            orginProductID = this.txtProductID.Text.Trim();
        }

        private void txtProductID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ProductIDPreCheck())
            {
                LoadProductByID();
            }
        }
        private void txtProductID_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            if (ProductIDPreCheck())
            {
                LoadProductByID();
                orginProductID = this.txtProductID.Text.Trim();
                e.Handled = true;
            }
        }

        private bool ProductIDPreCheck()
        {
            if (string.IsNullOrEmpty(this.txtProductID.Text.Trim()))//值为空不查询
            {
                this.txtProductSysNo.Text = string.Empty;
                return false;
            }
            if (this.txtProductID.Text.Trim() == orginProductID)//值不变不查询
                return false;
            return true;
        }

        public void LoadProductByID()
        {
            this.txtProductSysNo.ClearValidationError();
            PagingInfo p = new PagingInfo
            {
                PageIndex = 0,
                PageSize = 25
            };

            ProductSimpleQueryVM queryReq = new ProductSimpleQueryVM() { ProductID = this.txtProductID.Text.Trim(), CompanyCode = CPApplication.Current.CountryCode };
            new ProductQueryFacade(CPApplication.Current.CurrentPage).QueryProduct(queryReq, p, OnLoadProductByID);
        }

        private void OnLoadProductByID(object sender, RestClientEventArgs<dynamic> args)
        {
            if (args.Result != null)
            {
                dynamic totalCount = args.Result.TotalCount;
                if (totalCount == 0)
                {
                    //商品ID不存在
                    this.txtProductID.Text = string.Empty;
                    this.txtProductSysNo.Text = string.Empty;
                }
                else if (totalCount > 1)
                {
                    //同一商品ID存在多个
                    UCProductSearch ucProductSearch = new UCProductSearch();
                    ucProductSearch.SelectionMode = SelectionMode.Single;
                    ucProductSearch._viewModel.ProductID = this.txtProductID.Text.Trim();
                    ucProductSearch.BindDataGrid(totalCount, args.Result.Rows);
                    ucProductSearch.DialogHandler = CurrentWindow.ShowDialog(ResProductPicker.Dialog_Title, ucProductSearch, OnDialogResult);
                }
                else
                {
                    //商品ID只存在一个
                    ProductVM selectedProduct = DynamicConverter<ProductVM>.ConvertToVM(args.Result.Rows[0]);
                    this.txtProductID.Text = selectedProduct.ProductID;
                    this.txtProductSysNo.Text = selectedProduct.SysNo.ToString();
                    this.m_selectedProductInfo = selectedProduct;
                    OnProductSelected(selectedProduct);
                    this.txtProductID.Focus();
                }
            }
        }
        #endregion

        #region productsysno event
        private string orginProductSysNo = string.Empty;

        private void txtProductSysNo_GotFocus(object sender, RoutedEventArgs e)
        {
            orginProductSysNo = this.txtProductSysNo.Text.Trim();
        }

        private void txtProductSysNo_LostFocus(object sender, RoutedEventArgs e)
        {
            bool? result = ProductSysNoPreCheck();
            if (result != null && result.Value)
            {
                LoadProductBySysNo();
            }
        }
        private void txtProductSysNo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            bool? result = ProductSysNoPreCheck();
            if (result != null && result.Value)
            {
                LoadProductBySysNo();
                orginProductSysNo = this.txtProductSysNo.Text.Trim();
                e.Handled = true;
            }
            else if (result == null)
            {
                e.Handled = true;
            }

        }
        private bool? ProductSysNoPreCheck()
        {
            if (string.IsNullOrEmpty(this.txtProductSysNo.Text.Trim()))//值为空不查询
            {
                this.txtProductSysNo.ClearValidationError();
                this.txtProductID.Text = string.Empty;
                this.ProductSysNo = null;
                return false;
            }
            int sysno;
            if (!int.TryParse(this.txtProductSysNo.Text.Trim(), out sysno))//不为int型也需要清空
            {
                this.txtProductSysNo.Validation("请输入一个有效的正整数");
                this.txtProductSysNo.Text = string.Empty;
                this.ProductSysNo = null;
                this.txtProductID.Text = string.Empty;
                return null;
            }
            if (this.txtProductSysNo.Text.Trim() == orginProductSysNo)//值不变不查询
                return false;

            return true;
        }
        public void LoadProductBySysNo()
        {
            this.txtProductSysNo.ClearValidationError();
            PagingInfo p = new PagingInfo
            {
                PageIndex = 0,
                PageSize = 25
            };
            ProductSimpleQueryVM queryReq = new ProductSimpleQueryVM() { ProductSysNo = this.txtProductSysNo.Text.Trim(), CompanyCode = CPApplication.Current.CountryCode };
            new ProductQueryFacade(CPApplication.Current.CurrentPage).QueryProduct(queryReq, p, OnLoadProductBySysNo);
        }

        private void OnLoadProductBySysNo(object sender, RestClientEventArgs<dynamic> args)
        {
            if (args.Result == null || args.Result.TotalCount == 0)
            {
                //商品系统编号不存在
                this.txtProductID.Text = string.Empty;
                this.txtProductSysNo.Text = string.Empty;
            }
            else
            {
                //商品系统编号只存在一个
                ProductVM selectedProduct = DynamicConverter<ProductVM>.ConvertToVM(args.Result.Rows[0]);
                this.txtProductSysNo.Text = selectedProduct.SysNo.ToString();
                this.txtProductID.Text = selectedProduct.ProductID;
                this.txtProductSysNo.Focus();
                this.m_selectedProductInfo = selectedProduct;
                OnProductSelected(selectedProduct);
            }
        }
        #endregion

        #region searchbutton event
        void ImageProductPicker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UCProductSearch ucProductSearch = new UCProductSearch();
            ucProductSearch.SelectionMode = SelectionMode.Single;
            ucProductSearch.DialogHandler = CurrentWindow.ShowDialog(ResProductPicker.Dialog_Title, ucProductSearch, OnDialogResult);
        }
        private void OnDialogResult(object sender, ResultEventArgs e)
        {
            if (e.DialogResult == DialogResultType.OK)
            {
                var selectedProduct = e.Data as ProductVM;
                if (selectedProduct != null)
                {
                    this.txtProductID.Text = selectedProduct.ProductID;
                    this.txtProductSysNo.Text = selectedProduct.SysNo.ToString();
                    this.m_selectedProductInfo = selectedProduct;
                    OnProductSelected(selectedProduct);
                    this.txtProductID.Focus();
                }
            }
        }
        #endregion

        /// <summary>
        /// 为控件赋初值
        /// </summary>
        /// <param name="productSysNo"></param>
        public void SetProductSysNo(int productSysNo)
        {
            this.txtProductSysNo.Text = productSysNo.ToString();
            bool? result = ProductSysNoPreCheck();
            if (result != null && result.Value)
            {
                LoadProductBySysNo();
            }
        }
    }
}
