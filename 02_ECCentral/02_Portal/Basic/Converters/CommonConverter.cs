using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using System.Collections.Generic;

namespace ECCentral.Portal.Basic.Converters
{
    /// <summary>
    /// true -->Visibility.Visible
    /// false-->Visibility.Collapsed;
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool flag = (bool)value;
            if (parameter != null && parameter.Equals("!"))
            {
                if (parameter.ToString() == "test")
                {
                    flag = !flag;
                }
                flag = !flag;
            }
            
            if (flag)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }

    /// <summary>
    /// true -->Visibility.Collapsed
    /// false-->Visibility.Visible;
    /// </summary>
    public class ReverseBoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool flag = (bool)value;
            if (flag)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }

    /// <summary>
    /// 把true转变为false,false变为true
    /// </summary>
    public class ReverseBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool flag = (bool)value;
            return !flag;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }


    /// <summary>
    /// 如果是数字类型，则非零为真，零为假；
    /// 如果是字符类型，则非空字符串为真，否则为假；
    /// 如果是Boolen类型，则true为真，false为假；
    /// 其他对象类型，非Null为真，Null为假；
    /// </summary>
    public class ObjectToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                return ((int)value) != 0;
            }
            else if (value is decimal)
            {
                return ((decimal)value) != 0;
            }
            else if (value is double)
            {
                return ((double)value) != 0;
            }
            else if (value is float)
            {
                return ((float)value) != 0;
            }
            else if (value is short)
            {
                return ((short)value) != 0;
            }
            else if (value is long)
            {
                return ((long)value) != 0;
            }
            else if (value is string)
            {
                return value.ToString().Length > 0;
            }
            else if (value is bool)
            {
                return (bool)value;
            }
            else
            {
                return value != null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isTrue = (bool)value;
            return isTrue;
        }

        #endregion IValueConverter Members
    }

    /// <summary>
    /// 如果是数字类型，则1为真，0为假；
    /// </summary>
    public class IntToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                return ((int)value) != 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isTrue = (bool)value;
            if (isTrue)
            {
                return 1;
            }
            return 0;
        }

        #endregion IValueConverter Members
    }

    public class IntToVisibilityValueConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                return (((int)value) != 0) ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isTrue = (bool)value;
            if (isTrue)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
    }


    /// <summary>
    /// 截断文中转换，避免表格文字过长
    /// </summary>
    public class SubStringConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = string.Empty;
            if (value != null && parameter != null)
            { 
                int maxLength = int.Parse(parameter.ToString());
                if (value.ToString().Length > maxLength)
                {
                    result = value.ToString().Substring(0, maxLength) + "...";
                }
                else
                {
                    result = value.ToString();
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// 保留N位小数
    /// </summary>
    public class SubNumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value==null) return null;
            string result = value.ToString();
            if (value != null && parameter != null)
            {
                int maxLength = int.Parse(parameter.ToString());
                result = Math.Round(decimal.Parse(value.ToString()), maxLength).ToString();
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 人民币Convert
    /// </summary>
    public class RMBConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "";

            if (value is decimal)
            {
                return "￥" + decimal.Parse(value.ToString()).ToString("0.00");
            }

            if (value is int)
            {
                return "￥" + int.Parse(value.ToString()).ToString();
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}