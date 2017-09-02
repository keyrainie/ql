using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Linq;
using Newegg.Oversea.Silverlight.Utilities.Validation;
namespace ECCentral.Portal.Basic.Components.UserControls.ProductPicker
{
    public partial class UCProductSearch : UserControl
    {
        #region Properties

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(UCProductSearch),
            new PropertyMetadata(SelectionMode.Single, new PropertyChangedCallback(UCProductSearch.OnSelectionModePropertyChanged)));

        private static void OnSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as UCProductSearch;
            if (control != null)
            {
                control.OnSelectionModePropertyChanged(e);
            }
        }

        void OnSelectionModePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            switch (this.SelectionMode)
            {
                case SelectionMode.Multiple:
                    this.DataGrid.Columns[0].Visibility = Visibility.Collapsed;
                    this.DataGrid.Columns[1].Visibility = Visibility.Visible;
                    this.SelectedListBox.Visibility = Visibility.Visible;
                    break;
                default:
                    this.DataGrid.Columns[0].Visibility = Visibility.Visible;
                    this.DataGrid.Columns[1].Visibility = Visibility.Collapsed;
                    this.SelectedListBox.Visibility = Visibility.Collapsed;
                    break;
            }
        }


        /// <summary>
        /// 应用Window.ShowDialog弹出时，会返回一个DialogHandle,根据这个DialogHandle可以做关闭对话框等操作
        /// </summary>
        public IDialog DialogHandler { get; set; }

        /// <summary>
        /// 查询条件,与UI建立双向绑定
        /// </summary>
        public ProductSimpleQueryVM _viewModel;

        /// <summary>
        /// 当前打开的Tab页面
        /// </summary>
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        private List<ProductVM> _vmList;

        private CheckBox _checkBoxAll;

        /// <summary>
        /// 商品3级类别查询。
        /// </summary>
        public int? ProductC3SysNo
        {
            get;
            set;
        }
        #endregion

        public UCProductSearch()
        {
            InitializeComponent();
            if(null==_viewModel)
            _viewModel = new ProductSimpleQueryVM();
            this.Loaded += new RoutedEventHandler(UCProductSearch_Loaded);
        }

        public UCProductSearch(int vendorSysNo, string vendorName)
            : this()
        {
            if (null == _viewModel)
            {
                _viewModel = new ProductSimpleQueryVM();
            }
            _viewModel.VendorSysNo = vendorSysNo;
            _viewModel.VendorName = vendorName;
        }

        void UCProductSearch_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(UCProductSearch_Loaded);

            this.Grid.DataContext = _viewModel;

            CodeNamePairHelper.GetList("IM", "ProductConsignFlag", CodeNamePairAppendItemType.All, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    cmbConsignFlagList.ItemsSource = args.Result;
                }
            });
        }


        #region Event Handlers

        /// <summary>
        /// 统一处理查询区域中TextBox的回车执行查询事件
        /// </summary>
        void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                var txtBinding = textBox.GetBindingExpression(TextBox.TextProperty);
                if (txtBinding != null)
                {
                    txtBinding.UpdateSource();
                    DataGridBind();
                }
            }
        }

        /// <summary>
        /// 请求服务，并将查询结果绑定到DataGrid
        /// </summary>
        void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //1.初始化查询条件，分页信息
            //2.请求服务查询
            PagingInfo p = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            _viewModel.C3SysNo = ProductC3SysNo;
            ProductQueryFacade facade = new ProductQueryFacade(CPApplication.Current.CurrentPage);
            facade.QueryProduct(_viewModel, p, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    BindDataGrid(args.Result.TotalCount, args.Result.Rows);
                });
        }

        public void BindDataGrid(int totalCount, dynamic rows)
        {
            this.DataGrid.TotalCount = totalCount;
            _vmList = DynamicConverter<ProductVM>.ConvertToVMList<List<ProductVM>>(rows);
            this.DataGrid.ItemsSource = _vmList;
            if (_checkBoxAll != null)
            {
                //重新查询后，将全选CheckBox置为false
                _checkBoxAll.IsChecked = false;
            }
            if (this.SelectionMode == System.Windows.Controls.SelectionMode.Multiple)
            {
                _vmList.ForEach(item =>
                {
                    item.onIsCheckedChange = new ProductIsCheckedChange(IsCheckChanged);
                });
            }
        }

        private void IsCheckChanged(bool ischecked, ProductVM vm)
        {
            if (ischecked)//添加
            {
                if (_viewModel.SelectedProducts.Where(p => p.SysNo == vm.SysNo).Count() <= 0)
                {
                    _viewModel.SelectedProducts.Add(vm);
                    SelectedListBox.ItemsSource = _viewModel.SelectedProducts;
                }
            }
            else //移除
            {
                if (_viewModel.SelectedProducts.Where(p => p.SysNo == vm.SysNo).Count() > 0)
                {
                    _viewModel.SelectedProducts.Remove(_viewModel.SelectedProducts.Where(p => p.SysNo == vm.SysNo).First());
                    SelectedListBox.ItemsSource = _viewModel.SelectedProducts;
                }
            }
            tblSelectedTotal.Text = string.Format("{0}件商品已选择", _viewModel.SelectedProducts.Count);
        }
        /// <summary>
        /// 查询按钮事件，直接调用DataGrid的Bind方法，DataGrid内部会触发LoadingDataSource事件
        /// </summary>
        void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            DataGridBind();
        }

        private void DataGridBind()
        {
            ValidationManager.Validate(this.filterGrid);
            if (!_viewModel.HasValidationErrors)
            {
                this.DataGrid.Bind();
            }
        }
        void ButtonConfirmSelected_Click(object sender, RoutedEventArgs e)
        {
            if (this.DialogHandler == null)
                return;
            if (_vmList == null)
            {
                this.CurrentWindow.Alert(ResProductPicker.Msg_PleaseSelect, MessageType.Warning);
                return;
            }
            //获取用户选择的记录列表
            List<ProductVM> selectedList = new List<ProductVM>();
            if (this.SelectionMode == SelectionMode.Single)
            {
                foreach (var item in _vmList)
                {
                    if (item.IsChecked)
                    {
                        selectedList.Add(item);
                    }
                }
            }
            else
            {
                _viewModel.SelectedProducts.ForEach(item =>
                {
                    selectedList.Add(item);
                });
            }
            if (selectedList.Count == 0)
            {
                this.CurrentWindow.Alert(ResProductPicker.Msg_PleaseSelect, MessageType.Warning);
                return;
            }
            //关闭对话框并返回数据
            if (this.SelectionMode == SelectionMode.Single)
            {
                this.DialogHandler.ResultArgs.Data = selectedList[0];
            }
            else if (this.SelectionMode == SelectionMode.Multiple)
            {
                this.DialogHandler.ResultArgs.Data = selectedList;
            }
            this.DialogHandler.ResultArgs.DialogResult = DialogResultType.OK;
            this.DialogHandler.Close();
        }

        void ButtonCloseDialog_Click(object sender, RoutedEventArgs e)
        {
            if (this.DialogHandler == null)
                return;

            this.DialogHandler.ResultArgs.DialogResult = DialogResultType.Cancel;
            this.DialogHandler.Close();
        }

        private void Hyperlink_ProductNumber_Click(object sender, RoutedEventArgs e)
        {
            var lnk = sender as FrameworkElement;
            if (null != lnk)
            {
                CurrentWindow.Navigate(string.Format(ConstValue.IM_ProductMaintainUrlFormat, lnk.Tag.ToString()), null, true);
            }
        }

        /// <summary>
        /// DataGrid中当前页全选，全不选事件处理器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            var checkBoxAll = sender as CheckBox;
            if (_vmList == null || checkBoxAll == null)
                return;
            _vmList.ForEach(item =>
                {
                    item.IsChecked = checkBoxAll.IsChecked ?? false;
                });
        }
        #endregion

        private void DataGridCheckBoxAll_Loaded(object sender, RoutedEventArgs e)
        {
            _checkBoxAll = sender as CheckBox;
        }

        private void ckb_MoreQueryBuilder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void hlbDelete_Click(object sender, RoutedEventArgs e)
        {
            var sysNo = (int)(sender as HyperlinkButton).Tag;
            if (_viewModel.SelectedProducts.Where(p => p.SysNo == sysNo).Count() > 0)
            {
                _viewModel.SelectedProducts.Remove(_viewModel.SelectedProducts.Where(p => p.SysNo == sysNo).First());
                SelectedListBox.ItemsSource = _viewModel.SelectedProducts;
            }
            tblSelectedTotal.Text = string.Format("{0}件商品已选择", _viewModel.SelectedProducts.Count);
        }
    }
}
