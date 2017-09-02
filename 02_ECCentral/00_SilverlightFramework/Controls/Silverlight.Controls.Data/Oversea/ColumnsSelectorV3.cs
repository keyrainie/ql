using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using Newegg.Oversea.Silverlight.Controls.Data;
using System.Collections;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data.Oversea;
using System.Windows.Shapes;

namespace Newegg.Oversea.Silverlight.Controls.Primitives
{
    public class ColumnsSelectorV3 : ContentControl
    {
        IDialog dialog = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.Browser.GetComponent<IDialog>();

        #region Data

        private FrameworkElement _elementRoot;
        private bool _isOpen;
        private FrameworkElement _elementContentPresenterBorder;
        private Canvas _elementOutsidePopup;
        private Canvas _elementPopupChildCanvas;
        private FrameworkElement _elementPopupChild;
        private ToggleButton _expanderButton;
        private Popup _elementPopup;
        private Button _customizeButton;
        private Button _deleteButton;
        internal NumericUpDown _rowHeightInput;
        private ComboBox _gridSettingsComboBox;
        private GridSetting m_resetGridConfig;
        private ObservableCollection<ComboBoxItem> m_settingsCollection = new ObservableCollection<ComboBoxItem>();
        private ComboBox _backgroundColorPicker, _bandedColorPicker;
        private TextBlock m_tbResult;
        private TextBox _txtSettingName;
        private Button _excelButton;

        internal ComboBox _cmbPageSize;
        private Button _resetButton;

        private bool _isReset;


        #endregion

        #region Constants

        private const string RowHeightInput = "RowHeightInput";
        private const string ElementCustomizeButtonName = "ButtonCustomize";
        private const string ColumnName_Prefix = "DataGrid_ListBoxItem_";
        private const string RootElementName = "RootElement";
        private const string ExpanderButtonName = "ExpanderButton";
        private const string ElementPopupName = "ElementPopup";
        private const string ContentPresenterBorder = "ContentPresenterBorder";
        private const string SwitchGridConfigName = "SwitchGridConfig";
        private const string ApplyColumnConfig = "ApplyColumnConfig";
        private const string TabItemApply = "TabItemApply";
        private const string TabItemCustomize = "TabItemCustomize";
        private const string RESET_NAME = "[Reset]";
        private const string Default_Prefix = "[D]";
        private const string ElementGridSettingsComboBoxName = "GridSettingsList";
        private const string ElementBackgroundColorPickerName = "BackgroundColorPicker";
        private const string ElementBandedColorPickerName = "BandedColorPicker";
        private const string Element_tbResult = "tbResult";
        private const string Element_GridSettingName = "GridSettingName";
        private const string Element_DeleteButton = "DeleteButton";
        private const string Element_ExcelButton = "ExcelButton";

        private const string Element_PageSize = "cmbPageSize";
        private const int DEFAULT_PAGESIZE = 25;
        private const string Element_ButtonReset = "ButtonReset";

        public static readonly DependencyProperty MaxDropDownHeightProperty;

        #endregion

        public new int PageSize
        {
            get
            {
                return DataPager.PageSize;
            }

            set
            {
                DataPager.PageSize = value;

                //Sync PageSize ComboBox's SelectedItem
                if (_cmbPageSize != null)
                {
                    foreach (var item in _cmbPageSize.Items)
                    {
                        var ele = item as ComboBoxItem;
                        if (int.Parse(ele.Tag.ToString()) == value)
                        {
                            _cmbPageSize.SelectedItem = ele;
                            break;
                        }
                    }
                }
            }
        }

        public bool IsOpen
        {
            get
            {
                return this._isOpen;
            }
            set
            {
                this._isOpen = value;
                if (this._elementPopup != null)
                {
                    this._elementPopup.IsOpen = value;
                }
            }
        }

        public double MaxDropDownHeight
        {
            get
            {
                return (double)base.GetValue(MaxDropDownHeightProperty);
            }
            set
            {
                base.SetValue(MaxDropDownHeightProperty, value);
            }
        }

        internal Data.DataGrid OwningGrid { get; set; }

        internal Data.DataPager DataPager { get; set; }

        static ColumnsSelectorV3()
        {
            MaxDropDownHeightProperty = DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(ColumnsSelectorV3), new PropertyMetadata((double)1.0 / (double)0.0, new PropertyChangedCallback(ColumnsSelectorV3.OnMaxDropDownHeightChanged)));
        }

        public ColumnsSelectorV3()
        {
            DefaultStyleKey = typeof(ColumnsSelectorV3);
        }

