using System;
using System.Globalization;
using System.Windows.Data;
using ECCentral.Portal.Basic.Utilities;
using System.Collections.Generic;

namespace ECCentral.Portal.Basic.Converters
{
    /// <summary>
    /// 将格式为IPPSystemAdmin\bitkoo\IPPSystemAdmin[8601]的转为简单标示IPPSystemAdmin
    /// </summary>
    public class UserNameConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = string.Empty;
            if (value != null && value.ToString().Length > 0)
            {
                result = value.ToString().Split('\\')[0];
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
