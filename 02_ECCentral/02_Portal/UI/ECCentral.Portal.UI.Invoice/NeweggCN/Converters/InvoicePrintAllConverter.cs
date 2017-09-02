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
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Converters
{
    public class InvoicePrintAllConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DynamicXml data = value as DynamicXml;
            
            if (data != null
                && parameter != null)
            {
                switch (parameter.ToString())
                {
                    case "IsVAT":
                        switch (data["InvoiceType"].ToString())
                        { 
                            case "增票":
                                return Visibility.Collapsed;
                            default:
                                return Visibility.Visible;
                        }
                    default:
                        break;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