        public void Refresh()
        {
            //this._switchGridConfig.Refresh();
            //this._applyColumnConfig.Refresh();
        }

        #region Protected Methods

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size size = base.ArrangeOverride(arrangeBounds);
            this.ArrangePopup();
            return size;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            (GetTemplateChild("RowHeight") as TextBlock).Text = Resource.RowHeight;
            (GetTemplateChild("RowBackground") as TextBlock).Text = Resource.RowBackground;
            (GetTemplateChild("BandedRowColor") as TextBlock).Text = Resource.BandedRowColor;
            this._excelButton = GetTemplateChild(Element_ExcelButton) as Button;
            this._excelButton.Click += new RoutedEventHandler(_excelButton_Click);
            this._excelButton.Content = Resource.btn_Excel;

            (GetTemplateChild("ExportData") as TextBlock).Text = Resource.ExportTableData;

            this.m_tbResult = GetTemplateChild(Element_tbResult) as TextBlock;
            this._txtSettingName = GetTemplateChild(Element_GridSettingName) as TextBox;
            this._gridSettingsComboBox = GetTemplateChild(ElementGridSettingsComboBoxName) as ComboBox;
            this._gridSettingsComboBox.DisplayMemberPath = "Name";
            this._gridSettingsComboBox.ItemsSource = this.m_settingsCollection;
            this._gridSettingsComboBox.SelectionChanged += new SelectionChangedEventHandler(_gridSettingsComboBox_SelectionChanged);


            (GetTemplateChild("txtPageSize") as TextBlock).Text = Resource.Pager_PerPage;

            if (_cmbPageSize != null)
            {
                _cmbPageSize.SelectionChanged -= new SelectionChangedEventHandler(_cmbPageSize_SelectionChanged);
            }
            this._cmbPageSize = GetTemplateChild(Element_PageSize) as ComboBox;
            this._cmbPageSize.SelectionChanged += new SelectionChangedEventHandler(_cmbPageSize_SelectionChanged);

            this._resetButton = GetTemplateChild(Element_ButtonReset) as Button;
            this._resetButton.Content = Resource.ColumnsV3_Reset;
            this._resetButton.Click += new RoutedEventHandler(_resetButton_Click);

            this._backgroundColorPicker = GetTemplateChild(ElementBackgroundColorPickerName) as ComboBox;
            this._backgroundColorPicker.ItemsSource = new ColorsHelper().GetColorSet();
            this._backgroundColorPicker.SelectionChanged += new SelectionChangedEventHandler(_backgroundColorPicker_SelectionChanged);
            this._bandedColorPicker = GetTemplateChild(ElementBandedColorPickerName) as ComboBox;
            this._bandedColorPicker.SelectionChanged += new SelectionChangedEventHandler(_bandedColorPicker_SelectionChanged);
            this._bandedColorPicker.ItemsSource = new ColorsHelper().GetColorSet();

            this._elementRoot = GetTemplateChild(RootElementName) as FrameworkElement;
            this._expanderButton = GetTemplateChild(ExpanderButtonName) as ToggleButton;
            this._elementPopup = GetTemplateChild(ElementPopupName) as Popup;
            this._elementPopup.Opened += new EventHandler(_elementPopup_Opened);
            this._elementPopup.Closed += new EventHandler(_elementPopup_Closed);

            this._elementContentPresenterBorder = GetTemplateChild(ContentPresenterBorder) as FrameworkElement;

            this._customizeButton = GetTemplateChild(ElementCustomizeButtonName) as Button;
            if (this._customizeButton != null)
            {
                _customizeButton.Content = Resource.ColumnsV2_Title;
                this._customizeButton.Click += new RoutedEventHandler(_customizeButton_Click);
            }
            this._rowHeightInput = GetTemplateChild(RowHeightInput) as NumericUpDown;
            //this._rowHeightInput.LostFocus += new RoutedEventHandler(_rowHeightInput_LostFocus);
            this._rowHeightInput.ValueChanged += new RoutedPropertyChangedEventHandler<double>(_rowHeightInput_ValueChanged);

            this._deleteButton = GetTemplateChild(Element_DeleteButton) as Button;
            this._deleteButton.Content = Resource.ColumnsV2_btnDelete;
            this._deleteButton.Click += new RoutedEventHandler(_deleteButton_Click);

