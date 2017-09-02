using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Controls.Data;


namespace Newegg.Oversea.Silverlight.Controls.Primitives
{
    public class CustomizeColumns : ContentControl
    {
        #region Constants

        private const string Element_RootElement = "RootElement";
        private const string Element_lbAvailableColumns = "lbAvailableColumns";
        private const string Element_lbSelectedColumns = "lbSelectedColumns";
        private const string Element_btnUp = "btnUp";
        private const string Element_btnDown = "btnDown";
        private const string Element_txtWidth = "txtWidth";
        private const string Element_cmbPageSize = "cmbPageSize";
        private const string Element_tbWidth = "tbWidth";
        private const string Element_tbName = "tbName";
        private const string Element_tbPageSize = "tbPageSize";
        private const string Element_tbAvailableColumns = "tbAvailableColumns";
        private const string Element_tbSelectedColumns = "tbSelectedColumns";
        private const string Element_txtName = "txtName";
        private const string Element_ComboSettings = "ComboSettings";
        private const string Element_btnNew = "hlbNew";
        private const string Element_btnSave = "hlbSave";
        private const string Element_btnDelete = "hlbDelete";
        private const string Element_tbResult = "tbResult";
        private const string Element_btnClose = "btnClose";
        private const string Element_chkDefault = "chkDefault";
        private const string ListBoxItemNamePrefix = "DataGrid_ListBoxItem_";
        private const double MAXWIDTH = 999;
        private const string RESET_NAME = "[Reset]";
        private const string Default_Prefix = "[D]";

        #endregion

        #region Data

        private string m_listBoxItemName_Prefix = "DataGrid_ListBoxItem_{0}";
        private ListBox m_availableColumnsListBox;
        private ListBox m_selectedColumnsListBox;
        private Grid m_rootElement;
        private TextBox m_txtWidth;
        private TextBox m_txtName;
        private Button m_btnUp;
        private HyperlinkButton m_btnNew;
        private HyperlinkButton m_btnSave;
        private HyperlinkButton m_btnDelete;
        private Button m_btnDown;
        private ComboBox m_comboSettings;
        private ComboBox m_cmbPageSize;
        private TextBlock m_tbResult;
        private Button m_btnClose;
        private CheckBox m_chkDefault;
        private ObservableCollection<ListBoxItemExt> m_availableColumns = new ObservableCollection<ListBoxItemExt>();
        private ObservableCollection<ListBoxItemExt> m_selectedColumns = new ObservableCollection<ListBoxItemExt>();
        private bool m_needRefresh = false;//删除ComboxItem后是否需要刷新ListBox       

        #endregion

        internal Data.DataGrid OwningGrid { get; set; }
        internal GridSetting CurrentGridConfig { get; set; }
        internal GridSetting DeletingGridConfig { get; set; }
        internal ChildWindow Container { get; set; }

