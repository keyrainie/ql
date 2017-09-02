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
using System.ComponentModel;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Primitives;
using Newegg.Oversea.Silverlight.Utilities;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace Newegg.Oversea.Silverlight.Controls.Data.Oversea
{
    public partial class CustomDataGridDialog : UserControl
    {
        public DataGrid OwningGrid { get; set; }

        private IUserProfile m_userProfile;
        private bool m_isNotNeedChangeColumn;

        private ObservableCollection<ICustomGridListItem> m_customProfilelist = null;

        internal IDialog Dialog { get; set; }

        public CustomDataGridDialog(DataGrid owner)
        {
            InitializeComponent();
            this.DataGridExample.IsNotCustomizeColumn = true;
            OwningGrid = owner;
            this.OwningGrid.Loaded += new RoutedEventHandler(OwningGrid_Loaded);
            m_userProfile = CPApplication.Current.Browser.Profile;
            Loaded += new RoutedEventHandler(CustomDataGridDialog_Loaded);
            this.DataGridExample.Loaded += new RoutedEventHandler(DataGridExample_Loaded);
        }

        void OwningGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (OwningGrid.EnableCustomizeColumn)
            {
                OwningGrid.m_columnsSelector.SaveConfig();
                InitDataGrid();
            }
        }

        #region Private Method
       

        void DataGridExample_Loaded(object sender, RoutedEventArgs e)
        {
            List<GridSetting> localSettings = this.OwningGrid.AllGridSettings;
            this.OwningGrid.GridSettings = localSettings.Where(p => p.GridGuid == this.OwningGrid.GridID).ToList();
            BuildCustomProfileList();
            GridSetting grid = this.OwningGrid.GridSettings.FirstOrDefault(p => p.IsDefault);
            grid.Columns.Sort((p1, p2) => p1.Index.CompareTo(p2.Index));
            int i = 0;
            foreach (var item in grid.Columns)
            {
                item.Index = i;
                var columnData = this.DataGridExample.Columns.FirstOrDefault(p => p.GetColumnName() == item.Name);
                if (columnData != null)
                {
                    columnData.DisplayIndex = i;
                    if (item.ActualWidth > 20)
                    {
                        columnData.Width = new DataGridLength(item.ActualWidth);
                    }
                    columnData.Visibility = item.IsHided ? Visibility.Collapsed : Visibility.Visible;
                }
                i++;
            }
        }

        void BuildCustomProfileList()
        {
            m_customProfilelist = new ObservableCollection<ICustomGridListItem>();
            foreach (var item in this.OwningGrid.GridSettings)
            {
                m_customProfilelist.Add(new CustomProfileItem 
                {
                    DefaultIconVisibility = item.IsDefault ? Visibility.Visible : Visibility.Collapsed,
                    DisplayContent = item.Name,
                    IsDeleting = false,
                    IsRenaming = false
                });
                if (item.IsDefault)
                {
                    TextBlockContent.Text = item.Name;
                    TextBlockNeedCoverProfileName.Text = item.Name;
                }
            }
            theOperator.ListSource = m_customProfilelist;
            theOperator.Dialog = this;
        }

        void CustomDataGridDialog_Loaded(object sender, RoutedEventArgs e)
        {
            InitUIByProfileData();
            if (this.ComboxOwnerList.ItemsSource == null)
            {
                this.ButtonCopy.IsEnabled = false;
                this.ComboxOwnerList.ItemsSource = new ObservableCollection<DataGridProfileItem>() { new DataGridProfileItem { Owner = new AuthUser { DisplayName = Resource.ProflieDataGrid_Loading } } };
                this.ComboxOwnerList.SelectedIndex = 0;
                this.ComboxOwnerList.SelectionChanged += new SelectionChangedEventHandler(ComboxOwnerList_SelectionChanged);

                this.ComboxGridProfileList.ItemsSource = new ObservableCollection<GridSetting>() { new GridSetting { Name = Resource.ProflieDataGrid_Loading } };
                this.ComboxGridProfileList.SelectedIndex = 0;

                this.OwningGrid.m_userProfile.GetDataGridProfileItems(this.OwningGrid.GridID, (result) =>
                {
                    if (result != null)
                    {
                        var list = result.Where(p => p.Owner.LoginName != CPApplication.Current.LoginUser.LoginName && p.Owner.LoginName != null);

                        if (list.Count() <= 0)
                        {
                            this.ComboxOwnerList.ItemsSource = new ObservableCollection<DataGridProfileItem>() { new DataGridProfileItem { Owner = new AuthUser { DisplayName = Resource.ProflieDataGrid_NoOtherProfileData } } };
                            this.ComboxGridProfileList.ItemsSource = new ObservableCollection<GridSetting>() { new GridSetting { Name = Resource.ProflieDataGrid_NoOtherProfileData } };
                            this.ComboxGridProfileList.SelectedIndex = 0;
                        }
                        else
                        {
                            this.ComboxOwnerList.ItemsSource = list;
                        }
                        this.ComboxOwnerList.SelectedIndex = -1;
                        this.ComboxOwnerList.SelectedIndex = 0;
                    }
                    else
                    {
                        this.ComboxOwnerList.ItemsSource = new ObservableCollection<DataGridProfileItem>() { new DataGridProfileItem { Owner = new AuthUser { DisplayName = Resource.ProflieDataGrid_ErrorInServer } } };
                        this.ComboxOwnerList.SelectedIndex = 0;
                        this.ComboxGridProfileList.ItemsSource = new ObservableCollection<GridSetting>() { new GridSetting { Name = Resource.ProflieDataGrid_ErrorInServer } };
                        this.ComboxGridProfileList.SelectedIndex = 0;
                    }
                });
            }
        }

        void ComboxOwnerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboxOwnerList.SelectedIndex != -1)
            {
                DataGridProfileItem item = ComboxOwnerList.SelectedItem as DataGridProfileItem;
                if (item != null && item.DataGridProfileXml != null)
                {
                    var list = UtilityHelper.XmlDeserialize<List<GridSetting>>(item.DataGridProfileXml);
                    if (list != null)
                    {
                        this.ComboxGridProfileList.ItemsSource = list.Where(p => p.GridGuid == this.OwningGrid.GridID);
                        this.ButtonCopy.IsEnabled = true;
                        this.ComboxGridProfileList.SelectedIndex = 0;
                    }
                }
            }
        }

        void InitDataGrid()
        {
            this.DataGridExample.GridID = this.OwningGrid.GridID;
            this.DataGridExample.IsShowColumnsSelector = false;
            this.DataGridExample.NeedStoreColumns = true;
            foreach (var col in OwningGrid.Columns)
            {
                if (col is DataGridTemplateColumn)
                {
                    var colTemp = col as DataGridTemplateColumn;
                    var newCol = new DataGridTemplateColumn { Width = colTemp.Width, Name = colTemp.Name, CellTemplate = colTemp.CellTemplate };
                    DataGridAttached.SetHeader(newCol, DataGridAttached.GetHeader(colTemp));
                    this.DataGridExample.Columns.Add(newCol);
                }
                else if (col is DataGridTextColumn)
                {
                    var colTemp = col as DataGridTextColumn;
                    var newCol = new DataGridTextColumn { Width = colTemp.Width, Name = colTemp.Name, Binding = new System.Windows.Data.Binding { Converter = colTemp.Binding.Converter, ConverterParameter = colTemp.Binding.ConverterParameter, Mode = colTemp.Binding.Mode, Path = colTemp.Binding.Path } };
                    DataGridAttached.SetHeader(newCol, DataGridAttached.GetHeader(colTemp));
                    this.DataGridExample.Columns.Add(newCol);
                }
            }
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveDeletedItem();
            SaveConfig();
            CancelButton_Click(null, null);
        }

        void SaveConfig()
        {
            SaveProfileData(true);
            var allSettings = this.OwningGrid.AllGridSettings;
            allSettings.RemoveAll(k => k.GridGuid == this.OwningGrid.GridID);
            this.OwningGrid.GridSettings.ForEach(p =>
            {
                allSettings.Add(p);
            });
            this.OwningGrid.m_userProfile.Set(GridKeys.KEY_UP_GRIDSETTINGS, allSettings, true);
        }

        void RemoveDeletedItem()
        {
            foreach (var item in m_customProfilelist)
            {
                if (item.IsDeleting)
                {
                    var result = this.OwningGrid.GridSettings.FirstOrDefault(p => p.Name == item.DisplayContent);
                    if (result != null)
                    {
                        this.OwningGrid.GridSettings.Remove(result);
                    }
                }
            }
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.Close();
        }


        List<GridColumn> GetColumns()
        {
            if (!this.OwningGrid.EnableCustomizeColumn)
            {
                if (!this.DataGridExample.NeedStoreColumns)
                {
                    return null;
                }
            }

            var columns = new List<GridColumn>();

            foreach (var col in this.DataGridExample.Columns)
            {
                var c = new GridColumn();
                c.ActualWidth = col.ActualWidth;
                c.Name = Extensions.GetColumnName(col);
                c.Index = col.DisplayIndex;
                c.Width = col.Width;
                c.IsHided = col.Visibility == System.Windows.Visibility.Visible ? false : true;
                columns.Add(c);
            }

            return columns;
        }

        void InitUIByProfileData()
        {
            GridSetting grid = this.OwningGrid.GridSettings.FirstOrDefault(p => p.IsDefault);
            grid.Columns.Sort((p1, p2) => p1.Index.CompareTo(p2.Index));

            foreach (var column in this.OwningGrid.Columns)
            {
                var name = string.Empty;
                try
                {
                    name = DataGridAttached.GetHeader(column).ToString();
                }
                catch
                {
                    name = column.GetColumnName();
                }

                grid.Columns.FirstOrDefault(p => p.Name == column.GetColumnName()).DisplayName = name;
            }

            ObservableCollection<GridColumn> collectionSelected = new ObservableCollection<GridColumn>(grid.Columns.Where(p => !p.IsHided));
            collectionSelected.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collectionSelected_CollectionChanged);
            ListBoxSelectedColumns.ItemsSource = collectionSelected;

            ObservableCollection<GridColumn> collectionAvailable = new ObservableCollection<GridColumn>(grid.Columns.Where(p => p.IsHided));
            ListBoxAvailableColumns.ItemsSource = collectionAvailable;
        }

        void collectionSelected_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() => 
            {
                ObservableCollection<GridColumn> collection = sender as ObservableCollection<GridColumn>;
                int i = 0;
                foreach (var item in collection)
                {
                    item.Index = i;
                    var columnData = this.DataGridExample.Columns.FirstOrDefault(p => p.GetColumnName() == item.Name);
                    if (columnData != null)
                    {
                        columnData.DisplayIndex = i;
                        if (!m_isNotNeedChangeColumn)
                        {
                            if (item.ActualWidth == 0)
                            {
                                columnData.Width = item.Width;
                            }
                            else
                            {
                                columnData.Width = new DataGridLength(item.ActualWidth);
                            }
                        }
                        columnData.Visibility = item.IsHided ? Visibility.Collapsed : Visibility.Visible;
                    }
                    i++;
                }

                if (m_isNotNeedChangeColumn)
                {
                    m_isNotNeedChangeColumn = false;
                }
            });
        }

        void ButtonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            var selectedColumns = ListBoxSelectedColumns.ItemsSource as ObservableCollection<GridColumn>;
            selectedColumns.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collectionSelected_CollectionChanged);
            var availableColumns = ListBoxAvailableColumns.ItemsSource as ObservableCollection<GridColumn>;
            for (int i = availableColumns.Count - 1; i > -1; i--)
            {
                availableColumns[i].IsHided = false;
                selectedColumns.Add(availableColumns[i]);
                availableColumns.Remove(availableColumns[i]);
            }
            selectedColumns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collectionSelected_CollectionChanged);
            m_isNotNeedChangeColumn = true;
            collectionSelected_CollectionChanged(selectedColumns, null);
        }

        void ButtonUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            var selectedColumns = ListBoxSelectedColumns.ItemsSource as ObservableCollection<GridColumn>;
            selectedColumns.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collectionSelected_CollectionChanged);
            var availableColumns = ListBoxAvailableColumns.ItemsSource as ObservableCollection<GridColumn>;
            for (int i = selectedColumns.Count - 1; i > -1; i--)
            {
                selectedColumns[i].IsHided = true;
                availableColumns.Add(selectedColumns[i]);
                selectedColumns.Remove(selectedColumns[i]);
            }
            selectedColumns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collectionSelected_CollectionChanged);
            m_isNotNeedChangeColumn = true;
            collectionSelected_CollectionChanged(availableColumns, null);
        }

        #endregion

        #region Internel Method

        internal bool IsEnableCustomizeColumn()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                if (string.IsNullOrEmpty(this.OwningGrid.GridID))
                {
                    return false;
                }

                if (!this.OwningGrid.EnableCustomizeColumn)
                {
                    return false;
                }

                if (OwningGrid.Columns != null && OwningGrid.Columns.Count > 0) 
                {
                    foreach (var col in OwningGrid.Columns)
                    {
                        if (col is DataGridTextColumn)
                        {
                            if ((col as DataGridTextColumn).Name == null)
                                return false;
                        }
                        else if (col is DataGridTemplateColumn)
                        {
                            if ((col as DataGridTemplateColumn).Name == null)
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        internal void CheckDataGridColumnChanged()
        {
            bool isNeedChanged = false;
            GridSetting defaultGridSetting = this.OwningGrid.GridSettings.FirstOrDefault(p => p.IsDefault);
            List<GridColumn> columns = UtilityHelper.DeepClone<List<GridColumn>>(defaultGridSetting.Columns);
            foreach (var col in columns)
            {
                var findResult = this.OwningGrid.Columns.FirstOrDefault(colTemp => colTemp.GetColumnName() == col.Name);
                if (findResult == null)
                {
                    isNeedChanged = true;
                    RemoveCol(col.Name);
                }
            }

            foreach (var col in this.OwningGrid.Columns)
            {
                var findResult = defaultGridSetting.Columns.FirstOrDefault(colTemp => col.GetColumnName() == colTemp.Name);
                if (findResult == null)
                {
                    isNeedChanged = true;
                    var name = string.Empty;
                    try
                    {
                        name = DataGridAttached.GetHeader(col).ToString();
                    }
                    catch
                    {
                        name = col.GetColumnName();
                    }
                    col.Visibility = Visibility.Collapsed;
                    this.DataGridExample.Columns[this.OwningGrid.Columns.IndexOf(col)].Visibility = Visibility.Collapsed;
                    AddCol(new GridColumn
                    {
                        ActualWidth = col.ActualWidth,
                        Width = col.Width,
                        DisplayName = name,
                        Index = col.DisplayIndex,
                        IsFreezed = false,
                        IsHided = true,
                        Name = col.GetColumnName()
                    });
                }
            }
            if (isNeedChanged)
            {
                var allSettings = this.OwningGrid.AllGridSettings;
                allSettings.RemoveAll(k => k.GridGuid == this.OwningGrid.GridID);
                this.OwningGrid.GridSettings.ForEach(p =>
                {
                    allSettings.Add(p);
                });
                this.OwningGrid.m_userProfile.Set(GridKeys.KEY_UP_GRIDSETTINGS, allSettings, true);
            }
        }

        void RemoveCol(string name)
        {
            foreach (var setting in this.OwningGrid.GridSettings)
            {
                setting.Columns.RemoveAll(p => p.Name == name);
            }
        }
        void AddCol(GridColumn col)
        {
            foreach (var setting in this.OwningGrid.GridSettings)
            {
                setting.Columns.Add(col);
            }
        }

        internal void ConvertOldProfileDataToNewProfileData()
        {
           var settings = m_userProfile.Get<List<Setting>>(UserSetting.s_userSetting_Key);
           if (settings != null)
           {
               var m_currentSetting = settings.FirstOrDefault(p => string.Equals(p.Guid, this.OwningGrid.GridID, StringComparison.OrdinalIgnoreCase));
               if (m_currentSetting != null)
               {
                   GridSetting setting = this.OwningGrid.GridSettings.FirstOrDefault();
                   if (setting == null)
                   {
                       setting = UtilityHelper.DeepClone<GridSetting>(this.OwningGrid.m_resetGridConfig);
                       setting.IsDefault = true;
                   }
                   setting.Name = "Default Setting";

                   if (setting.Columns.Count == m_currentSetting.Columns.Count)
                   {
                       for (int i = 0; i < setting.Columns.Count; i++)
                       {
                           setting.Columns[i].Index = m_currentSetting.Columns[i].DisplayIndex;
                           setting.Columns[i].ActualWidth = m_currentSetting.Columns[i].ActualWidth == this.OwningGrid.Columns[i].ActualWidth ? m_currentSetting.Columns[i].ActualWidth : this.OwningGrid.Columns[i].ActualWidth;
                           setting.Columns[i].Width = this.OwningGrid.Columns[i].Width;
                           setting.Columns[i].IsFreezed = false;
                           setting.Columns[i].IsHided = false;
                           setting.Columns[i].Name = this.OwningGrid.Columns[i].GetColumnName();
                       }
                   }
                   //移除以前老版本的ColumnProfile数据；
                   settings.Remove(m_currentSetting);
                   m_userProfile.Set(UserSetting.s_userSetting_Key, settings);

                   //添加转换后的ColumnProfile数据到新的ProfileKey中；
                   List<GridSetting> localSettings = this.OwningGrid.AllGridSettings;
                   var result = localSettings.FirstOrDefault(p => p.Name == setting.GridGuid && p.GridGuid == setting.GridGuid);
                   if (result != null)
                   {
                       localSettings.Remove(result);
                       this.OwningGrid.GridSettings.Clear();
                   }
                   localSettings.Add(setting);
                   this.OwningGrid.GridSettings.Add(setting);
                   m_userProfile.Set(GridKeys.KEY_UP_GRIDSETTINGS, localSettings);
               }
           }
        }

        internal void ApplyProfileData(string name, string guid)
        {
            var result = this.OwningGrid.GridSettings.FirstOrDefault(p => p.Name == name && p.GridGuid == guid);
            if (result != null)
            {
                OwningGrid.m_columnsSelector.ApplyConfig(result);
            }
        }

        internal void SaveProfileData()
        {
            this.SaveProfileData(false);
        }
        internal void SaveProfileData(bool isApplyConfig)
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

            if (config == null)
            {
                config = new GridSetting
                {
                    GridGuid = this.OwningGrid.GridID,
                    Name = "Default Setting",
                    PageSize = this.OwningGrid.PageSize,
                    IsDefault = true,
                    RowHeight = this.OwningGrid.IsShowColumnsSelector ? this.OwningGrid.m_columnsSelector._rowHeightInput.Value : this.OwningGrid.RowHeight,
                    RowBackground = this.OwningGrid.m_columnsSelector.TransFormColorToUint(this.OwningGrid.RowBackground),
                    AlternatingRowBackground = this.OwningGrid.m_columnsSelector.TransFormColorToUint(this.OwningGrid.AlternatingRowBackground),
                    Columns = GetColumns()
                };
            }
            else
            {
                config.Columns = GetColumns();
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

            if (isApplyConfig)
            {
                this.OwningGrid.m_columnsSelector.ApplyConfig(config);
            }
            if (config.IsDefault)
            {
                TextBlockContent.Text = config.Name;
                TextBlockNeedCoverProfileName.Text = config.Name;
            }
        }

        internal void RefreshDefaultProfileData()
        {
            InitUIByProfileData();
            GridSetting grid = this.OwningGrid.GridSettings.FirstOrDefault(p => p.IsDefault);
            grid.Columns.Sort((p1, p2) => p1.Index.CompareTo(p2.Index));
            collectionSelected_CollectionChanged(new ObservableCollection<GridColumn>(grid.Columns), null);
        }

        internal void SetDefault(string name)
        {
            foreach (var item in this.OwningGrid.GridSettings)
            {
                if (item.Name == name)
                {
                    item.IsDefault = true;
                    this.TextBlockContent.Text = name;
                    TextBlockNeedCoverProfileName.Text = name;
                }
                else
                {
                    item.IsDefault = false;
                }
            }
        }

        internal void Rename(string name,string newName)
        {
            foreach (var item in this.OwningGrid.GridSettings)
            {
                if (item.Name == name)
                {
                    item.Name = newName;
                    return;
                }
            }
        }

        #endregion

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.MessageBox.Clear();
            var selectedGridSetting = this.ComboxGridProfileList.SelectedItem as GridSetting;
            if (selectedGridSetting != null)
            {
                var settingCopy = UtilityHelper.DeepClone<GridSetting>(selectedGridSetting);
                var findResult = this.OwningGrid.GridSettings.FirstOrDefault(p => p.Name == this.TextBlockContent.Text);
                if (findResult != null)
                {
                    this.OwningGrid.GridSettings.Remove(findResult);
                    settingCopy.IsDefault = true;
                    settingCopy.Name = this.TextBlockContent.Text;
                    this.OwningGrid.GridSettings.Add(settingCopy);
                    this.RefreshDefaultProfileData();
                }
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(Resource.ProflieDataGrid_CopySuccessfully);
            }
        }

        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.MessageBox.Clear();
            if (ListBoxSelectedColumns.SelectedItem != null)
            {
                var selectedColumns = ListBoxSelectedColumns.ItemsSource as ObservableCollection<GridColumn>;
                var gridColumn = ListBoxSelectedColumns.SelectedItem as GridColumn;
                var currentPosition = selectedColumns.IndexOf(gridColumn);
                if (currentPosition == 0)
                {
                    return;
                }
                selectedColumns.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collectionSelected_CollectionChanged);
                selectedColumns.Remove(gridColumn);
                selectedColumns.Insert(currentPosition - 1, gridColumn);                
                selectedColumns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collectionSelected_CollectionChanged);
                m_isNotNeedChangeColumn = true;
                collectionSelected_CollectionChanged(selectedColumns, null);
                this.Dispatcher.BeginInvoke(() => 
                {
                    ListBoxSelectedColumns.SelectedIndex = currentPosition - 1;
                });
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(Resource.ProflieDataGrid_AdjustSequenceTip, MessageBoxType.Warning);
            }
        }

        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.MessageBox.Clear();
            if (ListBoxSelectedColumns.SelectedItem != null)
            {
                var selectedColumns = ListBoxSelectedColumns.ItemsSource as ObservableCollection<GridColumn>;
                var gridColumn = ListBoxSelectedColumns.SelectedItem as GridColumn;
                var currentPosition = selectedColumns.IndexOf(gridColumn);
                if (currentPosition + 1 == selectedColumns.Count)
                {
                    return;
                }
                selectedColumns.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collectionSelected_CollectionChanged);
                selectedColumns.Remove(gridColumn);
                selectedColumns.Insert(currentPosition + 1, gridColumn);
                selectedColumns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collectionSelected_CollectionChanged);
                m_isNotNeedChangeColumn = true;
                collectionSelected_CollectionChanged(selectedColumns, null);
                this.Dispatcher.BeginInvoke(() =>
                {
                    ListBoxSelectedColumns.SelectedIndex = currentPosition + 1;
                });
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(Resource.ProflieDataGrid_AdjustSequenceTip, MessageBoxType.Warning);
            }
        }

        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.MessageBox.Clear();
            if (ListBoxAvailableColumns.SelectedItem != null)
            {
                var selectedColumns = ListBoxSelectedColumns.ItemsSource as ObservableCollection<GridColumn>;
                var availableColumns = ListBoxAvailableColumns.ItemsSource as ObservableCollection<GridColumn>;
                var gridColumn = ListBoxAvailableColumns.SelectedItem as GridColumn;
                selectedColumns.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collectionSelected_CollectionChanged);
                gridColumn.IsHided = false;
                availableColumns.Remove(gridColumn);
                selectedColumns.Add(gridColumn);
                selectedColumns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collectionSelected_CollectionChanged);
                m_isNotNeedChangeColumn = true;
                collectionSelected_CollectionChanged(selectedColumns, null);
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(Resource.ProflieDataGrid_SelectedButtonTip, MessageBoxType.Warning);
            }
        }

        private void ButtonUnselect_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.MessageBox.Clear();
            if (ListBoxSelectedColumns.SelectedItem != null)
            {
                var selectedColumns = ListBoxSelectedColumns.ItemsSource as ObservableCollection<GridColumn>;
                var availableColumns = ListBoxAvailableColumns.ItemsSource as ObservableCollection<GridColumn>;
                var gridColumn = ListBoxSelectedColumns.SelectedItem as GridColumn;
                selectedColumns.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collectionSelected_CollectionChanged);
                gridColumn.IsHided = true;
                selectedColumns.Remove(gridColumn);
                availableColumns.Add(gridColumn);
                selectedColumns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collectionSelected_CollectionChanged);
                m_isNotNeedChangeColumn = true;
                List<GridColumn> columns = new List<GridColumn>();
                columns.AddRange(selectedColumns);
                columns.AddRange(availableColumns);
                collectionSelected_CollectionChanged(new ObservableCollection<GridColumn>(columns), null);
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(Resource.ProflieDataGrid_UnselectedButtonTip, MessageBoxType.Warning);
            }
        }

 
    }


    public class CustomProfileItem : ICustomGridListItem
    {
        public string DisplayContent
        {
            get;
            set;
        }

        public Visibility DefaultIconVisibility
        {
            get;
            set;
        }

        public bool IsDeleting
        {
            get;
            set;
        }

        public bool IsRenaming
        {
            get;
            set;
        }
    }
}

