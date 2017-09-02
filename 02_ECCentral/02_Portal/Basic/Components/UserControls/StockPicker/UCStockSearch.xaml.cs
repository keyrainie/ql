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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.Inventory;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.UserControls.StockPicker;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic.Components;
using ECCentral.Portal.Basic.Components.Facades;

namespace ECCentral.Portal.Basic.Components.UserControls.StockPicker
{
    public partial class UCStockSearch : UserControl
    {
        #region Properties

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(UCStockSearch),
            new PropertyMetadata(SelectionMode.Single, new PropertyChangedCallback(UCStockSearch.OnSelectionModePropertyChanged)));

        private static void OnSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as UCStockSearch;
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
                    break;
                default:
                    this.DataGrid.Columns[0].Visibility = Visibility.Visible;
                    this.DataGrid.Columns[1].Visibility = Visibility.Collapsed;
                    break;
            }
        }


        /// <summary>
        /// 应用Window.ShowDialog弹出时，会返回一个DialogHandle,根据这个DialogHandle可以做关闭对话框等操作
        /// </summary>
        public IDialog DialogHandle { get; set; }

        /// <summary>
        /// 查询条件,与UI建立双向绑定
        /// </summary>
        private StockQueryVM _queryReq;

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

        private List<StockVM> _vmList;

        private CheckBox _checkBoxAll;

        #endregion

        public UCStockSearch()
        {
            InitializeComponent();
            this.Grid.DataContext = _queryReq = new StockQueryVM();
            //this.Loaded += new RoutedEventHandler(UCStockSearch_Loaded);
        }

        void UCStockSearch_Loaded(object sender, RoutedEventArgs e)
        {
            CommonDataFacade _facadeCommon = new CommonDataFacade(CPApplication.Current.CurrentPage);
            _facadeCommon.GetWebChannelList(true, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;
                this.cmbWebChannelList.ItemsSource = args.Result;
                this.cmbWebChannelList.SelectedIndex = 0;
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
                    this.DataGrid.Bind();
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
            StockQueryFacade facade = new StockQueryFacade(CPApplication.Current.CurrentPage);
            facade.QueryStock(_queryReq, p, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;
                BindDataGrid(args.Result.TotalCount, args.Result.Rows);
            });
        }

        public void BindDataGrid(int totalCount, dynamic rows)
        {
            this.DataGrid.TotalCount = totalCount;
            _vmList = DynamicConverter<StockVM>.ConvertToVMList<List<StockVM>>(rows);
            this.DataGrid.ItemsSource = _vmList;
            if (_checkBoxAll != null)
            {
                //重新查询后，将全选CheckBox置为false
                _checkBoxAll.IsChecked = false;
            }
        }

        /// <summary>
        /// 查询按钮事件，直接调用DataGrid的Bind方法，DataGrid内部会触发LoadingDataSource事件
        /// </summary>
        void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //this.DataGrid.Bind();
        }

        void ButtonConfirmSelected_Click(object sender, RoutedEventArgs e)
        {
            if (this.DialogHandle == null)
                return;
            if (_vmList == null)
            {
                this.CurrentWindow.Alert(ResStockPicker.Msg_PleaseSelect, MessageType.Warning);
                return;
            }
            //获取用户选择的记录列表
            List<StockVM> selectedList = new List<StockVM>();
            foreach (var item in _vmList)
            {
                if (item.IsChecked)
                {
                    selectedList.Add(item);
                }
            }
            if (selectedList.Count == 0)
            {
                this.CurrentWindow.Alert(ResStockPicker.Msg_PleaseSelect, MessageType.Warning);
                return;
            }
            //关闭对话框并返回数据
            if (this.SelectionMode == SelectionMode.Single)
            {
                this.DialogHandle.ResultArgs.Data = selectedList[0];
            }
            else if (this.SelectionMode == SelectionMode.Multiple)
            {
                this.DialogHandle.ResultArgs.Data = selectedList;
            }
            this.DialogHandle.ResultArgs.DialogResult = DialogResultType.OK;
            this.DialogHandle.Close();
        }

        void ButtonCloseDialog_Click(object sender, RoutedEventArgs e)
        {
            if (this.DialogHandle == null)
                return;

            this.DialogHandle.ResultArgs.DialogResult = DialogResultType.Cancel;
            this.DialogHandle.Close();
        }

        private void Hyperlink_StockNumber_Click(object sender, RoutedEventArgs e)
        {
            var lnk = sender as FrameworkElement;
            if (null != lnk)
            {
                CurrentWindow.Navigate(string.Format(ConstValue.Inventory_StockMaintainUrlFormat, lnk.Tag.ToString()), null, true);
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
    }
}
