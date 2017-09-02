using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data.Oversea;
using Newegg.Oversea.Silverlight.Controls.Primitives;
using Newegg.Oversea.Silverlight.Core.Components;

namespace Newegg.Oversea.Silverlight.Controls.Data
{
    public class DataGrid : System.Windows.Controls.DataGrid
    {
        #region Const Field

        private const string DATAGRID_elementDataPager = "DataPager";
        private const string DATAGRID_elementLoading = "LoadingLayer";
        private const string DATAGRID_elementNoRecords = "NoRecordsInfo";
        private const string DATAGRID_elementROWSPRESENTER = "RowsPresenter";
        private const string DATAGRID_elementColumnsSelectorName = "ColumnsSelector";//自定义列显示区域
        private const string DATAGRID_elementTopLeftCorderBorder = "TopLeftCorderBorder";
        private const string DATAGRID_elementTopRightScollViewerBorder = "TopRightScollViewerBorder";
        private const string DATAGRID_elementVerticalScrollbar = "VerticalScrollbar";

        private const int DATAGRID_delayTimeSpan = 200;
        private const int DATAGRID_delaySaveColumnsSetting = 5000;

        #endregion Const Field

        #region Private & internal Field

        internal CustomDataGridDialog m_customizeDialog = null;
        private bool m_isColumnIndexChanged = false;
        private bool m_isLoaded = false;
        //该字段标识TotalCount属性是否在OnApplyTemplate方法访问之前被设置过；
        private bool m_isSetTotalCount = false;
        private bool m_isSortChange = true;
        private DataPager m_dataPager;
        private Border m_loadingLayer;
        private TextBlock m_noRecords;
        private DataGridRowsPresenter m_rowsPresenter;
        private Border m_topLeftCorderBorder;
        private bool m_needReloadData = true;//PageIndex变化后是否需要ReLoad数据
        private bool m_needRefreshPagerIndex = true;//当PageIndex变化后是否需要同步DataPager的PageIndex
        internal ColumnsSelectorV3 m_columnsSelector;
        private IEnumerable m_itemsSource;
        private IEnumerable m_orginalItemSource;
        private ScrollBar m_verticalScrollbar;
        private List<AdvancedTextBox> m_filterTextBox;
        private bool m_isTopCountMode;
        private Border m_borderExportAll;
        private Border m_borderExportCurrent;
        private RichButton m_buttonExport;
        private bool m_needSyncOrignal = true;
        private Dictionary<string, string> m_filterValues;
        private DispatcherTimer m_timer;
        private bool m_isBindingItemSource = false;
        private double m_ActualRowHeight = 99999;
        private int count = 0;
        private List<ColumnPinControl> m_visiableControls;
        private UserSetting UserSetting;
        private bool m_UserSettingChanged;
        private DispatcherTimer m_columnsTimer;
        private bool m_isInitColumnFilter;

        internal GridSetting m_resetGridConfig;//用于保存Grid的初始列状态
        internal IUserProfile m_userProfile;
        internal Border m_topRightScollViewerBorder;
        internal IEventTracker m_tracker;

        internal bool IsNotCustomizeColumn
        {
            get;
            set;
        }

        public bool EnableCustomizeColumn
        {
            get;
            set;
        }

        #endregion Private & internal Field

        #region Events

        public event EventHandler<LoadingDataEventArgs> LoadingDataSource;
        public event EventHandler<CancelEventArgs> PageIndexChanging;

        /// <summary>
        /// 自定义导出全部的事件
        /// </summary>
        public event EventHandler ExportAllClick;

        private EventHandler<ExportAllDataEventArgs> m_exportAllData;

        public event EventHandler<ExportAllDataEventArgs> ExportAllData
        {
            add
            {
                m_exportAllData -= value;
                m_exportAllData += value;
            }
            remove
            {
                m_exportAllData -= value;
            }
        }

        internal event EventHandler<ExportAllDataCompletedEventArgs> ExportAllDataCompleted;

        #endregion Events

        #region Private Property

        private string SortField
        {
            get;
            set;
        }
        private SortDescription SortDescription
        {
            get;
            set;
        }
        private List<PropertyGroupDescription> ColumnGroups
        {
            get;
            set;
        }

        #endregion Private Property

        #region Internal & Public Property

        internal bool NeedStoreColumns
        {
            get;
            set;
        }

        internal GridSetting CurrentGridSettings
        {
            get;
            set;
        }

        internal List<GridSetting> GridSettings
        {
            get;
            set;
        }

        internal List<GridSetting> AllGridSettings
        {
            get
            {
                List<GridSetting> settings = null;
                if (!DesignerProperties.IsInDesignTool)
                {
                    settings = m_userProfile.Get<List<GridSetting>>(GridKeys.KEY_UP_GRIDSETTINGS);
                }
                if (settings == null)
                    return new List<GridSetting>();
                return settings;
            }
        }

        public IEnumerable FilterItemsSource
        {
            get
            {
                return m_itemsSource;
            }
        }

        public new IEnumerable ItemsSource
        {
            get
            {
                if (m_orginalItemSource != null && m_orginalItemSource.GetCount() > 0)
                {
                    return m_orginalItemSource;
                }
                return m_itemsSource;
            }
            set
            {
                this.CloseLoading();
                m_itemsSource = value;
                m_isBindingItemSource = true;
                if (m_resetGridConfig != null && !DisableProfileRowHeight)
                {
                    RowHeight = m_resetGridConfig.RowHeight;
                }

                if (m_needSyncOrignal)
                {
                    m_orginalItemSource = value;
                }

                if (m_needSyncOrignal && this.m_filterValues != null && this.m_filterValues.Count > 0)
                {
                    BuildFilterResult();
                    return;
                }

                m_needSyncOrignal = true;

                if (value != null)
                {
                    PagedCollectionView source;

                    if (value is PagedCollectionView)
                    {
                        source = value as PagedCollectionView;
                    }
                    else
                    {
                        //Fix bug by Hax 20120725 服务端已经排序的数据到了客户端，加上SortDescription后默认又重新作了一次客户端排序
                        source = new PagedCollectionView(value, true, false);
                        //source = new PagedCollectionView(value);
                    }

                    if (this.m_noRecords != null)
                    {
                        this.m_noRecords.Visibility = source.TotalItemCount > 0 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
                    }

                    if (this.SortDescription != null && !string.IsNullOrEmpty(this.SortDescription.PropertyName))
                    {
                        source.SortDescriptions.Add(new SortDescription(this.SortDescription.PropertyName, this.SortDescription.Direction));
                    }

                    base.ItemsSource = source;
                    base.SelectedIndex = -1;
                }
                else
                {
                    base.ItemsSource = value;

                    if (this.m_noRecords != null)
                    {
                        this.m_noRecords.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }
        }

        public bool IsTopCountMode
        {
            get
            {
                return m_isTopCountMode;
            }
            set
            {
                m_isTopCountMode = value;
            }
        }

        public bool DisableProfileRowHeight
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool CanUserPinColumns
        {
            get;
            set;
        }

        internal double ActualRowHeight
        {
            get
            {
                return m_ActualRowHeight;
            }
        }

        #endregion Internal & Public Property

        #region Dependency Property

        private NeweggDataGridSource DataSource
        {
            get
            {
                return (NeweggDataGridSource)GetValue(DataSourceProperty);
            }
            set
            {
                SetValue(DataSourceProperty, value);
            }
        }

        private static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(NeweggDataGridSource), typeof(DataGrid), new PropertyMetadata(null, OnDataSourcePropertyChanged));

        private static void OnDataSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dg = d as DataGrid;

            if (dg != null)
            {
                NeweggDataGridSource old = e.OldValue as NeweggDataGridSource;

                if (old != null)
                {
                    old.Collection.CollectionChanged -= dg.collection_CollectionChanged;
                }
                if (dg.DataSource != null)
                {
                    if (dg.SortDescription != null && !string.IsNullOrEmpty(dg.SortDescription.PropertyName))
                    {
                        dg.DataSource.Collection.SortDescriptions.Clear();
                        var sort = new SortDescription(dg.SortDescription.PropertyName, dg.SortDescription.Direction);
                        dg.DataSource.Collection.SortDescriptions.Add(sort);
                    }

                    dg.DataSource.Collection.CollectionChanged += dg.collection_CollectionChanged;
                    dg.ItemsSource = dg.DataSource.Collection;
                    dg.SelectedIndex = -1;
                }
            }
        }

