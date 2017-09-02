using System;
using System.Net;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;

/*
 销售收款单相关Converter
 */

namespace ECCentral.Portal.UI.Invoice.Converters
{
    public class SaleIncomeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as SaleIncomeVM;
            if (data != null)
            {
                string par = parameter.ToString();
                switch (par)
                {
                    case "ToleranceAmt":
                        if (data.ToleranceAmt == -1 || data.ToleranceAmt == -999999)
                        {
                            return "N/A";
                        }
                        return data.ToleranceAmt.HasValue ? data.ToleranceAmt.Value.ToString(ConstValue.Invoice_DecimalFormat)
                            : "0.00";

                    case "IncomeUser":
                        string incomeUserFormat = "{0} [{1}]";
                        if (data.IncomeTime.HasValue)
                        {
                            return string.Format(incomeUserFormat, data.IncomeUser, data.IncomeTime.Value.ToString(ConstValue.Invoice_LongTimeFormat));
                        }
                        else
                        {
                            return string.Format(incomeUserFormat, data.IncomeUser, string.Empty);
                        }

                    case "ConfirmUser":
                        string confirmUserFormat = "{0} [{1}]";
                        if (data.ConfirmTime.HasValue)
                        {
                            return string.Format(confirmUserFormat, data.ConfirmUser, data.ConfirmTime.Value.ToString(ConstValue.Invoice_LongTimeFormat));
                        }
                        return string.Empty;

                    case "ReturnPointAmt":
                        if (data.ReturnPointAmt == -1 || data.ReturnPointAmt == -999999)
                        {
                            return "N/A";
                        }
                        return data.ReturnPointAmt.HasValue ? data.ReturnPointAmt.ToString() : "0";

                    case "ReturnCash":
                        if (data.ReturnCash == -1 || data.ReturnCash == -999999)
                        {
                            return "N/A";
                        }
                        return data.ReturnCash.HasValue ? data.ReturnCash.Value.ToString(ConstValue.Invoice_DecimalFormat)
                            : "0.00";

                    case "ReferenceID":
                        return !string.IsNullOrEmpty(data.ReferenceID) ? data.ReferenceID : "N/A";

                    case "OrderType":
                        string combineOrderTypeString = data.OrderType.ToDescription();
                        if (data.OrderType == SOIncomeOrderType.SO && (data.IsCombine ?? false))
                        {
                            combineOrderTypeString = combineOrderTypeString + ResSaleIncomeQuery.String_CombineOrder;
                        }
                        return combineOrderTypeString;

                    case "SapImportedStatus":
                        if (data.SapImportedStatus == null)
                        {
                            return SapImportedStatus.UnHandle.ToDescription();
                        }
                        return data.SapImportedStatus.ToDescription();

                    case "OrderID_HyperlinkButton":
                        return (data.OrderType != SOIncomeOrderType.RO_Balance) ? Visibility.Visible : Visibility.Collapsed;

                    case "OrderID_TextBlock":
                        return (data.OrderType == SOIncomeOrderType.RO_Balance) ? Visibility.Visible : Visibility.Collapsed;

                    case "IncomeAmt_HyperlinkButton":

                        return AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeQuery_SetIncomeAmount)
                            && data.IncomeStyle == SOIncomeOrderStyle.Normal
                            && data.IncomeStatus == SOIncomeStatus.Origin;

                        //return visiable? Visibility.Visible : Visibility.Collapsed;

                    case "IncomeAmt_TextBlock":
                        return !AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeQuery_SetIncomeAmount) ? Visibility.Visible : Visibility.Collapsed;

                    case "ReferenceID_HyperlinkButton":
                        return AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeQuery_SetReferenceID) ? Visibility.Visible : Visibility.Collapsed;

                    case "ReferenceID_TextBlock":
                        return !AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeQuery_SetReferenceID) ? Visibility.Visible : Visibility.Collapsed;

                    case "Hyperlink_SapImportedStatus":
                        return data.SapImportedStatus == SapImportedStatus.Fault ? Visibility.Visible : Visibility.Collapsed;

                    case "TextBlock_SapImportedStatus":
                        return data.SapImportedStatus == SapImportedStatus.Fault ? Visibility.Collapsed : Visibility.Visible;

                    case "Hyperlink_OrderSysNo":
                        return data.OrderType == SOIncomeOrderType.RO_Balance ? Visibility.Visible : Visibility.Collapsed;

                    case "TextBlock_OrderSysNo":
                        return data.OrderType == SOIncomeOrderType.RO_Balance ? Visibility.Collapsed : Visibility.Visible;
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