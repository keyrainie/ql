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
    public class AuditRefundConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as AuditRefundVM;
            if (data != null)
            {
                var par = parameter.ToString();
                switch (par)
                {
                    case "CreateInfo":
                        if (data.CreateTime.HasValue)
                        {
                            return string.Format("{0} [{1}]"
                                , data.CreateUser
                                , data.CreateTime.HasValue ? data.CreateTime.Value.ToString(ConstValue.Invoice_LongTimeFormat) : "");
                        }
                        else
                            return string.Empty;

                    case "EditInfo":
                        if (data.EditTime.HasValue)
                        {
                            return string.Format("{0} [{1}]"
                             , data.EditUser
                             , data.EditTime.HasValue ? data.EditTime.Value.ToString(ConstValue.Invoice_LongTimeFormat) : "");
                        }
                        else
                            return string.Empty;

                    case "AuditInfo":
                        if (data.AuditTime.HasValue)
                        {
                            return string.Format("{0} [{1}]"
                             , data.AuditUser
                             , data.AuditTime.HasValue ? data.AuditTime.Value.ToString(ConstValue.Invoice_LongTimeFormat) : "");
                        }
                        else
                            return string.Empty;

                    case "RefundCashAmt":
                        if (data.ShipRejected == true)
                        {
                            //如果是"RMA物流拒收",则退款金额要取SOIncome中的IncomeAmt
                            return data.IncomeAmt.HasValue ? data.IncomeAmt.Value.ToString(ConstValue.Invoice_DecimalFormat) : "0.00";
                        }
                        return data.RefundCashAmt.HasValue ? data.RefundCashAmt.Value.ToString(ConstValue.Invoice_DecimalFormat) : "0.00";

                    case "Hyperlink_Edit":
                        if (data.AuditStatus == RefundStatus.Origin || data.AuditStatus == RefundStatus.WaitingRefund || data.AuditStatus == RefundStatus.WaitingFinAudit
                            || (data.AuditStatus == RefundStatus.Audit && data.RefundPayType == RefundPayType.BankRefund))
                        {
                            return Visibility.Visible;
                        }
                        return Visibility.Collapsed;

                    case "Hyperlink_RefundPoint":                      
                    case "Hyperlink_RefundPrepayCard":                     
                        return Visibility.Collapsed;

                    case "TextBlock_Disabled":
                        var editLinkShow = data.AuditStatus == RefundStatus.Origin
                            || data.AuditStatus == RefundStatus.WaitingRefund
                            || data.AuditStatus == RefundStatus.WaitingFinAudit
                            || (data.AuditStatus == RefundStatus.Audit && data.RefundPayType == RefundPayType.BankRefund);

                      
                        return (!editLinkShow) ? Visibility.Visible : Visibility.Collapsed;

                    case "AuditStatus":
                        if (data.AuditStatus == RefundStatus.WaitingFinAudit)
                        {
                            return new SolidColorBrush(Colors.Purple);
                        }
                        else if (data.AuditStatus == RefundStatus.Audit)
                        {
                            return new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            return new SolidColorBrush(Colors.Black);
                        }

                    case "RMARefundStatus":
                        if (!data.RefundStatus.HasValue || (data.RefundStatus.HasValue && (int)data.RefundStatus == -99))
                        {
                            return "N/A";
                        }
                        else
                        {
                            return data.RefundStatus.Value.ToDescription();
                        }

                    case "Hyperlink_RMANumber":
                        return (data.OrderType != RefundOrderType.RO_Balance) ? Visibility.Visible : Visibility.Collapsed;

                    case "TextBlock_RMANumber":
                        return (data.OrderType == RefundOrderType.RO_Balance) ? Visibility.Visible : Visibility.Collapsed;
                    case "Backcolor":                      
                        return new SolidColorBrush(Colors.Black);
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