            if (this._elementPopup != null)
            {
                this._elementPopupChild = this._elementPopup.Child as FrameworkElement;
                this._elementOutsidePopup = new Canvas();
            }
            else
            {
                this._elementPopupChild = null;
                this._elementOutsidePopup = null;
            }
            if (this._elementOutsidePopup != null)
            {
                this._elementOutsidePopup.Background = new SolidColorBrush(Colors.Transparent);
                this._elementOutsidePopup.MouseLeftButtonDown += new MouseButtonEventHandler(ElementOutsidePopup_MouseLeftButtonDown);
            }
            if (this._elementPopupChild != null)
            {
                this._elementPopupChild.SizeChanged += new SizeChangedEventHandler(this.ElementPopupChild_SizeChanged);
                this._elementPopupChildCanvas = new Canvas();
            }
            else
            {
                this._elementPopupChildCanvas = null;
            }
            if ((this._elementPopupChildCanvas != null) && (this._elementOutsidePopup != null))
            {
                this._elementPopup.Child = this._elementPopupChildCanvas;
                this._elementPopupChildCanvas.Children.Add(this._elementOutsidePopup);
                this._elementPopupChildCanvas.Children.Add(this._elementPopupChild);
            }

            //this._expanderButton.Click += new RoutedEventHandler(_expanderButton_Click);
            this._expanderButton.Checked += new RoutedEventHandler(_expanderButton_Checked);
            this._expanderButton.Unchecked += new RoutedEventHandler(_expanderButton_Unchecked);


            base.SizeChanged += new SizeChangedEventHandler(this.ElementPopupChild_SizeChanged);
            this.IsOpen = false;

           
            this.m_resetGridConfig = this.OwningGrid.m_resetGridConfig;

            RefreshGridConfig(true);
            InitPageSizeSet(ref this._cmbPageSize, this.OwningGrid);
            if (this.OwningGrid.m_customizeDialog != null)
            {
                //判断是否启用DataGridCustomizeColumn
                if (this.OwningGrid.m_customizeDialog.IsEnableCustomizeColumn())
                {
                    _customizeButton.Visibility = Visibility.Visible;
                    this.OwningGrid.m_customizeDialog.CheckDataGridColumnChanged();
                }
            }

            if (this.OwningGrid.DisableProfileRowHeight)
            {
                _rowHeightInput.IsEnabled = false;
                (GetTemplateChild("GridRowHeight") as Grid).Visibility = Visibility.Collapsed;
                (GetTemplateChild("GridRowHeightLine") as Rectangle).Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 初始化每页显示数量集合
        /// </summary>
        /// <param name="cmbpageSizeSet"></param>
        /// <param name="dataGrid"></param>
        public void InitPageSizeSet(ref ComboBox cmbpageSizeSet, Newegg.Oversea.Silverlight.Controls.Data.DataGrid dataGrid)
        {
            cmbpageSizeSet.ItemsSource = dataGrid.PageSizeSet == null ? GetDefaultPageSizeSet() : GetCustomPageSizeSet(dataGrid);

            if (dataGrid.PageSizeSet != null && dataGrid.PageSizeSet.Count > 0)
            {
                var b = dataGrid.PageSizeSet.Any(item => item.IsDefault == true);

                Queue<int> queue = new Queue<int>();

                foreach (var i in dataGrid.PageSizeSet)
                {
                    if (!b)
                    {
                        queue.Enqueue(i.PageSize);
                    }
                    else if (b && i.IsDefault)
                    {
                        queue.Enqueue(i.PageSize);
                    }
                }

                var minValue = queue.Dequeue();

                while (queue.Count > 0)
                {
                    var i = queue.Dequeue();

                    if (i < minValue)
                    {
                        minValue = i;
                    }
                }

                m_resetGridConfig.PageSize = minValue;
                dataGrid.m_resetGridConfig.PageSize = minValue;
                dataGrid.PageSize = minValue;
            }


            foreach (var item in cmbpageSizeSet.Items)
            {
                var comboBoxItem = item as ComboBoxItem;

                if (dataGrid != null)
                {
                    if (!dataGrid.IsShowPager)
                    {
                        if (comboBoxItem.Tag.ToString() == "10000")
                        {
                            cmbpageSizeSet.SelectedItem = comboBoxItem;
                            cmbpageSizeSet.IsEnabled = false;
                            break;
                        }
                    }
                    else
                    {
                        if (int.Parse(comboBoxItem.Tag.ToString()) == dataGrid.PageSize)
                        {
                            cmbpageSizeSet.SelectedItem = comboBoxItem;
                            break;
                        }
                    }
                }
            }

            if (cmbpageSizeSet.SelectedItem == null)
            {
                cmbpageSizeSet.SelectedIndex = 0;
            }
        }


        private IEnumerable GetCustomPageSizeSet(Newegg.Oversea.Silverlight.Controls.Data.DataGrid dataGrid)
        {
            var set = new ObservableCollection<ComboBoxItem>();

            foreach (var item in dataGrid.PageSizeSet)
            {
                set.Add(new ComboBoxItem { Content = item.DisplayName, Tag = item.PageSize });
            }

            return set;
        }

        private IEnumerable GetDefaultPageSizeSet()
        {
            var set = new ObservableCollection<ComboBoxItem>();
            set.Add(new ComboBoxItem { Content = 10, Tag = 10 });
            set.Add(new ComboBoxItem { Content = 25, Tag = 25 });
            set.Add(new ComboBoxItem { Content = 50, Tag = 50 });
            set.Add(new ComboBoxItem { Content = 100, Tag = 100 });
            set.Add(new ComboBoxItem { Content = 200, Tag = 200 });
            set.Add(new ComboBoxItem { Content = 500, Tag = 500 });
            set.Add(new ComboBoxItem { Content = Resource.Pager_PageSizeAll, Tag = 10000 });

            return set;
        }

        void _rowHeightInput_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double height = _rowHeightInput.Value;

            if (this.OwningGrid.ActualRowHeight <= height)
            {
                this.OwningGrid.RowHeight = height;
            }

            if (!_isReset)
                SaveConfig(false);
        }



