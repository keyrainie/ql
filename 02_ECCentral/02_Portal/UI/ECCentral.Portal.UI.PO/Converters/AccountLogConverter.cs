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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.Converters
{
    public class AccountLogConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as DynamicXml;
            if (data != null)
            {
                string param = parameter.ToString();
                switch (param)
                {
                    case "Status":
                        if ((ConsignToAccountLogStatus?)data["Status"] == ConsignToAccountLogStatus.Origin)
                        {
                            return new SolidColorBrush(Colors.Gray);
                        }
                        else if ((ConsignToAccountLogStatus?)data["Status"] == ConsignToAccountLogStatus.Settled)
                        {
                            return new SolidColorBrush(Colors.Green);
                        }
                        else if ((ConsignToAccountLogStatus?)data["Status"] == ConsignToAccountLogStatus.SystemCreated)
                        {
                            return new SolidColorBrush(Colors.Blue);
                        }
                        else
                        {
                            return new SolidColorBrush(Colors.Black);
                        }
                    case "LinkOrderSysNo":
                        if ((ConsignToAccountType?)data["ReferenceType"] == ConsignToAccountType.SO
                            || (ConsignToAccountType?)data["ReferenceType"] == ConsignToAccountType.Adjust)
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    case "NoLinkOrderSysNo":
                        if ((ConsignToAccountType?)data["ReferenceType"] == ConsignToAccountType.RMA
                            || (ConsignToAccountType?)data["ReferenceType"] == ConsignToAccountType.Manual)
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    default:
                        return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
