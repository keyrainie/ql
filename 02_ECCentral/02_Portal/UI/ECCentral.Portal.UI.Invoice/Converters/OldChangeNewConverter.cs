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
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Portal.UI.Invoice.Converters
{
    public class OldChangeNewConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as OldChangeNewQueryVM;
            if (data != null)
            {
                var par = parameter.ToString();
                switch (par)
                {
                    #region 状态颜色
                    case "Status_Color":
                        switch (data.Status)
                        {
                            case OldChangeNewStatus.Abandon:
                                return new SolidColorBrush(Colors.Gray);
                            case OldChangeNewStatus.Audited:
                                return new SolidColorBrush(Colors.Brown);
                            case OldChangeNewStatus.Close:
                                return new SolidColorBrush(Colors.Green);
                            case OldChangeNewStatus.Origin:
                                return new SolidColorBrush(Colors.Black);
                            case OldChangeNewStatus.Refund:
                                return new SolidColorBrush(Colors.DarkGray);
                            case OldChangeNewStatus.RefuseAudit:
                                return new SolidColorBrush(Colors.Red);
                            case OldChangeNewStatus.SubmitAudit:
                                return new SolidColorBrush(Colors.Blue);
                        }
                        return new SolidColorBrush(Colors.Black);
                    #endregion

                    #region 操作
                    case "Hyperlink_View":
                        if (data.StatusCode < 0 || data.StatusCode >= 2)
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    case "Hyperlink_Edit":
                        if (data.StatusCode >= 0 && data.StatusCode < 2)
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    case "Hyperlink_Close":
                        if (data.StatusCode == 3)
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    case "Hyperlink_Refund":
                        if (data.StatusCode == 2)
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    #endregion
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