        #endregion

        #region Methods

        void ArrangePopup()
        {
            if (((this._elementPopup != null) && (this._elementPopupChild != null)) && (((this._elementContentPresenterBorder != null) && (this._elementOutsidePopup != null)) && this.IsOpen))
            {
                var content = Application.Current.Host.Content;
                double actualWidth = content.ActualWidth;
                double actualHeight = content.ActualHeight;
                double num3 = this._elementPopupChild.ActualWidth;
                double num4 = this._elementPopupChild.ActualHeight;
                if (((actualHeight != 0.0) && (actualWidth != 0.0)) && ((num3 != 0.0) && (num4 != 0.0)))
                {
                    GeneralTransform transform = null;
                    try
                    {
                        transform = this._elementContentPresenterBorder.TransformToVisual(null);
                    }
                    catch
                    {
                        this.IsOpen = false;
                    }
                    if (transform != null)
                    {
                        Point point = new Point(0.0, 0.0);
                        Point point2 = new Point(1.0, 0.0);
                        Point point3 = new Point(0.0, 1.0);
                        Point point4 = transform.Transform(point);
                        Point point5 = transform.Transform(point2);
                        Point point6 = transform.Transform(point3);
                        double x = point4.X;
                        double y = point4.Y;
                        //加上这句，防止X大于屏幕的宽度，这样算出来的位置不准确
                        x = Math.Min(x, actualWidth);
                        double num7 = base.ActualHeight;
                        double num8 = base.ActualWidth;
                        double maxDropDownHeight = this.MaxDropDownHeight;
                        if (double.IsInfinity(maxDropDownHeight) || double.IsNaN(maxDropDownHeight))
                        {
                            maxDropDownHeight = ((actualHeight - num7) * 3.0) / 5.0;
                        }
                        num3 = Math.Min(num3, actualWidth);
                        num4 = Math.Min(num4, maxDropDownHeight);
                        num3 = Math.Max(num8, num3);
                        double num10 = x;
                        if (actualWidth < (num10 + num3))
                        {
                            num10 = actualWidth - num3;
                            num10 = Math.Max(0.0, num10);
                        }
                        bool flag = true;
                        double num11 = y + num7;
                        if (actualHeight < (num11 + num4))
                        {
                            flag = false;
                            num11 = y - num4;
                            if (num11 < 0.0)
                            {
                                if (y < ((actualHeight - num7) / 2.0))
                                {
                                    flag = true;
                                    num11 = y + num7;
                                }
                                else
                                {
                                    flag = false;
                                    num11 = y - num4;
                                }
                            }
                        }
                        if (flag)
                        {
                            maxDropDownHeight = Math.Min(actualHeight - num11, maxDropDownHeight);
                        }
                        else
                        {
                            maxDropDownHeight = Math.Min(y, maxDropDownHeight);
                        }
                        this._elementPopup.HorizontalOffset = 0;
                        this._elementPopup.VerticalOffset = 0;
                        this._elementOutsidePopup.Width = actualWidth;
                        this._elementOutsidePopup.Height = actualHeight;
                        Matrix identity = Matrix.Identity;
                        identity.M11 = point5.X - point4.X;
                        identity.M12 = point5.Y - point4.Y;
                        identity.M21 = point6.X - point4.X;
                        identity.M22 = point6.Y - point4.Y;
                        identity.OffsetX -= point4.X;
                        identity.OffsetY -= point4.Y;
                        MatrixTransform transform2 = new MatrixTransform();
                        transform2.Matrix = identity;
                        this._elementOutsidePopup.RenderTransform = transform2;
                        this._elementPopupChild.MinWidth = num8;
                        this._elementPopupChild.MaxWidth = actualWidth;
                        this._elementPopupChild.MinHeight = num7;
                        this._elementPopupChild.MaxHeight = Math.Max(0.0, maxDropDownHeight);
                        this._elementPopupChild.HorizontalAlignment = HorizontalAlignment.Left;
                        this._elementPopupChild.VerticalAlignment = VerticalAlignment.Top;
                        var top = num11 - y;
                        Canvas.SetLeft(this._elementPopupChild, num10 - x);
                        if (top > 0)
                            Canvas.SetTop(this._elementPopupChild, top - 18);
                        else
                            Canvas.SetTop(this._elementPopupChild, top + 2);
                    }
                }
            }
        }

