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
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace Newegg.Oversea.Silverlight.Controls.Primitives
{
    public class SwitchGridConfig : ContentControl
    {
        public event EventHandler SwitchConfigCompleted;

        #region Data

        private FrameworkElement _elementRoot;      
        private ListBox _gridSettingsListBox;
        private HyperlinkButton _configButton;       
        private ObservableCollection<ListBoxItem> m_settingsCollection = new ObservableCollection<ListBoxItem>();
        private GridSetting m_resetGridConfig;

        #endregion

        #region Constants

        private const string ColumnName_Prefix = "DataGrid_ListBoxItem_";
        private const string RootElementName = "RootElement";       
        private const string ElementGridSettingsListBoxName = "GridSettingsList";     
        private const string ElementConfigButtonName = "Config";
        private const string RESET_NAME = "[Reset]";
        private const string Default_Prefix = "[D]";        

        #endregion
     
        internal Data.DataGrid OwningGrid { get; set; }

        internal ColumnsSelectorV3 OwningSelector { get; set; }
             
        public SwitchGridConfig()
        {
            DefaultStyleKey = typeof(SwitchGridConfig);
        }
       
        public override void OnApplyTemplate()
        {
            this._elementRoot = GetTemplateChild(RootElementName) as FrameworkElement;            
            this._gridSettingsListBox = GetTemplateChild(ElementGridSettingsListBoxName) as ListBox;
                      
            this._configButton = GetTemplateChild(ElementConfigButtonName) as HyperlinkButton;
            if (this._configButton != null)
            {
                _configButton.Content = Resource.ColumnsV2_ColumnOptions;
                this._configButton.Click += new RoutedEventHandler(_configButton_Click);
            }
            
            base.OnApplyTemplate();

            this._gridSettingsListBox.DisplayMemberPath = "Name";
            this._gridSettingsListBox.SelectionChanged += new SelectionChangedEventHandler(_gridSettingsListBox_SelectionChanged);
            this._gridSettingsListBox.ItemsSource = m_settingsCollection;

            this.m_resetGridConfig = this.OwningGrid.m_resetGridConfig;

            RefreshGridConfig(false);            
        }        

        /// <summary>
        /// Refresh Grid Config
        /// </summary>
        /// <param name="flag">是否需要设置Column的状态</param>
        void RefreshGridConfig(bool flag)
        {
            GridSetting currentConfig = null;

            m_settingsCollection.Clear();
            //增加初始设置
            var resetItem = new ListBoxItem { Content = Resource.ColumnsV3_Reset, Name = RESET_NAME };
            m_settingsCollection.Insert(0, resetItem);

            if (this.OwningGrid.GridSettings != null)
            {
                var reset = this.OwningGrid.GridSettings.FirstOrDefault(p => p.Name == RESET_NAME);
                if (reset != null)
                {
                    this.OwningGrid.GridSettings.Remove(reset);
                }

                this.OwningGrid.GridSettings.ForEach(p =>
                {
                    var item = new ListBoxItem { Content = p.Name };
                    if (p.IsDefault)
                    {
                        item.Content = string.Format("{0}{1}", Default_Prefix, p.Name);
                    }
                    if (p.Name != RESET_NAME)
                    {
                        m_settingsCollection.Insert(1, item);
                    }
                });
            }

            if (this.OwningGrid.GridSettings != null && this.OwningGrid.GridSettings.Count > 0)
            {
                var defaultConfig = this.OwningGrid.GridSettings.FirstOrDefault(p => p.IsDefault);
                if (defaultConfig == null)
                {
                    currentConfig = this.OwningGrid.GridSettings[0];
                }
                else
                {
                    currentConfig = defaultConfig;
                }

                foreach (var item in this._gridSettingsListBox.Items)
                {
                    if ((item as ListBoxItem).Content.ToString().Replace(Default_Prefix, "") == currentConfig.Name)
                    {
                        var index = m_settingsCollection.IndexOf((item as ListBoxItem));
                        this._gridSettingsListBox.SelectedIndex = index;
                        break;
                    }
                }

                if (flag)
                    this.OwningSelector.ApplyConfig(currentConfig);
            }
            else
            {
                if (flag)
                    this.OwningSelector.ApplyConfig(m_resetGridConfig);
                this._gridSettingsListBox.SelectedIndex = 0;
            }
        }

        internal void Refresh()
        {
            RefreshGridConfig(true);
        }
        
        void _gridSettingsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._gridSettingsListBox.SelectedItem != null && e.RemovedItems.Count > 0 && e.AddedItems.Count > 0)
            {
                var item = this._gridSettingsListBox.SelectedItem as ListBoxItem;
                var config = this.OwningGrid.GridSettings.FirstOrDefault(p => p.Name == item.Content.ToString().Replace(Default_Prefix, ""));
                if (item.Name == RESET_NAME)
                {
                    config = this.m_resetGridConfig;
                }                
                //config.IsDefault = true;
               
                this.OwningGrid.PageSize = config.PageSize;

                this.OwningSelector.ApplyConfig(config);

                //if (this.OwningGrid.GridSettings != null)
                //{
                //    this.OwningGrid.GridSettings.ForEach(p =>
                //    {
                //        if (p.Name.Trim().ToLower() == config.Name.Trim().ToLower())
                //        {
                //            p.IsDefault = true;
                //        }
                //        else
                //        {
                //            p.IsDefault = false;
                //        }
                //    });
                //}

                //RefreshGridSettings();

                //this.OwningGrid.OnSaveGridSettings(config);
                if (this.SwitchConfigCompleted != null)
                {
                    SwitchConfigCompleted(this, null);
                }
            }           
        }

        void _configButton_Click(object sender, RoutedEventArgs e)
        {
            ChildWindow window = new ChildWindow();

            window.FontSize = 12;
            window.Title = Resource.ColumnsV2_Title;
            var colselector = new CustomizeColumns();
            colselector.Container = window;
            colselector.OwningGrid = this.OwningGrid;
            window.Content = colselector;
            window.Show();
        }                      
    }
}