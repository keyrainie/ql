using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;

namespace ECCentral.Portal.UI.Invoice.Converters
{
    public class PayableConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as PayableVM;
            if (data != null)
            {
                var par = parameter.ToString();
                switch (par)
                {
                    case "Hyperlink_Edit":
                        return (!string.IsNullOrEmpty(data.Operation)) ? Visibility.Visible : Visibility.Collapsed;
                    case "TextBlock_Disabled":
                        return (string.IsNullOrEmpty(data.Operation)) ? Visibility.Visible : Visibility.Collapsed;
                    case "Hyperlink_OrderID":
                        if (true)
                        {
                            return Visibility.Visible;
                        }
                        //else
                        //{
                        //    return Visibility.Collapsed;
                        //}
                    case "TextBlock_OrderID":
                        if (true)
                        {
                            return Visibility.Collapsed;
                        }
                        //else
                        //{
                        //    return Visibility.Visible;
                        //}
                    case "PayStatus":
                        if (data.PayStatus == PayableStatus.Abandon)
                        {
                            return new SolidColorBrush(Colors.Purple);
                        }
                        else if (data.PayStatus == PayableStatus.FullPay)
                        {
                            return new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            return new SolidColorBrush(Colors.Black);
                        }
                    case "Hyperlink_InvoiceStatus":
                        return AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayQuery_UpdateInvoiceStatus);

                    case "SapImportedStatus":
                        if (data.SapImportedStatus == null)
                        {
                            return ECCentral.Portal.UI.Invoice.Resources.ResPayQuery.Msg_SapImportUnhandled;
                        }
                        return data.SapImportedStatus.ToDescription();
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