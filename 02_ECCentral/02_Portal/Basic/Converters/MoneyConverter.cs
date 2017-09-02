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

namespace ECCentral.Portal.Basic.Converters
{
    public class MoneyConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal money = 0.00M;

            if (value != null)
            {
                if (decimal.TryParse(value.ToString(), out money))
                {
                    return decimal.Round(money, 2).ToString("0.00");
                }
                return value.ToString();
            }
            return money.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion IValueConverter Members

        #region Extend Interface

        public static string ConvertToString(decimal? money)
        {
            string result = string.Empty;
            if (money.HasValue)
            {
                result = money.Value.ToString("￥0.00");
            }
            return result;
        }

        #endregion
    }

    public class NullableMoneyConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal money;

            if (value != null)
            {
                if (decimal.TryParse(value.ToString(), out money))
                {
                    return decimal.Round(money, 2).ToString();
                }
                return value.ToString();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion IValueConverter Members
    }

    public class NullablePercentageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal money;

            if (value != null)
            {
                if (decimal.TryParse(value.ToString(), out money))
                {
                    return (decimal.Round(money * 100, 2)).ToString() + "%";
                }
                return value.ToString();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion IValueConverter Members
    }

    public class CurrencyMoneyConverter : IValueConverter
    {
        private const string Currency_Format = "#####0.00";

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal money = 0.00M;

            if (value != null)
            {
                if (decimal.TryParse(value.ToString(), out money))
                {
                    return "￥" + decimal.Round(money, 2).ToString(Currency_Format);
                }
                return value.ToString();
            }
            return money.ToString(Currency_Format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal money = 0.00M;
            if (value != null)
            {
                string currencyMoney = value.ToString();
                decimal.TryParse(currencyMoney.Trim('￥'), out money);
            }
            return money;
        }

        #endregion IValueConverter Members
    }

    public class  NullCurrencyMoneyConverter : IValueConverter
    {
        private const string Currency_Format = "#####0.00";

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //value为null时返回￥0.00;
            decimal money = 0.00M;
            if (value != null)
            {
                if (decimal.TryParse(value.ToString(), out money))
                {
                   
                }
            }
            return "￥"+ decimal.Round(money, 2).ToString(Currency_Format);;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal money = 0.00M;
            if (value != null)
            {
                string currencyMoney = value.ToString();
                decimal.TryParse(currencyMoney.Trim('￥'), out money);
            }
            return money;
        }

        #endregion IValueConverter Members
    }

    public class NullableCurrencyMoneyConverter : IValueConverter
    {
        private const string Currency_Format = "#####0.00";

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal money;

            if (value != null)
            {
                if (decimal.TryParse(value.ToString(), out money))
                {
                    return "￥" + decimal.Round(money, 2).ToString(Currency_Format);
                }
                return value.ToString();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal money;
            if (value != null)
            {
                string currencyMoney = value.ToString();
                if (decimal.TryParse(currencyMoney.Trim('￥'), out money))
                {
                    return money;
                }
                return null;
            }
            return null;
        }

        #endregion IValueConverter Members
    }

    public class CurrencySpetorMoneyConverter : IValueConverter
    {
        private const string format = "###,###,###0.00";

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal money;
            if (value != null)
            {
                if (decimal.TryParse(value.ToString(),out money))
                {
                    return money.ToString(format);
                }
                return value.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal money;
            if (value != null)
            {
                string currencyMoney = value.ToString();
                if (decimal.TryParse(currencyMoney.Replace(",", ""), out money))
                {
                    return money;
                }
                return null;
            }
            return null;
        }

        #endregion
    }
}
