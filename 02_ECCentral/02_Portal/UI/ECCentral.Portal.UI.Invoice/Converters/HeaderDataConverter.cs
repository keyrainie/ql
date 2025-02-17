﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace ECCentral.Portal.UI.Invoice.Converters
{
    public class HeaderDataConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as dynamic;
            string para = parameter.ToString();
            switch (para)
            {
                case "hlbtnViewDetials":
                    {
                        if (data != null
                            && (data.XCount != 0 || data.ECount != 0))
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    }
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
