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
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Portal.UI.IM.Models.Product;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductERPCategorySearch : UserControl
    {
        #region Properties

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(ProductERPCategorySearch),
            new PropertyMetadata(SelectionMode.Single, new PropertyChangedCallback(ProductERPCategorySearch.OnSelectionModePropertyChanged)));

        private static void OnSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// 应用Window.ShowDialog弹出时，会返回一个DialogHandle,根据这个DialogHandle可以做关闭对话框等操作
        /// </summary>
        public IDialog DialogHandler { get; set; }

        /// <summary>
        /// 查询条件,与UI建立双向绑定
        /// </summary>
        public ProductERPCategoryQueryVM _viewModel;

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

        private List<ProductERPCategoryVM> _vmList;

        #endregion

        public ProductERPCategorySearch()
        {
            InitializeComponent();
            if (null == _viewModel)
                _viewModel = new ProductERPCategoryQueryVM();
            this.Loaded += new RoutedEventHandler(ProductERPCategorySearch_Loaded);
        }

        void ProductERPCategorySearch_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(ProductERPCategorySearch_Loaded);

            this.Grid.DataContext = _viewModel;
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
            ProductERPCategoryQueryFacade facade = new ProductERPCategoryQueryFacade(CPApplication.Current.CurrentPage);
            facade.QueryProductERPCategoryList(_viewModel, p, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;
                BindDataGrid(args.Result.TotalCount, args.Result.Rows);
                //this.DataGrid.TotalCount = args.Result.TotalCount;
                //this.DataGrid.ItemsSource = args.Result.Rows;
            });
        }

        public void BindDataGrid(int totalCount, dynamic rows)
        {
            this.DataGrid.TotalCount = totalCount;
            _vmList = DynamicConverter<ProductERPCategoryVM>.ConvertToVMList<List<ProductERPCategoryVM>>(rows);
            this.DataGrid.ItemsSource = _vmList;
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

            //获取用户选择的记录
            List<ProductERPCategoryVM> selectedList = new List<ProductERPCategoryVM>();
            foreach (var item in _vmList)
            {
                if (item.IsChecked)
                {
                    selectedList.Add(item);
                }
            }

            if (selectedList.Count == 0)
            {
                this.CurrentWindow.Alert(ResProductERPCategorySearch.Msg_PleaseSelect, MessageType.Warning);
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

        #endregion
    }
}