        public CustomizeColumns()
        {
            DefaultStyleKey = typeof(CustomizeColumns);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.m_txtWidth = GetTemplateChild(Element_txtWidth) as TextBox;
            this.m_txtName = GetTemplateChild(Element_txtName) as TextBox;
            this.m_rootElement = GetTemplateChild(Element_RootElement) as Grid;
            this.m_comboSettings = GetTemplateChild(Element_ComboSettings) as ComboBox;
            this.m_cmbPageSize = GetTemplateChild(Element_cmbPageSize) as ComboBox;
            (GetTemplateChild(Element_tbAvailableColumns) as TextBlock).Text = Resource.ColumnsV2_tbAvailableColumns;
            (GetTemplateChild(Element_tbSelectedColumns) as TextBlock).Text = Resource.ColumnsV2_tbSelectedColumns;
            (GetTemplateChild(Element_tbWidth) as TextBlock).Text = Resource.ColumnsV2_tbWidth;
            (GetTemplateChild(Element_tbName) as TextBlock).Text = Resource.ColumnsV2_tbName;
            (GetTemplateChild(Element_tbPageSize) as TextBlock).Text = Resource.ColumnsV2_tbPageSize;
            this.m_chkDefault = GetTemplateChild(Element_chkDefault) as CheckBox;
            if (this.m_chkDefault != null)
            {
                this.m_chkDefault.Content = Resource.ColumnsV2_chkDefault;
            }
            this.m_btnClose = GetTemplateChild(Element_btnClose) as Button;
            if (this.m_btnClose != null)
            {
                this.m_btnClose.Content = Resource.ColumnsV2_btnClose;
                this.m_btnClose.Click += new RoutedEventHandler(m_btnClose_Click);
            }

            this.m_tbResult = GetTemplateChild(Element_tbResult) as TextBlock;
            this.m_availableColumnsListBox = GetTemplateChild(Element_lbAvailableColumns) as ListBox;

            this.m_selectedColumnsListBox = GetTemplateChild(Element_lbSelectedColumns) as ListBox;
            if (this.m_btnNew != null)
            {
                this.m_btnNew.Click -= new RoutedEventHandler(m_btnNew_Click);
            }
            this.m_btnNew = GetTemplateChild(Element_btnNew) as HyperlinkButton;
            if (this.m_btnNew != null)
            {
                this.m_btnNew.Click += new RoutedEventHandler(m_btnNew_Click);
                ((this.m_btnNew.Content as StackPanel).Children[1] as TextBlock).Text = Resource.ColumnsV2_btnNew;
            }
            if (this.m_txtWidth != null)
            {
                this.m_txtWidth.TextChanged += new TextChangedEventHandler(m_txtWidth_TextChanged);
            }
            if (this.m_btnUp != null)
            {
                this.m_btnUp.Click -= new RoutedEventHandler(m_btnUp_Click);
            }
            this.m_btnUp = GetTemplateChild(Element_btnUp) as Button;
            if (this.m_btnUp != null)
            {
                this.m_btnUp.Click += new RoutedEventHandler(m_btnUp_Click);
            }
            if (this.m_btnDown != null)
            {
                this.m_btnDown.Click -= new RoutedEventHandler(m_btnDown_Click);
            }
            this.m_btnDown = GetTemplateChild(Element_btnDown) as Button;
            if (this.m_btnDown != null)
            {
                this.m_btnDown.Click += new RoutedEventHandler(m_btnDown_Click);
            }
            if (this.m_btnSave != null)
            {
                this.m_btnSave.Click += new RoutedEventHandler(m_btnSave_Click);
            }
            this.m_btnSave = GetTemplateChild(Element_btnSave) as HyperlinkButton;
            if (this.m_btnSave != null)
            {
                ((this.m_btnSave.Content as StackPanel).Children[1] as TextBlock).Text = Resource.ColumnsV2_btnSave;
                this.m_btnSave.Click += new RoutedEventHandler(m_btnSave_Click);
            }
            if (this.m_btnDelete != null)
            {
                this.m_btnDelete.Click -= new RoutedEventHandler(m_btnDelete_Click);
            }
            this.m_btnDelete = GetTemplateChild(Element_btnDelete) as HyperlinkButton;
            if (this.m_btnDelete != null)
            {
                ((this.m_btnDelete.Content as StackPanel).Children[1] as TextBlock).Text = Resource.ColumnsV2_btnDelete;
                this.m_btnDelete.Click += new RoutedEventHandler(m_btnDelete_Click);
            }
            this.m_selectedColumnsListBox.SelectionChanged += new SelectionChangedEventHandler(m_lstSelectedColumns_SelectionChanged);
            if (this.m_comboSettings != null)
            {
                this.m_comboSettings.SelectionChanged += new SelectionChangedEventHandler(m_comboSettings_SelectionChanged);
            }

            InitGridSettings();
        }

        #region Events

        void m_btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (this.Container != null)
            {
                this.Container.Close();
            }
        }

