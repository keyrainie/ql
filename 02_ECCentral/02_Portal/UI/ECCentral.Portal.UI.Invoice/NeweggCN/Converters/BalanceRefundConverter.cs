using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Converters
{
    public class BalanceRefundConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as dynamic;
            if (data != null)
            {
                var par = parameter.ToString();
                switch (par)
                {
                    case "Status":
                        if (data.Status == BalanceRefundStatus.Abandon)
                        {
                            return new SolidColorBrush(Colors.Gray);
                        }
                        else if (data.Status == BalanceRefundStatus.FinConfirmed)
                        {
                            return new SolidColorBrush(Colors.Green);
                        }
                        return new SolidColorBrush(Colors.Black);

                    case "ReferenceID":
                        if (string.IsNullOrEmpty(data.ReferenceID))
                        {
                            return "N/A";
                        }
                        return data.ReferenceID;

                    case "ReferenceID_Enable":
                        return AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_BalanceRefundQuery_SetReferenceID);

                    case "Hyperlink_Edit":
                        if (data.Status == BalanceRefundStatus.Origin || data.Status == BalanceRefundStatus.CSConfirmed)
                        {
                            return Visibility.Visible;
                        }
                        return Visibility.Collapsed;
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