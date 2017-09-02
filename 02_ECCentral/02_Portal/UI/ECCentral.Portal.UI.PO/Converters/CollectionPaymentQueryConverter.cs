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
    public class CollectionPaymentQueryConverter : IValueConverter
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
                        if ((POCollectionPaymentSettleStatus?)data["Status"] == POCollectionPaymentSettleStatus.Settled)
                        {
                            return new SolidColorBrush(Colors.Blue);
                        }
                        else if ((POCollectionPaymentSettleStatus?)data["Status"] == POCollectionPaymentSettleStatus.Abandon)
                        {
                            return new SolidColorBrush(Colors.Gray);
                        }
                        else if ((POCollectionPaymentSettleStatus?)data["Status"] == POCollectionPaymentSettleStatus.Audited)
                        {
                            return new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            return new SolidColorBrush(Colors.Black);
                        }
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
