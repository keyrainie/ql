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

namespace Newegg.Oversea.Silverlight.Controls.Data
{
    public class DataGridTemplateColumn : System.Windows.Controls.DataGridTemplateColumn
    {
        private bool m_NeedExport = true;

        public bool NeedExport
        {
            get { return this.m_NeedExport; }
            set { this.m_NeedExport = value; }
        }

        public string SortField { get; set; }

        public string FilterField { get; set; }


        /// <summary>
        /// 指定在导出当前页的时候导出的属性名
        /// </summary>
        public string ExportField { get; set; }

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
    DependencyProperty.Register("IsFixed", typeof(bool), typeof(DataGridTemplateColumn), new PropertyMetadata(false));  

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(DataGridTemplateColumn), new PropertyMetadata(null));        
    }
}
