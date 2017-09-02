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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.Basic.Components.UserControls.BrandPicker
{
    public partial class UCBrandQuery : UserControl
    {
        public IDialog Dialog { get; set; }
        public BrandQueryFacade serviceFacade;
        public BrandQueryFilter queryFilter;
        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(UCBrandQuery),
            new PropertyMetadata(SelectionMode.Single, new PropertyChangedCallback(UCBrandQuery.OnSelectionModePropertyChanged)));

        private static void OnSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as UCBrandQuery;
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
                    this.QueryResultGrid.Columns[0].Visibility = Visibility.Visible;
                    break;
                default:
                    this.QueryResultGrid.Columns[0].Visibility = Visibility.Collapsed;
                    break;
            }
        }

        public UCBrandQuery()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCBrandQuery_Loaded);

            serviceFacade = new BrandQueryFacade(CurrentPage);
            queryFilter = new BrandQueryFilter();
        }

        void UCBrandQuery_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComboBoxData();
        }

        private void LoadComboBoxData()
        {
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ValidStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryFilter.PagingInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };
            queryFilter.PagingInfo.SortBy = e.SortField;
            serviceFacade.QueryBrandInfo(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var vendorList = args.Result.Rows.ToList("IsChecked", false);
                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;
                QueryResultGrid.ItemsSource = vendorList;
            });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.QueryResultGrid.SelectedIndex = -1;
            this.queryFilter.BrandNameLocal = this.txtBrandName.Text.Trim();
            KeyValuePair<ValidStatus?, string> selectItem = (KeyValuePair<ValidStatus?, string>)cmbStatus.SelectedItem;

            this.queryFilter.Status = selectItem.Key;

            this.queryFilter.ManufacturerName = this.txtManufacturerName.Text.Trim();

            QueryResultGrid.Bind();
        }

        private void btnChooseBrand_Click(object sender, RoutedEventArgs e)
        {
            if (SelectionMode == SelectionMode.Single)
            {
                if (this.QueryResultGrid.SelectedIndex < 0 || this.QueryResultGrid.ItemsSource == null)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("错误", "请先选择一个代理品牌!", MessageType.Error, (obj, args) => { });
                    return;
                }
                DynamicXml selectedManufacturer = this.QueryResultGrid.SelectedItem as DynamicXml;
                if (null != selectedManufacturer)
                {
                    this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    this.Dialog.ResultArgs.Data = selectedManufacturer;
                    this.Dialog.Close(true);
                }
            }
            else
            {
                List<DynamicXml> list = new List<DynamicXml>();
                dynamic rows = this.QueryResultGrid.ItemsSource;
                if (rows != null)
                {
                    foreach (dynamic row in rows)
                    {
                        if (row.IsChecked)
                        {
                            list.Add(row);
                        }
                    }
                }

                if (list.Count <= 0)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("错误", "请先选择一个代理品牌!", MessageType.Error, (obj, args) => { });
                    return;
                }

                this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                this.Dialog.ResultArgs.Data = list;
                this.Dialog.Close(true);
            }
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            dynamic rows = this.QueryResultGrid.ItemsSource;
            if (rows != null)
            {
                foreach (dynamic row in rows)
                {
                    row.IsChecked = chk.IsChecked.Value;
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            this.Dialog.ResultArgs.Data = null;
            this.Dialog.Close(true);
        }
    }
}
