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
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.UI.Invoice.Models;

namespace ECCentral.Portal.UI.Invoice.Converters
{
    /// <summary>
    /// 对单条记录操作的CheckBox可用性Converter
    /// </summary>
    public class POSPayConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as POSPayVM;
            if (data != null)
            {
                var par = parameter.ToString();
                switch (par)
                {
                    case "CheckBox":
                        return (data.AutoConfirmStatus == AutoConfirmStatus.Fault && data.SOIncomeStatus == SOIncomeStatus.Origin);

                    case "CombineNumber":
                        return !string.IsNullOrEmpty(data.CombineNumber) ? data.CombineNumber : "----";
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}