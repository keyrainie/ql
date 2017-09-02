using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using Newegg.Oversea.Silverlight.Controls.Primitives;

namespace Newegg.Oversea.Silverlight.Controls.Data
{
    public class DataGridTextColumn : System.Windows.Controls.DataGridTextColumn
    {
        private bool m_NeedExport = true;

        public bool NeedExport
        {
            get { return this.m_NeedExport; }
            set { this.m_NeedExport = value; }
        }

        public string SortField { get; set; }

        public string FilterField { get; set; }

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public bool IsFixed
        {
            get { return (bool)GetValue(IsFixedProperty); }
            set { SetValue(IsFixedProperty, value); }
        }

        public static readonly DependencyProperty IsFixedProperty =
            DependencyProperty.Register("IsFixed", typeof(bool), typeof(DataGridTextColumn), new PropertyMetadata(false));

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(DataGridTextColumn), new PropertyMetadata(null));    

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            PlatTextBox block = new PlatTextBox();
            block.Margin = new Thickness(5, 0, 5, 0);
            block.VerticalAlignment = VerticalAlignment.Center;

            //如果绑定的字段内容为空，则不显示ToolTip
            var toolTip = new ToolTip();
            toolTip.SetBinding(ToolTip.ContentProperty, this.Binding);
            if (this.Binding != null && this.Binding.Path != null)
            {
                var path = this.Binding.Path.Path;
                var prop = dataItem.GetType().GetProperty(path);
                if (prop != null)
                {
                    var value = prop.GetValue(dataItem, null);
                    if (value != null && value.ToString().Trim().Length > 0)
                    {
                        ToolTipService.SetToolTip(block, toolTip);
                    }
                }
            }


            if (DependencyProperty.UnsetValue != base.ReadLocalValue(FontFamilyProperty))
            {
                block.FontFamily = this.FontFamily;
            }
            if (!double.IsNaN(this.FontSize))
            {
                block.FontSize = this.FontSize;
            }
            if (this.FontStyle != null)
            {
                block.FontStyle = this.FontStyle;
            }
            if (this.FontWeight != null)
            {
                block.FontWeight = this.FontWeight;
            }
            if (this.Foreground != null)
            {
                block.Foreground = this.Foreground;
            }
            if ((this.Binding != null) || !DesignerProperties.IsInDesignTool)
            {
                block.SetBinding(TextBox.TextProperty, this.Binding);
            }
            return block;
        }        
    }
}
