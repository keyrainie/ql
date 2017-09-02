using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace Newegg.Oversea.Silverlight.Controls.Primitives
{          
    [TemplatePart(Name = ApplyColumnConfig.ElementApplyButtonName, Type = typeof(HyperlinkButton))]
    public class ApplyColumnConfig : ContentControl
    {
        public event EventHandler ApplyCompleted;
                  
        private FrameworkElement _elementRoot;                    
        private ListBox _columnsListBox;
        private HyperlinkButton _selectAll;
        private HyperlinkButton _unSelectAll;       
        private HyperlinkButton _applyButton;
           
        private const string RootElementName = "RootElement";            
        private const string ElementColumnsListBoxName = "ColumnsList";                    
        private const string ElementApplyButtonName = "Apply";
        private const string ElementSelectAllName = "SelectAll";
        private const string ElementUnSelectAllName = "UnSelectAll";
        private const string CheckBoxName_Prefix = "DataGrid_CheckBox_";
                             
        internal Data.DataGrid OwningGrid { get; set; }

        internal ColumnsSelectorV3 OwningSelector { get; set; }
                           
        public ApplyColumnConfig()
        {
            DefaultStyleKey = typeof(ApplyColumnConfig);            
        }

        public override void OnApplyTemplate()
        {
            this._elementRoot = GetTemplateChild(RootElementName) as FrameworkElement;
            this._selectAll = GetTemplateChild(ElementSelectAllName) as HyperlinkButton;
            this._selectAll.Content = Resource.ColumnsV3_SelectAll;
            this._selectAll.Click += new RoutedEventHandler(_selectAll_Click);

            this._unSelectAll = GetTemplateChild(ElementUnSelectAllName) as HyperlinkButton;
            this._unSelectAll.Content = Resource.ColumnsV3_UnSelectAll;
            this._unSelectAll.Click += new RoutedEventHandler(_unSelectAll_Click);

            this._columnsListBox = GetTemplateChild(ElementColumnsListBoxName) as ListBox;
            this._applyButton = GetTemplateChild(ElementApplyButtonName) as HyperlinkButton;
            if (this._applyButton != null)
            {
                _applyButton.Content = Resource.Apply;
                this._applyButton.Click += new RoutedEventHandler(_applyButton_Click);
            }

            GenerateCheckBox();

            base.OnApplyTemplate();
        }

        void _unSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this._columnsListBox.Items)
            {
                var chk = item as CheckBox;
                if (chk != null)
                {
                    chk.IsChecked = false;
                }
            }
        }

        void _selectAll_Click(object sender, RoutedEventArgs e)
        {            
            foreach (var item in this._columnsListBox.Items)
            {
                var chk = item as CheckBox;
                if (chk != null)
                {
                    chk.IsChecked = true;
                }
            }
        }             
        
        void _applyButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _columnsListBox.Items)
            {
                var chk = item as CheckBox;
                var col = this.OwningGrid.Columns.FirstOrDefault(p =>
                {
                    var name = "";
                    if (!string.IsNullOrEmpty(p.GetColumnName()))
                    {
                        name = p.GetColumnName();
                    }
                    else if (!string.IsNullOrEmpty(p.GetBindingPath()))
                    {
                        name = p.GetBindingPath();
                    }
                    return name == chk.Name.Replace(CheckBoxName_Prefix, "");
                });
                if (col != null)
                {
                    col.Visibility = chk.IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            if (this.ApplyCompleted != null)
            {
                this.ApplyCompleted(this, null);
            }
        }

        internal void Refresh()
        {
            GenerateCheckBox();
        }

        void GenerateCheckBox()
        {
            if (_columnsListBox != null)
            {
                this._columnsListBox.Items.Clear();
                if (this.OwningGrid != null && this.OwningGrid.Columns.Count > 0)
                {
                    foreach (var column in this.OwningGrid.Columns)
                    {
                        if (column.Header == null)
                        {
                            continue;
                        }
                        CheckBox chk = new CheckBox();
                        if (!string.IsNullOrEmpty(column.GetColumnName()))
                        {
                            chk.Name = column.GetColumnName();
                        }
                        else if (!string.IsNullOrEmpty(column.GetBindingPath()))
                        {
                            chk.Name = column.GetBindingPath();
                        }
                        else
                        {
                            throw new ArgumentException("Column name and binding paths can't be both empty.");
                        }
                        chk.Name = CheckBoxName_Prefix + chk.Name;
                        chk.Content = column.Header;

                        chk.IsChecked = column.Visibility == Visibility.Visible ? true : false;

                        var flag = this._columnsListBox.Items.FirstOrDefault(p =>
                        {
                            var item = p as CheckBox;
                            return item.Name == chk.Name;
                        });
                        if (flag != null)
                        {
                            throw new ArgumentException(string.Format("Column name or binding path [{0}] must be unique in datagrid.", chk.Name));
                        }
                        this._columnsListBox.Items.Add(chk);
                    }
                }
            }                      
        }                                    
    }   
}