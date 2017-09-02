using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;

namespace ECCentral.Portal.UI.Invoice.Converters
{
    public class InvoiceConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as InvoiceVM;
            if (data != null)
            {
                string par = parameter.ToString();
                switch (par)
                {
                    case "PrepayAmt":
                        return (data.PrepayAmt != 0 && data.OrderType == SOIncomeOrderType.RO) ?
                            Colors.Red :
                            Colors.Black;

                    case "OrderSysNo":
                        return (data.OrderType == SOIncomeOrderType.RO_Balance) ?
                            data.NewOrderSysNo :
                            data.OrderSysNo;

                    case "IncomeAmt":
                        return (data.IncomeAmt.HasValue) ?
                            data.IncomeAmt.Value.ToString(ConstValue.Invoice_DecimalFormat) :
                            "0.00";

                    case "IncomeUser":
                        return string.Format("{0} [{1}]", data.IncomeUser, data.IncomeTime.Value.ToString(ConstValue.Invoice_LongTimeFormat));

                    case "ConfirmUser":
                        return data.ConfirmTime.HasValue ?
                            string.Format("{0} [{1}]", data.IncomeUser, data.IncomeTime.Value.ToString(ConstValue.Invoice_LongTimeFormat)) :
                            string.Empty;

                    case "InvoiceNo":
                        if (string.IsNullOrEmpty(data.InvoiceNo))
                            if (data.OrderType == SOIncomeOrderType.SO)
                                return "N/A";
                            else
                                return string.Empty;
                        else
                            return data.InvoiceNo;

                    case "IncomeStatus":
                        if (data.IncomeStatus == SOIncomeStatus.Abandon)
                        {
                            return new SolidColorBrush(Colors.LightGray);
                        }
                        else if (data.IncomeStatus == SOIncomeStatus.Confirmed)
                        {
                            return new SolidColorBrush(Colors.Green);
                        }
                        return new SolidColorBrush(Colors.Black);

                    case "IncomeStyle":
                        if (data.IncomeStyle == SOIncomeOrderStyle.Normal)
                        {
                            return new SolidColorBrush(Colors.Purple);
                        }
                        return new SolidColorBrush(Colors.Black);

                    case "SapImportedStatus":
                        if (data.SapImportedStatus == null)
                        {
                            return ECCentral.Portal.UI.Invoice.Resources.ResInvoiceQuery.Message_SapImportUnhandled;
                        }
                        return data.SapImportedStatus.ToDescription();

                    case "InvoiceNo_HyperlinkButton":
                        return AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceQuery_Edit) ? Visibility.Visible : Visibility.Collapsed;
                    case "InvoiceNo_HyperlinkButton_IsEnabled":
                        if (data.OrderType == SOIncomeOrderType.SO && AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceQuery_Edit))
                            return true;
                        else
                            return false;
                    case "InvoiceNo_TextBlock":
                        return !AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceQuery_Edit) ? Visibility.Visible : Visibility.Collapsed;

                    case "Hyperlink_SapImportedStatus":
                        return data.SapImportedStatus == SapImportedStatus.Fault ? Visibility.Visible : Visibility.Collapsed;

                    case "TextBlock_SapImportedStatus":
                        return data.SapImportedStatus == SapImportedStatus.Fault ? Visibility.Collapsed : Visibility.Visible;
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