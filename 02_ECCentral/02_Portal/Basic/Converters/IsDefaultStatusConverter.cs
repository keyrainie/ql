using System;
using System.Globalization;
using System.Windows.Data;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.Basic.Converters
{

    /// <summary>
    /// 如果是数字类型，则1为真，0为假；
    /// </summary>
    public class IsDefaultStatusConverter : IValueConverter
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
                return IsDefault.DeActive;
            }
            return IsDefault.Active;
        }

        #endregion IValueConverter Members
    }

    public class IsShowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int valueDesc;
            if(int.TryParse(System.Convert.ToString(value),out valueDesc))
            {
                if(valueDesc==1)
                {
                    return "是";
                }
                return "否";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PriceAdjustedBySupplyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueDesc = System.Convert.ToString(value);
            if (!string.IsNullOrEmpty(valueDesc) &&
                   valueDesc.Trim().ToLower() == "true")
            {
                return "是";
            }
            return "否";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SetStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueDesc = System.Convert.ToString(value);
            if (valueDesc.ToLower() == "deactive")
            {
                return "无效";
            }
            return "有效";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}