        void m_btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.m_tbResult.Text = string.Empty;
            try
            {
                var config = GetConfig();
                if (config.Columns != null && config.Columns.Count > 0)
                {
                    #region Isolatestorage Version

                    var settings = this.OwningGrid.GridSettings;
                    if (settings == null)
                    {
                        settings = new List<GridSetting>();
                        settings.Add(config);
                    }
                    else
                    {
                        if (config.IsDefault)
                        {
                            settings.ForEach(p =>
                            {
                                p.IsDefault = false;
                            });
                        }

                        var setting = settings.FirstOrDefault(p => p.Name == config.Name);
                        if (setting == null)
                        {
                            settings.Add(config);
                        }
                        else
                        {
                            var index = settings.IndexOf(setting);
                            settings[index] = config;
                        }
                    }
                    
                    this.OwningGrid.CurrentGridSettings = config;

                    this.m_comboSettings.Items.Clear();

                    settings.ForEach(p =>
                                         {
                                             var item = new ComboBoxItem() { Content = p.Name };
                                             if (p.IsDefault)
                                             {
                                                 item.Content = Default_Prefix + p.Name;
                                             }
                                             this.m_comboSettings.Items.Add(item);
                                         });
                    var idx = settings.IndexOf(config);
                    this.m_comboSettings.SelectedIndex = idx;

                    this.OwningGrid.RefreshGridSetting();

                    this.m_btnSave.Focus();
                    this.m_tbResult.Text = Resource.ColumnsV2_SaveSuccessfully;

                    this.OwningGrid.m_userProfile.Set(GridKeys.KEY_UP_GRIDSETTINGS, settings, true);

                    #endregion

                    #region old Version

                    //this.OwningGrid.m_userProfile.SaveGridSetting(config, setting =>
                    //        {
                    //            var isDefault = setting.IsDefault;

                    //            this.OwningGrid.CurrentGridSettings = setting;

                    //            if (this.OwningGrid.GridSettings == null)
                    //            {
                    //                this.OwningGrid.GridSettings = new List<GridSetting>();
                    //                var comboBoxItem = new ComboBoxItem() { Content = setting.Name };
                    //                if (setting.IsDefault)
                    //                {
                    //                    comboBoxItem.Content = Default_Prefix + setting.Name;
                    //                }
                    //                this.m_comboSettings.SelectedIndex = -1;
                    //                this.m_comboSettings.Items.Add(comboBoxItem);
                    //                this.m_comboSettings.SelectedItem = comboBoxItem;
                    //                this.OwningGrid.GridSettings.Add(setting);
                    //            }
                    //            else
                    //            {
                    //                this.m_comboSettings.Items.Clear();
                    //                if (isDefault)
                    //                {
                    //                    this.OwningGrid.GridSettings.ForEach(p =>
                    //                    {
                    //                        p.IsDefault = false;
                    //                    });
                    //                }
                    //                setting.IsDefault = isDefault;
                    //                var result = this.OwningGrid.GridSettings.FirstOrDefault(p => p.Name.ToLower() == setting.Name.ToLower());
                    //                if (result != null)
                    //                {
                    //                    var index = this.OwningGrid.GridSettings.IndexOf(result);
                    //                    this.OwningGrid.GridSettings[index] = setting;
                    //                }
                    //                else
                    //                {
                    //                    this.OwningGrid.GridSettings.Add(setting);
                    //                }
                    //                this.OwningGrid.GridSettings.ForEach(p =>
                    //                {
                    //                    var item = new ComboBoxItem() { Content = p.Name };
                    //                    if (p.IsDefault)
                    //                    {
                    //                        item.Content = Default_Prefix + p.Name;
                    //                    }
                    //                    this.m_comboSettings.Items.Add(item);
                    //                });
                    //                var selectedIndex = this.OwningGrid.GridSettings.IndexOf(setting);
                    //                this.m_comboSettings.SelectedIndex = selectedIndex;
                    //            }

                    //            this.OwningGrid.RefreshGridSetting();

                    //            this.m_btnSave.IsEnabled = true;
                    //            this.m_btnSave.Focus();
                    //            this.m_tbResult.Text = Resource.ColumnsV2_SaveSuccessfully;
                    //        });

                    //this.m_btnSave.IsEnabled = false;

                    #endregion
                }
            }
            catch (Exception ex)
            {
                this.m_tbResult.Text = ex.Message;
            }
        }

        void m_btnNew_Click(object sender, RoutedEventArgs e)
        {
            this.m_tbResult.Text = string.Empty;
            this.m_availableColumns.Clear();
            this.m_selectedColumns.Clear();
            GenerateLeftListBoxItem();
            this.m_txtName.Text = string.Empty;
            this.m_chkDefault.IsEnabled = true;
            this.m_btnSave.IsEnabled = true;
            this.m_txtName.IsEnabled = true;
        }

        void m_btnDelete_Click(object sender, RoutedEventArgs e)
        {
            this.m_tbResult.Text = string.Empty;
            var selectedItem = this.m_comboSettings.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                var setting = this.OwningGrid.GridSettings.FirstOrDefault(
                    p => p.Name.Trim().ToLower() == selectedItem.Content.ToString().Replace(Default_Prefix, "").Trim().ToLower());
                this.DeletingGridConfig = setting;
                this.OwningGrid.GridSettings.Remove(this.DeletingGridConfig);

                this.OwningGrid.m_userProfile.Set(GridKeys.KEY_UP_GRIDSETTINGS, this.OwningGrid.GridSettings,true);

                var name = this.DeletingGridConfig.Name;
                var item = this.m_comboSettings.Items.FirstOrDefault(q =>
                {
                    var comboxItem = q as ComboBoxItem;
                    return comboxItem.Content.ToString().Replace(Default_Prefix, "") == name;
                });

                this.m_comboSettings.Items.Remove(item);

                if (this.m_comboSettings.Items.Count > 0)
                {
                    this.m_needRefresh = true;
                    this.m_comboSettings.SelectedIndex = 0;
                    this.m_needRefresh = false;
                    this.m_txtName.Text = (this.m_comboSettings.Items[0] as ComboBoxItem).Content.ToString().Replace(Default_Prefix, "");
                    var currentConfig = this.OwningGrid.GridSettings.FirstOrDefault(q => q.Name == this.m_txtName.Text);
                    this.m_chkDefault.IsChecked = currentConfig.IsDefault;
                }
                else
                {
                    this.m_txtName.Text = string.Empty;
                    GenerateLeftListBoxItem();
                }

                this.DeletingGridConfig = null;

                this.OwningGrid.RefreshGridSetting();
                this.m_tbResult.Text = Resource.ColumnsV2_DeleteSuccessfully;

                #region old version

                //this.OwningGrid.m_userProfile.RemoveGridSetting(setting, p =>
                //{
                //    var name = this.DeletingGridConfig.Name;
                //    var item = this.m_comboSettings.Items.FirstOrDefault(q =>
                //    {
                //        var comboxItem = q as ComboBoxItem;
                //        return comboxItem.Content.ToString().Replace(Default_Prefix, "") == name;
                //    });

                //    this.m_comboSettings.Items.Remove(item);

                //    if (this.m_comboSettings.Items.Count > 0)
                //    {
                //        this.m_needRefresh = true;
                //        this.m_comboSettings.SelectedIndex = 0;
                //        this.m_needRefresh = false;
                //        this.m_txtName.Text = (this.m_comboSettings.Items[0] as ComboBoxItem).Content.ToString().Replace(Default_Prefix, "");
                //        var currentConfig = this.OwningGrid.GridSettings.FirstOrDefault(q => q.Name == this.m_txtName.Text);
                //        this.m_chkDefault.IsChecked = currentConfig.IsDefault;
                //    }
                //    else
                //    {
                //        this.m_txtName.Text = string.Empty;
                //        GenerateLeftListBoxItem();
                //    }

                //    this.OwningGrid.GridSettings.Remove(this.DeletingGridConfig);

                //    this.DeletingGridConfig = null;
                //    this.m_btnDelete.IsEnabled = true;

                //    this.OwningGrid.RefreshGridSetting();
                //    this.m_tbResult.Text = Resource.ColumnsV2_DeleteSuccessfully;
                //});

                //this.m_btnDelete.IsEnabled = false;

                #endregion
            }
        }

        void m_btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.m_selectedColumnsListBox.SelectedItem != null)
            {
                object obj = m_selectedColumnsListBox.SelectedItem;
                int index = m_selectedColumnsListBox.SelectedIndex;
                if (index > 0)
                {
                    m_selectedColumns.RemoveAt(index);
                    m_selectedColumns.Insert(index - 1, (ListBoxItemExt)obj);
                    this.m_selectedColumnsListBox.SelectedIndex = index - 1;
                }
            }
        }

        void m_btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.m_selectedColumnsListBox.SelectedItem != null)
            {
                object obj = m_selectedColumnsListBox.SelectedItem;
                int index = m_selectedColumnsListBox.SelectedIndex;
                if (index < m_selectedColumnsListBox.Items.Count - 1)
                {
                    m_selectedColumns.RemoveAt(index);
                    m_selectedColumns.Insert(index + 1, (ListBoxItemExt)obj);
                    this.m_selectedColumnsListBox.SelectedIndex = index + 1;
                }
            }
        }

        void m_txtWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.m_selectedColumnsListBox.SelectedItem != null)
            {
                double width = 0;
                if (double.TryParse(this.m_txtWidth.Text.Trim(), out width) && width <= MAXWIDTH && width > 0)
                {
                    var item = this.m_selectedColumnsListBox.SelectedItem as ListBoxItemExt;
                    item.ColWidth = width;

                    var name = item.Name.Contains(ListBoxItemNamePrefix) ? item.Name.Replace(ListBoxItemNamePrefix, "") : item.Name;
                    if (this.CurrentGridConfig != null)
                    {
                        var result = CurrentGridConfig.Columns.FirstOrDefault(p =>
                        {
                            var colName = p.Name.Contains(ListBoxItemNamePrefix) ? p.Name.Replace(ListBoxItemNamePrefix, "") : p.Name;
                            return colName.Trim().ToLower() == name.Trim().ToLower();
                        });
                        if (result != null)
                        {
                            result.Width = new DataGridLength(width);
                        }
                    }
                }
            }
        }

        void m_comboSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.RemovedItems.Count == 0 || e.AddedItems.Count == 0) && !m_needRefresh)
            {
                return;
            }

            this.m_availableColumns.Clear();
            this.m_selectedColumns.Clear();

            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null)
            {
                return;
            }
            var settingName = selectedItem.Content.ToString().Replace(Default_Prefix, "");
            this.m_txtName.Text = settingName;
            var setting = this.OwningGrid.GridSettings.FirstOrDefault(p => p.Name.ToLower() == settingName.ToLower());
            if (setting != null)
            {
                this.m_chkDefault.IsChecked = setting.IsDefault;
                this.CurrentGridConfig = setting;

                this.m_cmbPageSize.SelectedIndex = -1;
                foreach (var item in this.m_cmbPageSize.Items)
                {
                    if ((item as ComboBoxItem).Content.ToString() == setting.PageSize.ToString())
                    {
                        var index = this.m_cmbPageSize.Items.IndexOf((item as ComboBoxItem));
                        this.m_cmbPageSize.SelectedIndex = index;
                        break;
                    }
                }
                setting.Columns.ForEach(p =>
                {
                    var column = GetColumnByName(p.Name);

                    if (column != null)
                    {
                        if (p.IsHided)
                        {
                            var item = GenerateListBoxItemExt(ExtType.Add, p);
                            this.m_availableColumns.Add(item);
                        }
                        else
                        {
                            var item = GenerateListBoxItemExt(ExtType.Remove, p);
                            this.m_selectedColumns.Add(item);
                        }
                    }
                });
            }
        }

        void m_lstSelectedColumns_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.m_selectedColumnsListBox.SelectedItem != null)
            {
                var index = this.m_selectedColumnsListBox.SelectedIndex;
                if (this.m_selectedColumnsListBox.Items.Count == 1)
                {
                    this.m_btnUp.IsEnabled = false;
                    this.m_btnDown.IsEnabled = false;
                }
                else if (index == 0)
                {
                    this.m_btnUp.IsEnabled = false;
                    this.m_btnDown.IsEnabled = true;
                }
                else if (index == this.m_selectedColumnsListBox.Items.Count - 1)
                {
                    this.m_btnUp.IsEnabled = true;
                    this.m_btnDown.IsEnabled = false;
                }
                else
                {
                    this.m_btnUp.IsEnabled = true;
                    this.m_btnDown.IsEnabled = true;
                }
                var item = this.m_selectedColumnsListBox.SelectedItem as ListBoxItemExt;

                this.m_txtWidth.Text = item.ColWidth.ToString();
                this.m_txtWidth.SelectAll();
                this.m_txtWidth.Focus();
            }
            else
            {
                this.m_btnUp.IsEnabled = false;
                this.m_btnDown.IsEnabled = false;
            }
        }

        void item_RemoveItem(object sender, EventArgs e)
        {
            var item = sender as ListBoxItemExt;
            var availableSource = m_availableColumnsListBox.ItemsSource as ObservableCollection<ListBoxItemExt>;
            var selectedSource = m_selectedColumnsListBox.ItemsSource as ObservableCollection<ListBoxItemExt>;
            selectedSource.Remove(item);

            var newItem = new ListBoxItemExt(ExtType.Add);
            newItem.Content = item.Content;
            newItem.ColWidth = item.ColWidth;
            newItem.Name = item.Name;
            newItem.AddItem += new EventHandler(item_AddItem);
            availableSource.Add(newItem);

            m_lstSelectedColumns_SelectionChanged(null, null);
        }

        void item_AddItem(object sender, EventArgs e)
        {
            var item = sender as ListBoxItemExt;
            var availableSource = m_availableColumnsListBox.ItemsSource as ObservableCollection<ListBoxItemExt>;
            var selectedSource = m_selectedColumnsListBox.ItemsSource as ObservableCollection<ListBoxItemExt>;
            availableSource.Remove(item);

            var newItem = new ListBoxItemExt(ExtType.Remove);
            newItem.Name = item.Name;
            newItem.Content = item.Content;
            newItem.ColWidth = item.ColWidth;
            newItem.DataContext = newItem;
            newItem.RemoveItem += new EventHandler(item_RemoveItem);
            selectedSource.Add(newItem);

            m_lstSelectedColumns_SelectionChanged(null, null);
        }

        #endregion

        #region Methods

        internal void InitGridSettings()
        {
            if (this.OwningGrid.GridSettings != null && this.OwningGrid.GridSettings.Count > 0)
            {
                var config = this.OwningGrid.GridSettings.FirstOrDefault(p => p.Name == RESET_NAME);
                if (config != null)
                {
                    this.OwningGrid.GridSettings.Remove(config);
                }
                GridSetting currentSetting = null;
                this.m_comboSettings.Items.Clear();
                this.OwningGrid.GridSettings.ForEach(p =>
                {
                    ComboBoxItem item = new ComboBoxItem { Content = p.Name };

                    if (p.IsDefault)
                    {
                        item.IsSelected = true;
                        currentSetting = p;
                        item.Content = Default_Prefix + p.Name;
                        this.m_txtName.Text = p.Name;
                    }
                    this.m_comboSettings.Items.Add(item);
                });
                //如果数据库中没有Default的配置，则取第一个（此种情况会在Delete default config后出现）
                if (currentSetting == null)
                {
                    this.m_comboSettings.SelectedIndex = 0;
                    this.m_txtName.Text = this.OwningGrid.GridSettings[0].Name;
                    currentSetting = this.OwningGrid.GridSettings[0];
                }

                this.CurrentGridConfig = currentSetting;
                this.m_chkDefault.IsChecked = currentSetting.IsDefault;
                var re = this.m_cmbPageSize.Items.FirstOrDefault(p =>
                {
                    return (p as ComboBoxItem).Content.ToString() == currentSetting.PageSize.ToString();
                });
                var index = this.m_cmbPageSize.Items.IndexOf(re);
                this.m_cmbPageSize.SelectedIndex = index;

                this.m_availableColumns.Clear();
                this.m_selectedColumns.Clear();

                //Edit by Hax 20100203
                this.OwningGrid.m_resetGridConfig.Columns.ForEach(p =>
                {
                    var colConfig = this.CurrentGridConfig.Columns.FirstOrDefault(q => q.Name == p.Name);
                    if (colConfig != null)
                    {
                        if (!colConfig.IsHided)
                        {
                            var item = GenerateListBoxItemExt(ExtType.Remove, colConfig);
                            item.ColIndex = colConfig.Index;
                            this.m_selectedColumns.Add(item);
                        }
                        else
                        {
                            var item = GenerateListBoxItemExt(ExtType.Add, colConfig);
                            this.m_availableColumns.Add(item);
                        }
                    }
                    else
                    {
                        var item = new ListBoxItemExt(ExtType.Add);
                        item.AddItem += new EventHandler(item_AddItem);
                        item.Name = string.Format(this.m_listBoxItemName_Prefix, p.Name);

                        var column = GetColumnByName(p.Name);
                        if (column != null && column.Header != null)
                        {
                            item.ColWidth = column.ActualWidth;
                            item.Content = column.Header;
                            this.m_availableColumns.Add(item);
                        }
                    }
                });

                var list = m_selectedColumns.ToList();
                list.Sort((x, y) =>
                {
                    if (x.ColIndex > y.ColIndex)
                        return 1;
                    else if (x.ColIndex == y.ColIndex)
                        return 0;
                    else return -1;
                });

                m_selectedColumns.Clear();

                list.ForEach(p => m_selectedColumns.Add(p));

                /*
                 * Comment by Hax--20100203                  
                 * 为了避免以后开发人员增加或删除xaml中DataGrid的Column,造成新增加的Column在AvailableColumn中显示不出来 
                 * 修改为根据DataGrid的ResetColumns(开发时设置的Column)来产生AvailableColumn和SelectedItem，而不是根据CurrentGridConfig来生成                
                */

                //this.CurrentGridConfig.Columns.ForEach(p =>
                //{
                //    var column = GetColumnByName(p.Name);

                //    if (column != null)
                //    {
                //        if (!p.IsHided)
                //        {
                //            var item = GenerateListBoxItemExt(ExtType.Remove, p);
                //            this.m_selectedColumns.Add(item);
                //        }
                //        else
                //        {
                //            var item = GenerateListBoxItemExt(ExtType.Add, p);
                //            this.m_availableColumns.Add(item);
                //        }
                //    }
                //});                
            }
            else
            {
                GenerateLeftListBoxItem();
                //GenerateDefaultConfig();
            }

            this.m_availableColumnsListBox.ItemsSource = this.m_availableColumns;
            this.m_selectedColumnsListBox.ItemsSource = this.m_selectedColumns;
        }

        void GenerateLeftListBoxItem()
        {
            this.m_availableColumns.Clear();
            this.m_selectedColumns.Clear();

            foreach (var col in this.OwningGrid.Columns)
            {
                if (col.Header == null)
                {
                    continue;
                }
                var colName = col.GetColumnName();

                ListBoxItemExt item = new ListBoxItemExt(ExtType.Add);
                item.Name = string.Format(this.m_listBoxItemName_Prefix, colName);
                item.AddItem += new EventHandler(item_AddItem);
                item.ColWidth = col.ActualWidth;
                item.Content = col.Header;
                m_availableColumns.Add(item);
            }
        }

        //void GenerateDefaultConfig()
        //{
        //    this.m_availableColumns.Clear();
        //    this.m_selectedColumns.Clear();

        //    foreach (var col in this.OwningGrid.ColumnsInternal)
        //    {
        //        if (col.Header == null || col.Visibility== Visibility.Collapsed)
        //        {
        //            continue;
        //        }
        //        var colName = GetColumnName(col);

        //        ListBoxItemExt item = new ListBoxItemExt(ExtType.Remove);
        //        item.Name = string.Format(this.m_listBoxItemName_Prefix, colName);
        //        item.RemoveItem += new EventHandler(item_RemoveItem);
        //        item.ColWidth = col.ActualWidth;
        //        item.Content = col.Header;
        //        item.DataContext = item;
        //        m_selectedColumns.Add(item);
        //    }
        //    this.m_chkDefault.IsChecked = true;
        //    this.m_chkDefault.IsEnabled = false;
        //    this.m_btnSave.IsEnabled = false;
        //    this.m_txtName.IsEnabled = false;
        //}        

        DataGridColumn GetColumnByName(string name)
        {
            var column = this.OwningGrid.Columns.FirstOrDefault(col =>
            {
                var colName = col.GetColumnName();

                return colName.Trim().ToLower() == name.Replace(ListBoxItemNamePrefix, "").Trim().ToLower();
            });

            return column;
        }

        ListBoxItemExt GenerateListBoxItemExt(ExtType type, GridColumn colConfig)
        {
            ListBoxItemExt item = null;
            switch (type)
            {
                case ExtType.Add:
                    item = new ListBoxItemExt(ExtType.Add);
                    break;
                case ExtType.Remove:
                    item = new ListBoxItemExt(ExtType.Remove);
                    break;
            }
            var column = GetColumnByName(colConfig.Name);
            item.Content = column != null ? column.Header : null;
            if (!colConfig.IsHided)
            {
                item.RemoveItem += new EventHandler(item_RemoveItem);
                item.DataContext = item;
                item.ColWidth = colConfig.Width.Value;
            }
            else
            {
                item.AddItem += new EventHandler(item_AddItem);
                item.ColWidth = column.ActualWidth;
            }
            item.Name = string.Format(this.m_listBoxItemName_Prefix, colConfig.Name);
            return item;
        }

        GridSetting GetConfig()
        {
            var config = new GridSetting();
            config.GridGuid = this.OwningGrid.GridID;
            config.Name = this.m_txtName.Text.Trim();
            config.PageSize = int.Parse((this.m_cmbPageSize.SelectedItem as ComboBoxItem).Content.ToString());
            if (string.IsNullOrEmpty(config.Name))
            {
                throw new ArgumentException(Resource.Name_Required);
            }
            config.IsDefault = this.m_chkDefault.IsChecked.Value;
            config.Columns = new List<GridColumn>();
            if (this.m_selectedColumnsListBox.Items.Count > 0)
            {
                foreach (var item in this.m_selectedColumnsListBox.Items)
                {
                    var listBoxItem = item as ListBoxItemExt;
                    GridColumn col = new GridColumn()
                    {
                        Index = this.m_selectedColumnsListBox.Items.IndexOf(listBoxItem),
                        IsHided = false,
                        Name = listBoxItem.Name.Replace(ListBoxItemNamePrefix, ""),
                        Width = new DataGridLength(listBoxItem.ColWidth)
                    };
                    config.Columns.Add(col);
                }
            }
            else
            {
                throw new ArgumentException(Resource.ColumnsV2_AtLeastSelectOneColumn); ;
            }
            if (this.m_availableColumnsListBox.Items.Count > 0)
            {
                foreach (var item in this.m_availableColumnsListBox.Items)
                {
                    var listBoxItem = item as ListBoxItemExt;
                    GridColumn col = new GridColumn()
                    {
                        Index = 0,//不显示,没有意义
                        IsHided = true,
                        Name = listBoxItem.Name.Replace(ListBoxItemNamePrefix, ""),
                        Width = new DataGridLength(listBoxItem.ColWidth)
                    };
                    config.Columns.Add(col);
                }
            }
            return config;
        }

        #endregion
    }
}