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
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.ComponentModel;

namespace Newegg.Oversea.Silverlight.Utilities
{
    public class RestrictDatePickerAttached
    {
        #region DependencyProperty

        public static readonly DependencyProperty MaxDateProperty = DependencyProperty.RegisterAttached("MaxDate", typeof(string), typeof(RestrictDatePickerAttached), new PropertyMetadata("6/6/2079"));

        public static void SetMaxDate(DependencyObject obj, string value)
        {
            obj.SetValue(MaxDateProperty, value);
        }

        public static string GetMaxDate(DependencyObject obj)
        {
            return (string)obj.GetValue(MaxDateProperty);
        }

        public static readonly DependencyProperty MinDateProperty = DependencyProperty.RegisterAttached("MinDate", typeof(string), typeof(RestrictDatePickerAttached), new PropertyMetadata("1/1/1900"));

        public static void SetMinDate(DependencyObject obj, string value)
        {
            obj.SetValue(MinDateProperty, value);
        }

        public static string GetMinDate(DependencyObject obj)
        {
            return (string)obj.GetValue(MinDateProperty);
        }

        #endregion
    }
}