        private void collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (m_isSortChange == false)
            {
                m_noRecords.Visibility = this.DataSource.Collection.Count > 0 ? Visibility.Collapsed : Visibility.Visible;

                this.ItemsSource = this.DataSource.Collection;
                //第一次进入页面且无数据的情况下，User更改了PageSize ComboBox中的选中项，数据加载后需要还原回去
                if (this.PageSize != m_dataPager.PageSize)
                {
                    m_dataPager.PageSize = this.PageSize;
                }

                if (this.PageSize != m_columnsSelector.PageSize)
                {
                    m_columnsSelector.PageSize = this.PageSize;
                }
            }
            m_isSortChange = false;
        }

        #region PageIndex

        public bool EnableCustomHeaderMode
        {
            get
            {
                return (bool)GetValue(EnableCustomHeaderModeProperty);
            }
            set
            {
                SetValue(EnableCustomHeaderModeProperty, value);
            }
        }

        public static readonly DependencyProperty EnableCustomHeaderModeProperty =
           DependencyProperty.Register("EnableCustomHeaderMode", typeof(bool), typeof(DataGrid), new PropertyMetadata(false));

        public int PageIndex
        {
            get
            {
                return (int)GetValue(PageIndexProperty);
            }
            set
            {
                SetValue(PageIndexProperty, value);
            }
        }

        public static readonly DependencyProperty PageIndexProperty =
           DependencyProperty.Register("PageIndex", typeof(int), typeof(DataGrid), new PropertyMetadata(-1, OnPageIndexPropertyChanged));

