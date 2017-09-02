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
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace Newegg.Oversea.Silverlight.Controls.Data
{
    public class DataGridAttached : DependencyObject
    {
        #region DependencyProperty

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.RegisterAttached("DataGridrAttached_HeaderProperty", typeof(object), typeof(DataGridAttached), new PropertyMetadata(null, HeaderPropertyChangedCallback));

        public static void SetHeader(DependencyObject obj, object value)
        {
            obj.SetValue(HeaderProperty, value);
        }

        public static object GetHeader(DependencyObject obj)
        {
            return obj.GetValue(HeaderProperty);
        }



        #endregion

        #region Private Event

        private static void HeaderPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridColumn column = d as DataGridColumn;

            if (column != null)
            {
                column.Header = e.NewValue == null ? string.Empty : e.NewValue.ToString();
            }
        }
        #endregion


        public static readonly DependencyProperty ScrollerVisibleProperty =
         DependencyProperty.RegisterAttached("DataGridrAttached_ScrollerVisibleProperty", 
         typeof(object), typeof(DataGridAttached), 
         new PropertyMetadata(null, ScrollerVisiblePropertyChangedCallback));

        public static void SetScrollerVisible(DependencyObject obj, object value)
        {
            obj.SetValue(ScrollerVisibleProperty, value);
        }

        public static object GetScrollerVisible(DependencyObject obj)
        {
            return obj.GetValue(ScrollerVisibleProperty);
        }

        private static void ScrollerVisiblePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var dataGrid = sender as DataGrid;
            if (args.NewValue != null && args.NewValue != args.OldValue)
            {
                if (((Visibility)args.NewValue) == Visibility.Collapsed)
                {
                    dataGrid.m_topRightScollViewerBorder.BorderThickness = new Thickness(1, 0, 0, 0);
                }
                else
                {
                    dataGrid.m_topRightScollViewerBorder.BorderThickness = new Thickness(1, 1, 1, 1);
                }
            }
        }
    }
}