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
using System.Windows.Data;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Portal.UI.ExternalSYS.Converters
{
    public class CpsPublicConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter.ToString() == "Available") //启用和禁用Converter
            {
                if (value == null)
                {
                    return "启用";
                }
                IsActive eum = (IsActive)value;
                if (eum == IsActive.Active)
                {
                    return "禁用";
                }
                else
                {
                    return "启用";
                }
            }
            if (parameter.ToString() == "AuditClearance") //审核通过按钮的Convert
            {
                if (value == null)
                {
                    return Visibility.Collapsed;
                }
                AuditStatus status = (AuditStatus)value;
                if (status == AuditStatus.AuditClearance)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }

            }
            if (parameter.ToString() == "AuditNoClearance") //审核未通过按钮的Convert
            {
                if (value == null)
                {
                    return Visibility.Visible;
                }
                AuditStatus status = (AuditStatus)value;
                if (status == AuditStatus.AuditNoClearance)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            if (parameter.ToString() == "AuditReady") //待审核按钮的Convert
            {
                if (value == null)
                {
                    return Visibility.Collapsed;
                }
                AuditStatus status = (AuditStatus)value;
                if (status == AuditStatus.AuditReady)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }

            if (parameter.ToString() == "Action")
            {
                if (value == null)
                {
                    return Visibility.Collapsed;
                }
                AuditStatus status = (AuditStatus)value;
                if (status == AuditStatus.AuditNoClearance || status == AuditStatus.AuditReady)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }

            if (parameter.ToString() == "CommisionAudit")
            {
                if (value == null)
                {
                    return Visibility.Collapsed;
                }
                ToCashStatus status = (ToCashStatus)value;
                if (status == ToCashStatus.Requested)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            if (parameter.ToString() == "CommisionEdit")
            {
                if (value == null)
                {
                    return Visibility.Collapsed;
                }
                ToCashStatus status = (ToCashStatus)value;
                if (status == ToCashStatus.Confirmed)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            if (parameter.ToString() == "CommisionConfirm")
            {
                if (value == null)
                {
                    return Visibility.Collapsed;
                }
                ToCashStatus status = (ToCashStatus)value;
                if (status == ToCashStatus.Confirmed)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            if (parameter.ToString() == "AdvertisingAction")
            {
                if (value == null)
                {
                    return Visibility.Collapsed;
                }
                ValidStatus status = (ValidStatus)value;
                if (status == ValidStatus.Active)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();

        }
    }
    public class CpsAfterTaxAmt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }
            decimal originalAmt = (decimal)value;
            decimal afterTaxAmt = 0.00m;
            decimal taxAmt = 0.00m;         //税费
            decimal taxableIncome = 0.00m;  //应纳税所得额


            //计算应纳税所得额
            if (originalAmt <= 800)
            {
                taxableIncome = 0;
            }
            else if (originalAmt > 800 && originalAmt <= 4000)
            {
                taxableIncome = originalAmt - 800;
            }
            else if (originalAmt > 4000)
            {
                taxableIncome = originalAmt * 0.8m;
            }

            //计算个税
            if (taxableIncome <= 20000)
            {
                taxAmt = taxableIncome * 0.2m;
            }
            else if (taxableIncome > 20000 && taxableIncome <= 50000)
            {
                taxAmt = taxableIncome * 0.3m - 2000m;
            }
            else if (taxableIncome > 50000)
            {
                taxAmt = taxableIncome * 0.4m - 7000m;
            }

            afterTaxAmt = originalAmt - taxAmt;

            return Decimal.Round(afterTaxAmt, 2);

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class CpsMonthConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string month = (string)value;

                if (month.Length > 0) return month.Substring(0, month.Length - 1);//去掉最后一位，
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
