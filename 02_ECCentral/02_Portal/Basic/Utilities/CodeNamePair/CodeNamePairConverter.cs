using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;

namespace ECCentral.Portal.Basic.Utilities
{
    public class CodeNamePairConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                List<CodeNamePair> list = (List<CodeNamePair>)parameter;
                return list.Where(p => p.Code == value.ToString()).FirstOrDefault().Name;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                List<CodeNamePair> list = (List<CodeNamePair>)parameter;
                return list.Where(p => p.Name == value.ToString()).FirstOrDefault().Code;
            }
            return string.Empty;
        }

        #endregion IValueConverter Members
    }
}