        internal void RefreshGridConfig(bool flag)
        {
            GridSetting currentConfig = null;

            m_settingsCollection.Clear();
            //增加初始设置
            var resetItem = new ComboBoxItem { Content = Resource.ColumnsV3_Reset, Name = RESET_NAME };
            m_settingsCollection.Insert(0, resetItem);

            if (this.OwningGrid.AllGridSettings.Count == 0)
            {
                this._deleteButton.IsEnabled = false;
            }

            if (this.OwningGrid.GridSettings != null && this.OwningGrid.GridSettings.Count > 0)
            {
                var reset = this.OwningGrid.GridSettings.FirstOrDefault(p => p.Name == RESET_NAME);
                if (reset != null)
                {
                    this.OwningGrid.GridSettings.Remove(reset);
                }

                this.OwningGrid.GridSettings.ForEach(p =>
                {
                    var item = new ComboBoxItem { Content = p.Name };
                    if (p.IsDefault)
                    {
                        item.Content = string.Format("{0}{1}", Default_Prefix, p.Name);
                    }
                    if (p.Name != RESET_NAME)
                    {
                        m_settingsCollection.Insert(1, item);
                    }
                });

                var defaultConfig = this.OwningGrid.GridSettings.FirstOrDefault(p => p.IsDefault);
                if (defaultConfig == null)
                {
                    currentConfig = this.OwningGrid.GridSettings[0];
                }
                else
                {
                    if (this.OwningGrid.ActualRowHeight < this._rowHeightInput.Maximum
                        && this.OwningGrid.ActualRowHeight > this._rowHeightInput.Minimum)
                    {
                        this._rowHeightInput.Minimum = this.OwningGrid.ActualRowHeight;
                    }
                    if (this.OwningGrid.ActualRowHeight != 99999
                        && this.OwningGrid.ActualRowHeight > this._rowHeightInput.Maximum)
                    {
                        this._rowHeightInput.IsEnabled = false;
                    }
                    else
                    {
                        this._rowHeightInput.IsEnabled = true;
                    }
                    currentConfig = defaultConfig;
                }
                this.OwningGrid.PageSize = currentConfig.PageSize;
                //_rowHeightInput.Value = currentConfig.RowHeight;
                //_txtSettingName.Text = currentConfig.Name;
                foreach (var item in this._gridSettingsComboBox.Items)
                {
                    if ((item as ComboBoxItem).Content.ToString().Replace(Default_Prefix, "") == currentConfig.Name)
                    {
                        var index = m_settingsCollection.IndexOf((item as ComboBoxItem));
                        this._gridSettingsComboBox.SelectedIndex = index;
                        break;
                    }
                }

                if (flag)
                    this.ApplyConfig(currentConfig);
            }
            else
            {
                if (flag)
                    this.ApplyConfig(m_resetGridConfig);
                _txtSettingName.Text = string.Empty;
                this._gridSettingsComboBox.SelectedIndex = 0;
            }
        }