        private static void OnPageIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dg = d as DataGrid;
            if (d != null && dg.m_dataPager != null)
            {
                dg.m_needReloadData = false;
                if (dg.m_needRefreshPagerIndex && dg.m_dataPager.Source != null)
                {
                    dg.m_dataPager.PageIndex = Convert.ToInt32(e.NewValue);
                }
            }
            dg.m_needRefreshPagerIndex = true;
        }

        #endregion PageIndex

        #region PageSize

        public int PageSize
        {
            get
            {
                return (int)GetValue(PageSizeProperty);
            }
            set
            {
                SetValue(PageSizeProperty, value);
            }
        }

        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register(
            "PageSize",
            typeof(int),
            typeof(DataGrid),
            new PropertyMetadata(25, OnPageSizePropertyChanged));

        private static void OnPageSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;

            if (dataGrid.m_columnsSelector != null)
            {
                var pageSize = int.Parse(e.NewValue.ToString());
                if (pageSize != dataGrid.m_columnsSelector.PageSize)
                {
                    dataGrid.m_columnsSelector.PageSize = pageSize;
                }
            }

            if (dataGrid.m_dataPager != null)
            {
                var pageSize = int.Parse(e.NewValue.ToString());
                if (pageSize != dataGrid.m_dataPager.PageSize)
                {
                    dataGrid.m_dataPager.PageSize = pageSize;
                }
            }
        }

        #endregion PageSize

        #region IsServerPaging

        public bool IsServerPaging
        {
            get
            {
                return (bool)GetValue(IsServerPagingProperty);
            }
            set
            {
                SetValue(IsServerPagingProperty, value);
            }
        }

        public static readonly DependencyProperty IsServerPagingProperty =
            DependencyProperty.Register("IsServerPaging", typeof(bool), typeof(DataGrid), new PropertyMetadata(false));

        //private static void OnIsServerPagingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    DataGrid dg = d as DataGrid;
        //    if (dg != null)
        //    {
        //        if ((bool)e.NewValue)
        //            dg.CanUserSortColumns = false;
        //    }
        //}

        #endregion IsServerPaging

        #region IsServerSort

        public bool IsServerSort
        {
            get
            {
                return (bool)GetValue(IsServerSortProperty);
            }
            set
            {
                SetValue(IsServerSortProperty, value);
            }
        }

        public static readonly DependencyProperty IsServerSortProperty =
            DependencyProperty.Register("IsServerSort", typeof(bool), typeof(DataGrid), new PropertyMetadata(true, OnIsServerSortPropertyChanged));

        private static void OnIsServerSortPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dg = d as DataGrid;
            if (dg != null)
            {
                if ((bool)e.NewValue)
                    dg.CanUserSortColumns = false;
            }
        }

        #endregion IsServerSort

        #region IsSaveColumns

        public bool IsSaveColumns
        {
            get
            {
                return (bool)this.GetValue(IsSaveColumnsProperty);
            }
            set
            {
                this.SetValue(IsSaveColumnsProperty, value);
            }
        }

        public static readonly DependencyProperty IsSaveColumnsProperty =
            DependencyProperty.Register("IsSaveColumns", typeof(bool), typeof(DataGrid), new PropertyMetadata(true));

        #endregion IsSaveColumns

        #region IsShowExcelExporter

        public bool IsShowExcelExporter
        {
            get
            {
                return (bool)GetValue(IsShowExcelExporterProperty);
            }
            set
            {
                SetValue(IsShowExcelExporterProperty, value);
            }
        }

        public static readonly DependencyProperty IsShowExcelExporterProperty =
            DependencyProperty.Register("IsShowExcelExporter", typeof(bool), typeof(DataGrid), new PropertyMetadata(false));

        #endregion IsShowExcelExporter

        #region IsShowAllExcelExporter

        public bool IsShowAllExcelExporter
        {
            get
            {
                return (bool)GetValue(IsShowAllExcelExporterProperty);
            }
            set
            {
                SetValue(IsShowAllExcelExporterProperty, value);
            }
        }

        public static readonly DependencyProperty IsShowAllExcelExporterProperty =
            DependencyProperty.Register("IsShowAllExcelExporter", typeof(bool), typeof(DataGrid), new PropertyMetadata(false));

        #endregion IsShowAllExcelExporter

        #region DataPagerStyle

        public Style DataPagerStyle
        {
            get
            {
                return (Style)GetValue(DataPagerStyleProperty);
            }
            set
            {
                SetValue(DataPagerStyleProperty, value);
            }
        }

        public static readonly DependencyProperty DataPagerStyleProperty =
            DependencyProperty.Register("DataPagerStyle", typeof(Style), typeof(DataGrid), null);

        #endregion DataPagerStyle

        #region LoadingDataCommand

        public ICommand LoadingDataCommand
        {
            get
            {
                return (ICommand)GetValue(LoadingDataCommandProperty);
            }
            set
            {
                SetValue(LoadingDataCommandProperty, value);
            }
        }

        public static readonly DependencyProperty LoadingDataCommandProperty =
            DependencyProperty.Register("LoadingDataCommand", typeof(ICommand), typeof(DataGrid), null);

        #endregion LoadingDataCommand

        #region LoadingDataCommandParameter

        public object LoadingDataCommandParameter
        {
            get
            {
                return (object)GetValue(LoadingDataCommandParameterProperty);
            }
            set
            {
                SetValue(LoadingDataCommandParameterProperty, value);
            }
        }

        public static readonly DependencyProperty LoadingDataCommandParameterProperty =
            DependencyProperty.Register("LoadingDataCommandParameter", typeof(object), typeof(DataGrid), null);

        #endregion LoadingDataCommandParameter

        #region ExportAllCommand

        public ICommand ExportAllDataCommand
        {
            get
            {
                return (ICommand)GetValue(ExportAllDataCommandProperty);
            }
            set
            {
                SetValue(ExportAllDataCommandProperty, value);
            }
        }

        public static readonly DependencyProperty ExportAllDataCommandProperty =
            DependencyProperty.Register("ExportAllDataCommand", typeof(ICommand), typeof(DataGrid), null);

        #endregion ExportAllCommand

        #region ExportAllDataCommandParameter

        public object ExportAllDataCommandParameter
        {
            get
            {
                return (object)GetValue(ExportAllDataCommandParameterProperty);
            }
            set
            {
                SetValue(ExportAllDataCommandParameterProperty, value);
            }
        }

        public static readonly DependencyProperty ExportAllDataCommandParameterProperty =
            DependencyProperty.Register("ExportAllDataCommandParameter", typeof(object), typeof(DataGrid), null);

        #endregion ExportAllDataCommandParameter

        #region TotalCount

        public int TotalCount
        {
            get
            {
                return (int)GetValue(TotalCountProperty);
            }
            set
            {
                SetValue(TotalCountProperty, value);
                GeneratePagerInfo();
                if (!m_isSetTotalCount)
                {
                    m_isSetTotalCount = true;
                }
            }
        }

        public static readonly DependencyProperty TotalCountProperty =
            DependencyProperty.Register("TotalCount", typeof(int), typeof(DataGrid), new PropertyMetadata(0, OnTotalCountPropertyChanged));

        private static void OnTotalCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            dataGrid.GeneratePagerInfo();
        }

        #endregion TotalCount

        #region QueryCriteria

        public object QueryCriteria
        {
            get
            {
                return (object)GetValue(QueryCriteriaProperty);
            }
            set
            {
                SetValue(QueryCriteriaProperty, value);
            }
        }

        public static readonly DependencyProperty QueryCriteriaProperty =
            DependencyProperty.Register("QueryCriteria", typeof(object), typeof(DataGrid), null);

        #endregion QueryCriteria

        #region IsShowLoading

        public bool IsShowLoading
        {
            get
            {
                return (bool)GetValue(IsShowLoadingProperty);
            }
            set
            {
                SetValue(IsShowLoadingProperty, value);
            }
        }

        public static readonly DependencyProperty IsShowLoadingProperty =
            DependencyProperty.Register("IsShowLoading", typeof(bool), typeof(DataGrid), new PropertyMetadata(false, OnIsShowLoadingPropertyChanged));

        private static void OnIsShowLoadingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            var flag = (bool)e.NewValue;
            if (flag)
            {
                dataGrid.ShowLoading();
            }
            else
            {
                dataGrid.CloseLoading();
            }
        }

        #endregion IsShowLoading

        #region IsShowPager

        public bool IsShowPager
        {
            get
            {
                return (bool)GetValue(IsShowPagerProperty);
            }
            set
            {
                SetValue(IsShowPagerProperty, value);
            }
        }

        public static readonly DependencyProperty IsShowPagerProperty =
            DependencyProperty.Register("IsShowPager", typeof(bool), typeof(DataGrid), new PropertyMetadata(true, new PropertyChangedCallback(OnIsShowPagerPropertyChanged)));

        private static void OnIsShowPagerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)d;
            var flag = (bool)e.NewValue;
            if (dataGrid.m_dataPager != null)
            {
                if (flag)
                {
                    dataGrid.m_dataPager.Visibility = Visibility.Visible;
                }
                else
                {
                    dataGrid.m_dataPager.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion IsShowPager

        #region GridID

        public string GridID
        {
            get
            {
                return (string)GetValue(GridIDProperty);
            }
            set
            {
                SetValue(GridIDProperty, value);
            }
        }

        public static readonly DependencyProperty GridIDProperty =
            DependencyProperty.Register("GridID", typeof(string), typeof(DataGrid), null);

        #endregion GridID

        #region IsShowColumnsSelector

        public bool IsShowColumnsSelector
        {
            get
            {
                return (bool)GetValue(IsShowColumnsSelectorProperty);
            }
            set
            {
                SetValue(IsShowColumnsSelectorProperty, value);
            }
        }

        public static readonly DependencyProperty IsShowColumnsSelectorProperty =
            DependencyProperty.Register(
            "IsShowColumnsSelector",
            typeof(bool),
            typeof(DataGrid),
            new PropertyMetadata(true));

        #endregion IsShowColumnsSelector

        #region PageSizeSet

        /// <summary>
        /// 获取或设置自定义每页显示数的集合
        /// </summary>
        public ObservableCollection<CustomPageSize> PageSizeSet
        {
            get
            {
                return (ObservableCollection<CustomPageSize>)GetValue(PageSizeSetProperty);
            }
            set
            {
                SetValue(PageSizeSetProperty, value);
            }
        }

        public static readonly DependencyProperty PageSizeSetProperty =
            DependencyProperty.Register("PageSizeSet", typeof(ObservableCollection<CustomPageSize>), typeof(DataGrid), new PropertyMetadata(null, OnPageSizeSetPropertyChanged));

        private static void OnPageSizeSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = d as DataGrid;

            if (dataGrid != null && dataGrid.m_columnsSelector != null)
            {
                dataGrid.m_columnsSelector.InitPageSizeSet(ref dataGrid.m_columnsSelector._cmbPageSize, dataGrid);
            }
        }

        #endregion PageSizeSet

        #endregion Dependency Property

        public DataGrid()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                DefaultStyleKey = typeof(DataGrid);

                this.ColumnGroups = new List<PropertyGroupDescription>();
                this.MouseLeftButtonDown += new MouseButtonEventHandler(PagingDataGrid_MouseLeftButtonDown);

                if (!DesignerProperties.IsInDesignTool && this.IsShowColumnsSelector)
                {
                    m_userProfile = ComponentFactory.GetComponent<IUserProfile>();
                }

                if (!DesignerProperties.IsInDesignTool && CPApplication.Current.Browser.EventTracker != null)
                {
                    this.m_tracker = CPApplication.Current.Browser.EventTracker;
                }

                this.Loaded += new RoutedEventHandler(DataGrid_Loaded);
                this.Unloaded += new RoutedEventHandler(DataGrid_Unloaded);
                this.LayoutUpdated += new EventHandler(DataGrid_LayoutUpdated);

                this.m_filterTextBox = new List<AdvancedTextBox>();
                this.m_visiableControls = new List<ColumnPinControl>();
            }
        }

        private void DataGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.m_columnsTimer != null && this.m_columnsTimer.IsEnabled)
            {
                this.m_columnsTimer.Stop();
            }
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                if (!m_isLoaded)
                {
                    if (m_columnsSelector != null)
                    {
                        this.m_resetGridConfig = m_columnsSelector.GetResetGridConfig();
                    }
                    m_isLoaded = true;

                    InitColumnCtrlAndFilter(this);
                    if (this.NeedStoreColumns && m_customizeDialog != null && !this.m_customizeDialog.IsEnableCustomizeColumn())
                    {
                        this.ColumnDisplayIndexChanged += new EventHandler<DataGridColumnEventArgs>(DataGrid_ColumnDisplayIndexChanged);

                        m_columnsTimer = new DispatcherTimer();
                        m_columnsTimer.Interval = new TimeSpan(0, 0, 0, 0, DATAGRID_delaySaveColumnsSetting);
                        m_columnsTimer.Tick += (obj, args) =>
                        {
                            try
                            {
                                System.Diagnostics.Debug.WriteLine(string.Format("UserSettingChanged's Value:{0}", this.m_UserSettingChanged));
                                if (this.m_UserSettingChanged)
                                {
                                    System.Diagnostics.Debug.WriteLine("SaveSetting method is Invoked.");

                                    this.m_UserSettingChanged = false;
                                    this.UserSetting.SaveSetting();
                                }
                            }
                            catch (Exception ex)
                            {
                                m_columnsTimer.Stop();
                                throw ex;
                            }
                        };

                        System.Diagnostics.Debug.WriteLine("Setting Timer is started.");
                    }
                    m_filterValues = new Dictionary<string, string>();
                    m_timer = new DispatcherTimer();
                    m_timer.Interval = new TimeSpan(0, 0, 0, 0, DATAGRID_delayTimeSpan);
                    m_timer.Tick += (obj, args) =>
                    {
                        try
                        {
                            BuildFilterResult();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            m_timer.Stop();
                        }
                    };
                }
                if (this.m_columnsTimer != null && !this.m_columnsTimer.IsEnabled)
                {
                    m_columnsTimer.Start();
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (!DesignerProperties.IsInDesignTool)
            {
                if (!IsNotCustomizeColumn)
                {
                    m_customizeDialog = new CustomDataGridDialog(this);
                }

                this.NeedStoreColumns = CanStoreColumns();
                if (this.NeedStoreColumns || this.EnableCustomizeColumn)
                {
                    this.AttachColumnHeader(this);
                }
                m_topRightScollViewerBorder = GetTemplateChild(DATAGRID_elementTopRightScollViewerBorder) as Border;
                m_verticalScrollbar = GetTemplateChild(DATAGRID_elementVerticalScrollbar) as ScrollBar;

                Binding bind = new Binding("Visibility")
                {
                    Source = m_verticalScrollbar
                };
                this.SetBinding(DataGridAttached.ScrollerVisibleProperty, bind);

                (GetTemplateChild("TextBlockExportCurrent") as TextBlock).Text = Resource.Pager_Export;
                (GetTemplateChild("TextBlockExportAll") as TextBlock).Text = Resource.Pager_ExportAll;

                m_buttonExport = GetTemplateChild("ButtonExport") as RichButton;
                m_borderExportAll = GetTemplateChild("borderExportAll") as Border;
                m_borderExportCurrent = GetTemplateChild("borderExportCurrent") as Border;
                m_borderExportAll = GetTemplateChild("borderExportAll") as Border;

                if (!IsShowAllExcelExporter && !IsShowExcelExporter)
                {
                    m_buttonExport.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    if (IsShowAllExcelExporter)
                    {
                        if (m_borderExportAll != null)
                        {
                            m_borderExportAll.MouseEnter -= new MouseEventHandler(border_MouseEnter);
                            m_borderExportAll.MouseLeave -= new MouseEventHandler(border_MouseLeave);
                            m_borderExportAll.MouseLeftButtonUp -= new MouseButtonEventHandler(border_MouseLeftButtonUp);
                        }

                        m_borderExportAll.MouseEnter += new MouseEventHandler(border_MouseEnter);
                        m_borderExportAll.MouseLeave += new MouseEventHandler(border_MouseLeave);
                        m_borderExportAll.MouseLeftButtonUp += new MouseButtonEventHandler(border_MouseLeftButtonUp);

                        m_buttonExport.Content = Resource.Pager_ExportAll;
                        m_buttonExport.Tag = "a";
                    }
                    else
                    {
                        m_borderExportAll.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    if (IsShowExcelExporter)
                    {
                        if (m_borderExportCurrent != null)
                        {
                            m_borderExportCurrent.MouseEnter -= new MouseEventHandler(border_MouseEnter);
                            m_borderExportCurrent.MouseLeave -= new MouseEventHandler(border_MouseLeave);
                            m_borderExportCurrent.MouseLeftButtonUp -= new MouseButtonEventHandler(border_MouseLeftButtonUp);
                        }
                        m_borderExportCurrent.MouseEnter += new MouseEventHandler(border_MouseEnter);
                        m_borderExportCurrent.MouseLeave += new MouseEventHandler(border_MouseLeave);
                        m_borderExportCurrent.MouseLeftButtonUp += new MouseButtonEventHandler(border_MouseLeftButtonUp);

                        m_buttonExport.Content = Resource.Pager_Export;
                        m_buttonExport.Tag = "c";
                    }
                    else
                    {
                        m_borderExportCurrent.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    if (m_buttonExport != null)
                    {
                        m_buttonExport.Click -= new RoutedEventHandler(m_buttonExport_Click);
                    }
                    m_buttonExport.Click += new RoutedEventHandler(m_buttonExport_Click);
                }

                m_topLeftCorderBorder = GetTemplateChild(DATAGRID_elementTopLeftCorderBorder) as Border;
                m_topLeftCorderBorder.Visibility = this.HeadersVisibility != DataGridHeadersVisibility.All ? Visibility.Visible : Visibility.Collapsed;

                m_rowsPresenter = GetTemplateChild(DATAGRID_elementROWSPRESENTER) as DataGridRowsPresenter;
                m_columnsSelector = GetTemplateChild(DATAGRID_elementColumnsSelectorName) as ColumnsSelectorV3;
                m_loadingLayer = GetTemplateChild(DATAGRID_elementLoading) as Border;
                m_noRecords = GetTemplateChild(DATAGRID_elementNoRecords) as TextBlock;
                m_noRecords.Text = Resource.NoRecords;
                m_noRecords.Visibility = Visibility.Collapsed;

                if (this.TotalCount <= 0 && m_isSetTotalCount)
                {
                    m_noRecords.Visibility = Visibility.Visible;
                }

                if (m_columnsSelector != null)
                {
                    this.m_columnsSelector.Visibility = this.IsShowColumnsSelector ? Visibility.Visible : Visibility.Collapsed;

                    if (this.GridID == null || this.GridID.Trim().Length == 0)
                    {
                        this.m_columnsSelector.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }

                m_dataPager = GetTemplateChild(DATAGRID_elementDataPager) as DataPager;

                if (m_dataPager != null)
                {
                    m_dataPager.PageIndexChanging -= new EventHandler<CancelEventArgs>(m_dataPager_PageIndexChanging);
                    m_dataPager.PageIndexChanged -= new EventHandler<EventArgs>(m_dataPager_PageIndexChanged);

                    if (this.DataPagerStyle != null)
                    {
                        this.m_dataPager.Style = this.DataPagerStyle;
                    }
                    this.m_dataPager.OwningGrid = this;
                    if (this.IsServerPaging)
                    {
                        m_dataPager.PageIndexChanging += new EventHandler<CancelEventArgs>(m_dataPager_PageIndexChanging);
                        m_dataPager.PageIndexChanged += new EventHandler<EventArgs>(m_dataPager_PageIndexChanged);
                    }
                    if (!this.IsShowPager)
                    {
                        this.m_dataPager.Visibility = Visibility.Collapsed;
                    }

                    m_dataPager.ApplyTemplate();
                }

                m_columnsSelector.OwningGrid = this;
                m_columnsSelector.DataPager = m_dataPager;

                this.m_resetGridConfig = m_columnsSelector.GetResetGridConfig();

                if (m_customizeDialog != null)
                {
                    //判断是否启用DataGridCustomizeColumn
                    if (m_customizeDialog.IsEnableCustomizeColumn())
                    {
                        List<GridSetting> localSettings = this.AllGridSettings;
                        this.GridSettings = localSettings.Where(p => p.GridGuid == this.GridID).ToList();
                        m_customizeDialog.ConvertOldProfileDataToNewProfileData();
                        if (this.GridSettings != null && this.GridSettings.Count > 0)
                        {
                            var currentConfig = this.GridSettings.FirstOrDefault(p => p.GridGuid == this.GridID && p.IsDefault);
                            m_columnsSelector.ApplyColumns(currentConfig);
                        }
                    }
                    else
                    {
                        //对本地保存的数据进行一次过滤处理。
                        if (!string.IsNullOrEmpty(this.GridID) && !DesignerProperties.IsInDesignTool)
                        {
                            List<GridSetting> localSettings = this.AllGridSettings;
                            this.GridSettings = localSettings.Where(p => p.GridGuid == this.GridID).ToList();
                            if (GridSettings != null && GridSettings.Count > 0)
                            {
                                GridSetting defaultSetting = null;
                                foreach (var setting in GridSettings)
                                {
                                    if (setting.IsDefault)
                                    {
                                        defaultSetting = setting;
                                    }
                                    setting.IsDefault = false;
                                }
                                GridSetting result = this.GridSettings.FirstOrDefault(p => p.Name == this.GridID);
                                if (result == null)
                                {
                                    defaultSetting.Name = this.GridID;
                                    result = defaultSetting;
                                }
                                if (result != null)
                                {
                                    result.IsDefault = true;
                                }

                                foreach (var setting in GridSettings)
                                {
                                    if (!setting.IsDefault)
                                    {
                                        localSettings.Remove(setting);
                                    }
                                }
                                this.GridSettings = localSettings.Where(p => (p.GridGuid == this.GridID && p.Name == this.GridID)).ToList();
                                this.m_userProfile.Set(GridKeys.KEY_UP_GRIDSETTINGS, localSettings, true);
                            }
                        }

                        if (this.NeedStoreColumns && !DesignerProperties.IsInDesignTool)
                        {
                            this.UserSetting = new Primitives.UserSetting(this);
                            this.UserSetting.LoadSetting();
                        }
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    Bind();
                }
                GeneratePagerInfo();

                //用于其他Team自定义Header的时候启用的样式；
                DataGridColumnHeader TopLeftCornerHeader = GetTemplateChild("TopLeftCornerHeader") as DataGridColumnHeader;
                Border TopLeftCorderBorder = GetTemplateChild("TopLeftCorderBorder") as Border;
                DataGridColumnHeader TopRightCornerHeader = GetTemplateChild("TopRightCornerHeader") as DataGridColumnHeader;
                Grid TopRightCornerGrid = GetTemplateChild("TopRightCornerGrid") as Grid;
                if (EnableCustomHeaderMode
                    && TopLeftCornerHeader != null
                    && TopLeftCorderBorder != null
                    && TopRightCornerHeader != null
                    && TopRightCornerGrid != null)
                {
                    TopLeftCornerHeader.Margin = new Thickness(0, 0, -1, 1);
                    TopLeftCorderBorder.Margin = new Thickness(0, 0, 0, 1);
                    TopLeftCorderBorder.Height = Double.NaN;
                    TopLeftCorderBorder.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    TopRightCornerHeader.Opacity = 1;
                    TopRightCornerHeader.Margin = new Thickness(0, 0, 0, 1);
                    TopRightCornerGrid.RowDefinitions.Clear();
                    TopRightCornerGrid.Margin = new Thickness(0, 0, 0, 1);
                }
            }
        }

        #region Event Impl

        private void DataGrid_ColumnDisplayIndexChanged(object sender, DataGridColumnEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DataGrid_ColumnDisplayIndexChanged event is Invoked.");
            this.m_isColumnIndexChanged = true;
            this.m_UserSettingChanged = true;
        }

        //获取Cell的真实高度
        private void DataGrid_LayoutUpdated(object sender, EventArgs e)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                if (!m_isInitColumnFilter && m_isLoaded)
                {
                    InitColumnCtrlAndFilter(this);
                }

                if (m_isBindingItemSource)
                {
                    Border rowHeightBorder = FindVisualChild(this);
                    if (rowHeightBorder != null)
                    {
                        m_isBindingItemSource = false;
                        m_ActualRowHeight = rowHeightBorder.ActualHeight;

                        if (IsShowColumnsSelector && GridID != null && GridID.Length != 0)
                        {
                            if (!this.m_isColumnIndexChanged)
                            {
                                this.m_columnsSelector.RefreshGridConfig(true);
                            }
                            else
                            {
                                this.m_isColumnIndexChanged = false;
                            }
                        }
                    }
                }
            }
        }

        private void filterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as AdvancedTextBox;

            #region 删除过滤文本框为空的过滤条件

            if (textBox.Text == null || textBox.Text.Trim().Length == 0)
            {
                if (textBox.Tag == null)
                {
                    return;
                }
                string key = textBox.Tag.ToString();
                if (m_filterValues.ContainsKey(key))
                {
                    m_filterValues.Remove(key);
                }
            }

            #endregion 删除过滤文本框为空的过滤条件

            if (textBox != null && textBox.Tag != null)
            {
                if (textBox.Text != null && textBox.Text.Trim().Length > 0)
                {
                    m_filterValues[textBox.Tag.ToString()] = textBox.Text;
                }
                m_timer.Start();
            }
        }

        private void m_buttonExport_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as RichButton;
            if (button.Tag.ToString() == "c")
            {
                m_tracker.TraceEvent(string.Format("Click:{0}", "Export"), string.Format("Grid:{0}", GetLabel()));
                Exporter.ExportDataGrid(this);
            }
            else
            {
                if (this.ExportAllClick != null)
                {
                    m_tracker.TraceEvent(string.Format("Click:{0}", "Export All"), string.Format("Grid:{0}", GetLabel()));
                    this.ExportAllClick(this, null);
                }
                else
                {
                    if (m_exportAllData != null)
                    {
                        m_tracker.TraceEvent(string.Format("Click:{0}", "Export All"), string.Format("Grid:{0}", GetLabel()));
                        m_dataPager._hlExportAll_Click(null, null);
                    }
                }
            }
        }

        public string GetLabel()
        {
            if (!string.IsNullOrEmpty(this.Name))
            {
                return this.Name;
            }
            else if (!string.IsNullOrEmpty(this.GridID))
            {
                return this.GridID;
            }
            return "N/A";
        }

        private void border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var textBox = border.Child as TextBlock;

            m_buttonExport.Content = textBox.Text;
            m_buttonExport.Tag = textBox.Tag;
            border.Background = new SolidColorBrush(Colors.White);
            m_buttonExport.ClosePopup();

            m_tracker.TraceEvent(string.Format("Click:{0}", "Export"), string.Format("Grid:{0}", GetLabel()));
            m_buttonExport_Click(this.m_buttonExport, null);
        }

        private void border_MouseLeave(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            border.Background = new SolidColorBrush(Colors.White);
        }

        private void border_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            var bytes = BitConverter.GetBytes(0xFFE5E5E5);
            border.Background = new SolidColorBrush(Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]));
        }

        #endregion Event Impl

        #region Public Methods

        public void Bind()
        {
            //add by ryan, 解决在loaded调用bind，没有对的datagrid相应控件进行初始化。
            if (this.m_dataPager == null)
            {
                count++;
                return;
            }
            this.SortDescription = new SortDescription();
            if (this.DataSource != null && this.DataSource.Collection != null)
            {
                m_isSortChange = true;
                this.DataSource.Collection.SortDescriptions.Clear();
                m_isSortChange = false;
            }
            //this.SortField = string.Empty;
            this.PageIndex = 0;
            this.LoadingDataInternal(BindActionType.Other);
        }

        public void ShowLoading()
        {
            if (this.m_loadingLayer != null && this.IsShowLoading)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    Canvas.SetZIndex(this.m_loadingLayer, 1);
                    var loading = this.m_loadingLayer.Child as LoadingControl;
                    if (loading != null)
                    {
                        loading.Start();
                    }
                    m_rowsPresenter.Opacity = 0.5;
                });
            }
        }

        public void CloseLoading()
        {
            if (this.m_loadingLayer != null)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    Canvas.SetZIndex(this.m_loadingLayer, -1);
                    var loading = this.m_loadingLayer.Child as LoadingControl;
                    if (loading != null)
                    {
                        loading.Stop();
                    }
                    m_rowsPresenter.Opacity = 1;
                });
            }
        }

        public void OnExportAllDataCompleted(ExportAllDataCompletedEventArgs e)
        {
            this.ExportAllDataCompleted(this, e);
        }

        #endregion Public Methods

        #region Internal Methods

        internal void OnExportAllData()
        {
            List<Dictionary<string, string>> visibleColumns = new List<Dictionary<string, string>>();
            foreach (var col in this.Columns)
            {
                if (col.Visibility == Visibility.Visible && col is DataGridBoundColumn)
                {
                    var boundCol = col as DataGridBoundColumn;
                    if (boundCol.Binding != null)
                    {
                        var dic = new Dictionary<string, string>();
                        dic.Add(col.Header.ToString(), boundCol.Binding.Path.Path);
                        visibleColumns.Add(dic);
                    }
                }
            }
            if (this.m_exportAllData != null)
            {
                this.m_exportAllData(this, new ExportAllDataEventArgs(visibleColumns));
            }

            var parameter = this.ExportAllDataCommandParameter;
            if (parameter == null)
            {
                parameter = new ExportAllDataEventArgs(visibleColumns);
            }

            ExecuteCommand(this.ExportAllDataCommand, parameter);
        }

        internal void RefreshGridSetting()
        {
            this.m_columnsSelector.Refresh();
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// 系统是否需要保持用户对列的修改
        /// </summary>
        /// <returns></returns>
        private bool CanStoreColumns()
        {
            if (string.IsNullOrEmpty(this.GridID))
            {
                return false;
            }

            else if (!this.IsSaveColumns)
            {
                return false;
            }
            else if (!this.CanUserResizeColumns)
            {
                return false;
            }
            else
            {
                var b = true;

                foreach (var column in this.Columns)
                {
                    var s = column.GetHashCode();

                    if (column is DataGridTemplateColumn)
                    {
                        var col = (DataGridTemplateColumn)column;
                        if (string.IsNullOrEmpty(col.Name))
                        {
                            b = false;
                            break;
                        }
                    }

                    if (column is DataGridTextColumn)
                    {
                        var col = (DataGridTextColumn)column;
                        if (string.IsNullOrEmpty(col.Name) && string.IsNullOrEmpty(col.GetBindingPath()))
                        {
                            b = false;
                            break;
                        }
                    }

                    if (!(column is DataGridTextColumn) && !(column is DataGridTemplateColumn))
                    {
                        b = false;
                        break;
                    }
                }

                return b;
            }
        }

        private void AttachColumnHeader(DependencyObject obj)
        {
            var count = VisualTreeHelper.GetChildrenCount(obj);

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                DataGridColumnHeader header = child as DataGridColumnHeader;
                if (header != null)
                {
                    header.MouseLeave += new MouseEventHandler(header_MouseLeave);
                }
                else
                {
                    AttachColumnHeader(child);
                }
            }
        }

        private void header_MouseLeave(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DataGrid_Cheader_MouseLeaveolumnDisplayIndexChanged event is Invoked.");

            var element = sender as FrameworkElement;
            if (element.Tag == null || (double)element.Tag != element.ActualWidth)
            {
                System.Diagnostics.Debug.WriteLine("Column width has been changed.");

                this.m_UserSettingChanged = true;
                element.Tag = element.ActualWidth;
            }
        }

        /// <summary>
        /// Generate pager info at the bottom of DataGrid
        /// </summary>
        private void GeneratePagerInfo()
        {
            if (m_dataPager != null)
            {
                List<int> list = new List<int>(this.TotalCount);
                for (int i = 0; i < this.TotalCount; i++)
                {
                    list.Add(i);
                }
                PagedCollectionView pcv = new PagedCollectionView(list);
                pcv.PageSize = this.PageSize;

                m_needReloadData = false;
                //重新设置了dataPager的Source后，PageIndex会变为0，触发PageIndexChanged事件,此时不应Reload Data
                m_dataPager.Source = pcv;

                //针对这种特殊需求：User要控制第一次加载数据的PageIndex
                if (m_dataPager.PageIndex != this.PageIndex && this.PageIndex >= 0)
                {
                    this.m_needReloadData = false;//Don't reload data
                    m_dataPager.PageIndex = this.PageIndex;
                }

                m_needReloadData = true;
                m_dataPager.TotalCount = this.TotalCount;
            }
        }

        private void m_dataPager_PageIndexChanging(object sender, CancelEventArgs e)
        {
            if (this.PageIndexChanging != null)
            {
                PageIndexChanging(this, e);
            }
        }

        private void m_dataPager_PageIndexChanged(object sender, EventArgs e)
        {
            var dataPager = sender as DataPager;
            if (dataPager.PageIndex != this.PageIndex && m_needReloadData)
            {
                m_needRefreshPagerIndex = false;
                this.PageIndex = dataPager.PageIndex;
                LoadingDataInternal(BindActionType.Paging);
            }
            this.m_needReloadData = true;
        }

        private void LoadingDataInternal(BindActionType type)
        {
            this.ShowLoading();

            ResetFilter();

            this.PageSize = m_columnsSelector.PageSize > 0 ? m_columnsSelector.PageSize : this.PageSize;

            //第一次进入页面且无数据的情况下，User更改了PageSize ComboBox中的选中项，数据加载后需要还原回去
            if (m_dataPager.PageSize == 0)
            {
                m_dataPager.PageSize = this.PageSize;
            }

            if (m_columnsSelector.PageSize == 0)
            {
                m_columnsSelector.PageSize = this.PageSize;
            }

            this.PageIndex = m_dataPager.PageIndex < 0 ? 0 : this.m_dataPager.PageIndex;

            if (this.m_noRecords != null)
            {
                this.m_noRecords.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (LoadingDataSource != null)
            {
                LoadingDataSource(this, new LoadingDataEventArgs(this.PageIndex, this.PageSize, this.SortField, this.QueryCriteria, type));
            }
            var parameter = this.LoadingDataCommandParameter;
            if (parameter == null)
            {
                parameter = new LoadingDataEventArgs(this.PageIndex, this.PageSize, this.SortField, this.QueryCriteria, type);
            }

            ExecuteCommand(this.LoadingDataCommand, parameter);
        }

        public void ResetFilter()
        {
            if (m_filterTextBox != null && m_filterTextBox.Count > 0)
            {
                m_filterTextBox.ForEach(item =>
                {
                    item.Text = string.Empty;
                });
            }
            if (this.m_filterValues != null)
            {
                this.m_filterValues.Clear();
            }
        }

        private void ExecuteCommand(ICommand command, object commandParameter)
        {
            if ((command != null) && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }

        private void PagingDataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var u = from element in VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), this)
                    where element is DataGridColumnHeader
                    select element as DataGridColumnHeader;

            DataGridColumnHeader dataGridColumnHeader = u.FirstOrDefault();
            DataGridColumn column = null;
            if (dataGridColumnHeader != null)
            {
                column = GetDataGridColumn(dataGridColumnHeader);
            }
            if (column != null && this.TotalCount > 0 && this.IsServerSort)
            {
                ProcessSort(column);

                e.Handled = true;
            }
        }

        private void ProcessSort(DataGridColumn column)
        {
            //Update By Aaron: 以下代码支持DataTemplateColumn如果未设置SortMemberPath，则默认使用SortField设置；
            if (column is DataGridTemplateColumn
                && string.IsNullOrWhiteSpace(column.SortMemberPath)
                && !string.IsNullOrWhiteSpace((column as DataGridTemplateColumn).SortField))
            {
                column.SortMemberPath = (column as DataGridTemplateColumn).SortField;
            }
            //Update End

            if (column.CanUserSort)
            {
                string sortField = string.Empty;
                string sortMemberPath = string.Empty;
                if (!string.IsNullOrWhiteSpace(column.SortMemberPath))
                {
                    sortMemberPath = column.SortMemberPath;
                }
                else
                {
                    DataGridBoundColumn col = column as DataGridBoundColumn;

                    if (col != null && col.Binding != null && col.Binding.Path != null)
                    {
                        sortMemberPath = col.Binding.Path.Path;
                    }
                }

                if (column is DataGridTextColumn)
                {
                    sortField = (column as DataGridTextColumn).SortField;
                }
                else if (column is DataGridTemplateColumn)
                {
                    sortField = (column as DataGridTemplateColumn).SortField;
                }

                //Update By Poseidon.y.tong:没有设置Binding.Path的时候取sortField
                if (string.IsNullOrWhiteSpace(sortMemberPath))
                {
                    sortMemberPath = sortField;
                }
                //Update End

                //SortDescription sortDesc = this.DataSource.Collection.SortDescriptions.SingleOrDefault(s => string.Equals(s.PropertyName, sortMemberPath));
                var sortDesc = this.SortDescription;

                if (string.IsNullOrEmpty(sortDesc.PropertyName) || sortDesc.PropertyName.ToLower() != sortMemberPath.ToLower())
                {
                    sortDesc = new SortDescription(sortMemberPath, ListSortDirection.Ascending);
                }
                else if (sortDesc.IsSealed == false)
                {
                    sortDesc.Direction = sortDesc.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                }
                else if (sortDesc.IsSealed)
                {
                    ListSortDirection direction = sortDesc.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    sortDesc = new SortDescription(sortMemberPath, direction);
                }

                this.SortField = string.Format("{0} {1}", sortField, sortDesc.Direction == ListSortDirection.Ascending ? "asc" : "desc");
                this.SortDescription = sortDesc;

                string state = sortDesc.Direction == ListSortDirection.Ascending ? "SortAscending" : "SortDescending";

                ////Note:在MVVM中此处才需要处理排序的图标，如果是基于事件的方式，则在此处不需要处理排序图标
                //m_isSortChange = true;
                //if (this.LoadingDataCommand != null)
                //{
                //    PagedCollectionView baseSource = base.ItemsSource as PagedCollectionView;
                //    if (baseSource != null)
                //    {
                //        baseSource.SortDescriptions.Clear();
                //        baseSource.SortDescriptions.Add(sortDesc);
                //    }
                //}
                //m_isSortChange = false;

                this.PageIndex = 0;
                LoadingDataInternal(BindActionType.Sort);
            }
        }

        private DataGridColumn GetDataGridColumn(DataGridColumnHeader dataGridColumnHeader)
        {
            DataGridColumn column = null;
            var content = dataGridColumnHeader.Content;
            var contentTemplate = dataGridColumnHeader.ContentTemplate;
            if (content != null)
            {
                column = this.Columns.SingleOrDefault(c =>
                {
                    if (c.Header != null)
                    {
                        return c.Header.ToString().Trim().ToLower() == content.ToString().Trim().ToLower();
                    }
                    return false;
                });
            }
            else if (contentTemplate != null)
            {
                column = this.Columns.SingleOrDefault(c =>
                {
                    Style style = c.HeaderStyle;
                    if (style != null)
                    {
                        Setter template = style.Setters.OfType<Setter>()
                                            .Where(setter => setter.Property == ContentControl.ContentTemplateProperty)
                                            .FirstOrDefault();
                        if (template != null)
                        {
                            return object.ReferenceEquals(template.Value, contentTemplate);
                        }
                    }
                    return false;
                });
            }
            else
            {
                return null;
            }

            return column;
        }

        //获取Cell中的RowHeight Border，以供来过去真实的高度；
        private Border FindVisualChild(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is Border && ((Border)child).Name == "RowHeightMeasurer")
                {
                    return child as Border;
                }
                else
                {
                    Border value = FindVisualChild(child);
                    if (value != null)
                    {
                        return value;
                    }
                }
            }
            return null;
        }

        private void InitColumnCtrlAndFilter(DependencyObject obj)
        {
            var count = VisualTreeHelper.GetChildrenCount(obj);

            if (count > 0)
            {
                this.m_isInitColumnFilter = true;

                for (int i = 0; i < count; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                    if (child != null && child is ColumnPinControl)
                    {
                        var control = child as ColumnPinControl;
                        if (control.Tag != null && this.CanUserPinColumns)
                        {
                            var column = this.Columns.Where(c => c.Header != null && c.Header.ToString() == control.Tag.ToString()).FirstOrDefault();

                            control.Column = column;
                            control.OwningGrid = this;
                            control.Visibility = System.Windows.Visibility.Visible;

                            this.m_visiableControls.Add(control);
                        }
                    }

                    if (child != null && child is AdvancedTextBox && ((AdvancedTextBox)child).Name == "DATAGRID_FILTER_TEXT")
                    {
                        var filterText = child as AdvancedTextBox;
                        m_filterTextBox.Add(filterText);

                        filterText.TextChanged -= new TextChangedEventHandler(filterText_TextChanged);
                        filterText.TextChanged += new TextChangedEventHandler(filterText_TextChanged);

                        var b = false;

                        //判断是否有设置过滤的功能
                        foreach (var c in this.Columns)
                        {
                            if (c is DataGridTextColumn && (c as DataGridTextColumn).FilterField != null)
                            {
                                b = true;
                                break;
                            }

                            if (c is DataGridTemplateColumn && (c as DataGridTemplateColumn).FilterField != null)
                            {
                                b = true;
                                break;
                            }
                        }
                        if (b)
                        {
                            filterText.Visibility = System.Windows.Visibility.Visible;
                            filterText.Opacity = 0;
                        }

                        foreach (var col in this.Columns)
                        {
                            if (filterText.Tag != null && col.Header != null)
                            {
                                if (col.Header.ToString() == filterText.Tag.ToString())
                                {
                                    if (col is DataGridTextColumn)
                                    {
                                        filterText.Tag = (col as DataGridTextColumn).FilterField;
                                    }
                                    else if (col is DataGridTemplateColumn)
                                    {
                                        filterText.Tag = (col as DataGridTemplateColumn).FilterField;
                                    }

                                    if (filterText.Tag != null)
                                    {
                                        filterText.Opacity = 1;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        InitColumnCtrlAndFilter(child);
                    }
                }
            }
        }

        private void BuildFilterResult()
        {
            if (this.ItemsSource == null)
            {
                return;
            }

            Type type;
            bool b;

            if (b = (this.ItemsSource is PagedCollectionView))
            {
                type = ((PagedCollectionView)this.ItemsSource).SourceCollection.GetType();
            }
            else
            {
                type = this.ItemsSource.GetType();
            }

            var list = (IList)Activator.CreateInstance(type);

            if (this.m_filterValues.Count > 0 && this.ItemsSource != null && this.m_orginalItemSource != null)
            {
                foreach (var item in this.m_orginalItemSource)
                {
                    var isPassed = true;
                    foreach (var keyValue in m_filterValues)
                    {
                        if (keyValue.Value != null && keyValue.Value.Trim().Length > 0)
                        {
                            var value = item.GetType().GetProperty(keyValue.Key).GetValue(item, null);

                            if (value == null)
                            {
                                isPassed = false;
                                break;
                            }

                            if (!value.ToString().Trim().ToUpper().Contains(keyValue.Value.Trim().ToUpper()))
                            {
                                isPassed = false;
                                break;
                            }
                        }
                    }
                    if (isPassed)
                    {
                        list.Add(item);
                    }
                }
            }

            if (m_filterValues.Count == 0)
            {
                this.ItemsSource = m_orginalItemSource;
            }
            else
            {
                m_needSyncOrignal = false;
                if (b)
                {
                    var newCollection = new PagedCollectionView(list);
                    var oldCollection = this.ItemsSource as PagedCollectionView;
                    if (oldCollection != null)
                    {
                        foreach (var item in oldCollection.SortDescriptions)
                        {
                            newCollection.SortDescriptions.Add(item);
                        }
                        foreach (var item in oldCollection.GroupDescriptions)
                        {
                            newCollection.GroupDescriptions.Add(item);
                        }
                    }
                    this.ItemsSource = newCollection;
                }
                else
                {
                    this.ItemsSource = list;
                }
            }
        }

        #endregion Private Methods

        //Add by ryan,解决拖拽header的dead cycle问题；
        //int cycleCount = 0;
        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    if (cycleCount < 5 || availableSize.Height == double.PositiveInfinity || availableSize.Width == double.PositiveInfinity)
        //    {
        //        cycleCount++;
        //        Size size = base.MeasureOverride(availableSize);
        //        return size;
        //    }
        //    return availableSize;
        //}

        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    cycleCount = 0;
        //    Size size = base.ArrangeOverride(finalSize);

        //    return size;
        //}
    }
}