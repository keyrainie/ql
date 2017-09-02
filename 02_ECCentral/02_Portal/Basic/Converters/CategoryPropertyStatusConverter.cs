using System;
using System.Windows.Data;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.Basic.Converters
{

    /// <summary>
    /// 如果是数字类型，则1为真，0为假；
    /// </summary>
    public class CategoryPropertyStatusToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var tempType = value.GetType();
            if (tempType.IsEnum)
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
                return CategoryPropertyStatus.Yes;
            }
            return CategoryPropertyStatus.No;
        }

        #endregion IValueConverter Members
    }
}