        internal GridSetting GetResetGridConfig()
        {
            GridSetting gridConfig = new GridSetting()
            {
                Name = RESET_NAME,
                PageSize = this.OwningGrid.PageSize,
                GridGuid = this.OwningGrid.GridID,
                RowHeight = this.OwningGrid.RowHeight,
                RowBackground = TransFormColorToUint(this.OwningGrid.RowBackground),
                AlternatingRowBackground = TransFormColorToUint(this.OwningGrid.AlternatingRowBackground),
                Columns = new System.Collections.Generic.List<GridColumn>()
            };

            if (this.OwningGrid.NeedStoreColumns || this.OwningGrid.EnableCustomizeColumn)
            {
                foreach (var col in this.OwningGrid.Columns)
                {
                    var colName = col.GetColumnName();
                    var colConfig = new GridColumn()
                    {
                        Index = col.DisplayIndex,
                        IsFreezed = false,
                        IsHided = false,
                        Name = colName,
                        Width = col.Width,
                        ActualWidth = col.ActualWidth == 20 ? 100 : col.ActualWidth
                    };

                    gridConfig.Columns.Add(colConfig);
                }
            }
            return gridConfig;
        }

        /// <summary>
        /// Set Column's width, dispaly index and visibility
        /// </summary>
        /// <param name="currentConfig"></param>
        internal void ApplyConfig(GridSetting currentConfig)
        {
            this.OwningGrid.RowBackground = TransformUintToColor(currentConfig.RowBackground);
            this.OwningGrid.AlternatingRowBackground = TransformUintToColor(currentConfig.AlternatingRowBackground);

            if (this.OwningGrid.ActualRowHeight <= currentConfig.RowHeight && !this.OwningGrid.DisableProfileRowHeight)
            {
                this.OwningGrid.RowHeight = currentConfig.RowHeight;
            }
            this._rowHeightInput.Value = currentConfig.RowHeight.CompareTo(double.NaN) == 0 ? this._rowHeightInput.Minimum : currentConfig.RowHeight;

            if (this.OwningGrid.IsShowPager)
            {
                var item = this._cmbPageSize.Items.SingleOrDefault(i => int.Parse(((ComboBoxItem)i).Tag.ToString()) == currentConfig.PageSize);
                if (item != null)
                {
                    this._cmbPageSize.SelectedItem = item;
                }
            }


            foreach (var colorItem in _backgroundColorPicker.Items)
            {
                var color = colorItem as ColorDescription;
                if (color.ColorCode == currentConfig.RowBackground)
                {
                    var index = _backgroundColorPicker.Items.IndexOf(colorItem);
                    _backgroundColorPicker.SelectedIndex = index;
                }
            }
            foreach (var colorItem in _bandedColorPicker.Items)
            {
                var color = colorItem as ColorDescription;
                if (color.ColorCode == currentConfig.AlternatingRowBackground)
                {
                    var index = _bandedColorPicker.Items.IndexOf(colorItem);
                    _bandedColorPicker.SelectedIndex = index;
                }
            }

            if (this.OwningGrid.NeedStoreColumns || this.OwningGrid.EnableCustomizeColumn)
            {
                ApplyColumns(currentConfig);
            }
        }

