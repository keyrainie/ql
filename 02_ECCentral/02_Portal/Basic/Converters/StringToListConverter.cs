using System;
using System.Globalization;
using System.Windows.Data;
using ECCentral.Portal.Basic.Utilities;
using System.Collections.Generic;

namespace ECCentral.Portal.Basic.Converters
{
    /// <summary>
    /// 将拼接的字符串转为集合，主要用于ItemsControl绑定, 字符串默认已逗号分隔
    /// </summary>
    public class StringToListConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                char splitChar = ',';

                if (parameter != null)
                {
                    switch (parameter.ToString().ToUpper())
                    {
                        case "POINT":
                            splitChar = '.';
                            break;
                        default:
                            break;
                    }
                }

                string stringArrs = value.ToString().Trim();
                if (stringArrs.IndexOf(splitChar) == 0)
                {
                    stringArrs = stringArrs.Substring(1);
                }
                if (stringArrs.IndexOf(splitChar) == stringArrs.Length)
                {
                    stringArrs = stringArrs.Substring(0, stringArrs.Length - 1);
                }
                return stringArrs.Split(splitChar);
            }
            return new string[] { };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