        internal void ApplyColumns(GridSetting currentConfig)
        {
            if (!this.OwningGrid.m_customizeDialog.IsEnableCustomizeColumn())
            {
                return;
            }
            if (currentConfig.Columns != null)
            {
                foreach (var col in this.OwningGrid.Columns)
                {
                    var name = Extensions.GetColumnName(col);
                    var c = currentConfig.Columns.FirstOrDefault(p => string.Equals(name, p.Name, StringComparison.OrdinalIgnoreCase));

                    if (c != null)
                    {
                        col.DisplayIndex = c.Index;
                        if (c.ActualWidth > 20)
                        {
                            col.Width = new DataGridLength(c.ActualWidth);
                        }
                        col.Visibility = c.IsHided ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
            }
        }

        private static void OnMaxDropDownHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColumnsSelectorV3)d).OnMaxDropDownHeightChanged((double)e.NewValue);
        }

        private void OnMaxDropDownHeightChanged(double newValue)
        {
            this.ArrangePopup();
        }

        Brush TransformUintToColor(uint colorCode)
        {
            byte[] bytes = BitConverter.GetBytes(colorCode);
            var color = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
            return new SolidColorBrush(color);
        }

        internal uint TransFormColorToUint(Brush brush)
        {
            Color color = (brush as SolidColorBrush).Color;
            byte[] bytes = new byte[4];
            bytes[3] = color.A;
            bytes[2] = color.R;
            bytes[1] = color.G;
            bytes[0] = color.B;

            return BitConverter.ToUInt32(bytes, 0);
        }


        internal void SaveConfig()
        {
            SaveConfig(true);
        }

        internal void SaveConfig(bool isStoreColumns)
        {
            var name = this.OwningGrid.GridID;
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            GridSetting config = null;
            if (this.OwningGrid.GridSettings != null)
            {
                config = this.OwningGrid.GridSettings.FirstOrDefault(p => p.IsDefault);
            }
            else
            {
                this.OwningGrid.GridSettings = new List<GridSetting>();
            }

            if (this.OwningGrid.IsShowColumnsSelector)
            {
                if (this.OwningGrid.ActualRowHeight <= _rowHeightInput.Value && !this.OwningGrid.DisableProfileRowHeight)
                {
                    this.OwningGrid.RowHeight = _rowHeightInput.Value;
                }
            }

            if (config == null)
            {
                config = new GridSetting
                {
                    GridGuid = this.OwningGrid.GridID,
                    Name = "Default Setting",
                    PageSize = this.OwningGrid.PageSize,
                    IsDefault = true,
                    RowHeight = this.OwningGrid.IsShowColumnsSelector ? _rowHeightInput.Value : this.OwningGrid.RowHeight,
                    RowBackground = TransFormColorToUint(this.OwningGrid.RowBackground),
                    AlternatingRowBackground = TransFormColorToUint(this.OwningGrid.AlternatingRowBackground),
                    Columns = GetColumns()
                };
            }
            else
            {
                config.PageSize = this.OwningGrid.PageSize;
                config.RowHeight = this.OwningGrid.IsShowColumnsSelector ? _rowHeightInput.Value : this.OwningGrid.RowHeight;
                config.RowBackground = TransFormColorToUint(this.OwningGrid.RowBackground);
                config.AlternatingRowBackground = TransFormColorToUint(this.OwningGrid.AlternatingRowBackground);
                if (isStoreColumns)
                {
                    config.Columns = GetColumns();
                }
            }

            var result = this.OwningGrid.GridSettings.FirstOrDefault(p => p.Name == config.Name);
            if (result == null)
            {
                this.OwningGrid.GridSettings.Insert(0, config);
            }
            else
            {
                var index = this.OwningGrid.GridSettings.IndexOf(result);
                this.OwningGrid.GridSettings[index] = config;
            }

            var allSettings = this.OwningGrid.AllGridSettings;

            this.OwningGrid.GridSettings.ForEach(p =>
            {
                var item = allSettings.FirstOrDefault(k => k.GridGuid == this.OwningGrid.GridID && k.Name == p.Name);
                allSettings.Remove(item);
            });

            this.OwningGrid.GridSettings.ForEach(p =>
            {
                allSettings.Add(p);
            });

            this.OwningGrid.m_userProfile.Set(GridKeys.KEY_UP_GRIDSETTINGS, allSettings, true);

            if (this.OwningGrid.IsShowColumnsSelector)
            {
                m_settingsCollection.Insert(0, new ComboBoxItem { Content = config.Name });
                if (this.OwningGrid.AllGridSettings.Count > 0)
                {
                    this._deleteButton.IsEnabled = true;
                }
            }
        }

        private List<GridColumn> GetColumns()
        {
            if (!this.OwningGrid.EnableCustomizeColumn)
            {
                if (!this.OwningGrid.NeedStoreColumns)
                {
                    return null;
                }
            }

            var columns = new List<GridColumn>();

            foreach (var col in this.OwningGrid.Columns)
            {
                var c = new GridColumn();
                c.ActualWidth =  col.ActualWidth;
                c.Name = Extensions.GetColumnName(col);
                c.Index = col.DisplayIndex;
                c.Width = col.Width;
                c.IsHided = col.Visibility == System.Windows.Visibility.Visible ? false : true;
                columns.Add(c);
            }

            return columns;
        }

        #endregion

        #region Events

        void _elementPopup_Closed(object sender, EventArgs e)
        {
            _expanderButton.IsChecked = false;
        }

        void _elementPopup_Opened(object sender, EventArgs e)
        {
            _expanderButton.IsChecked = true;
        }

        void _expanderButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
        }

        void _expanderButton_Checked(object sender, RoutedEventArgs e)
        {
            this.OwningGrid.m_tracker.TraceEvent(string.Format("Click:{0}", "Settings"), string.Format("Grid:{0}", this.OwningGrid.GetLabel()));
            this.IsOpen = true;
        }

        void _excelButton_Click(object sender, RoutedEventArgs e)
        {
            Exporter.ExportDataGrid(this.OwningGrid);
        }

        void _deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var allGridSettings = this.OwningGrid.AllGridSettings;

            if (allGridSettings.Count == 0)
            {
                this._deleteButton.IsEnabled = false;
            }

            var comboxItem = this._gridSettingsComboBox.SelectedItem as ComboBoxItem;
            var config = allGridSettings.FirstOrDefault(p => p.Name == comboxItem.Content.ToString().Replace(Default_Prefix, ""));
            var deleted = this.OwningGrid.GridSettings.FirstOrDefault(p => p.Name == comboxItem.Content.ToString().Replace(Default_Prefix, ""));

            allGridSettings.Remove(config);
            this.OwningGrid.GridSettings.Remove(deleted);

            this.OwningGrid.m_userProfile.Set(GridKeys.KEY_UP_GRIDSETTINGS, allGridSettings, true);
            var item = this.m_settingsCollection.FirstOrDefault(p => p.Content.ToString().Replace(Default_Prefix, "") == config.Name);
            if (item != null)
            {
                m_settingsCollection.Remove(item);
            }
            RefreshGridConfig(true);
        }

        void _customizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.OwningGrid.m_customizeDialog.Dialog = dialog.ShowDialog(Resource.ProflieDataGrid_Title, this.OwningGrid.m_customizeDialog, null, new Size(800, 600), (Application.Current.RootVisual as UserControl).Content as Grid); 
            this.IsOpen = false;
        }

        void _gridSettingsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._gridSettingsComboBox.SelectedItem != null && e.RemovedItems.Count > 0 && e.AddedItems.Count > 0)
            {
                this._deleteButton.IsEnabled = true;
                var item = this._gridSettingsComboBox.SelectedItem as ComboBoxItem;
                var config = this.OwningGrid.GridSettings.FirstOrDefault(p => p.Name == item.Content.ToString().Replace(Default_Prefix, ""));

                if (item.Name == RESET_NAME)
                {
                    config = this.m_resetGridConfig;
                    _txtSettingName.Text = string.Empty;
                    this.OwningGrid.PageSize = m_resetGridConfig.PageSize;
                    this._deleteButton.IsEnabled = false;
                }
                _txtSettingName.Text = config.Name == RESET_NAME ? string.Empty : config.Name;
                this._rowHeightInput.Value = config.RowHeight.CompareTo(double.NaN) == 0 ? this._rowHeightInput.Minimum : config.RowHeight;
                this.OwningGrid.PageSize = config.PageSize;

                this.ApplyConfig(config);
            }
        }

        void _rowHeightInput_LostFocus(object sender, RoutedEventArgs e)
        {
            double height = _rowHeightInput.Value;

            if (this.OwningGrid.ActualRowHeight <= height)
            {
                this.OwningGrid.RowHeight = height;
            }

            if (!_isReset)
                SaveConfig(false);
        }

        void _cmbPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var combo = sender as ComboBox;
            var comboBoxItem = combo.SelectedItem as ComboBoxItem;
            if (comboBoxItem != null)
            {
                var pageSize = int.Parse(comboBoxItem.Tag.ToString());

                this.PageSize = pageSize;
                this.OwningGrid.PageSize = pageSize;
                if (this.DataPager.Source != null && this.DataPager.ItemCount > 0)
                {
                    this.OwningGrid.Bind();
                }
            }

            if (!_isReset)
                SaveConfig(false);
        }

        void ElementOutsidePopup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsOpen = false;
        }

        void _expanderButton_Click(object sender, RoutedEventArgs e)
        {
            base.Focus();
            this.IsOpen = !this.IsOpen;
            if (base.Content is Control)
            {
                ((Control)base.Content).Focus();
            }
        }

        void ElementPopupChild_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ArrangePopup();
        }

        void _backgroundColorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Brush brush = (this._backgroundColorPicker.SelectedItem as ColorDescription).ARGB;
            this.OwningGrid.RowBackground = brush;

            if (!_isReset)
                SaveConfig(false);
        }

        void _bandedColorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Brush brush = (this._bandedColorPicker.SelectedItem as ColorDescription).ARGB;
            this.OwningGrid.AlternatingRowBackground = brush;

            if (!_isReset)
                SaveConfig(false);
        }

        void _resetButton_Click(object sender, RoutedEventArgs e)
        {
            _isReset = true;
            var resetConfig = m_resetGridConfig;
            this._rowHeightInput.Value = resetConfig.RowHeight.CompareTo(double.NaN) == 0 ? this._rowHeightInput.Minimum : resetConfig.RowHeight;

            //在没有启用分页的情况下，不需要对PageSize进行重置。
            if (this.OwningGrid.IsShowPager)
            {
                this.OwningGrid.PageSize = resetConfig.PageSize;
            }

            ApplyConfig(resetConfig);

            _isReset = false;
            SaveConfig();
        }

        #endregion
    }
}
