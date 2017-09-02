using System;
using System.Windows;
using System.Windows.Data;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Invoice.Models;

namespace ECCentral.Portal.UI.Invoice.Converters
{
    public class TrackingInfoConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as TrackingInfoVM;
            if (data != null)
            {
                var par = parameter.ToString();
                switch (par)
                {
                    case "EditInfo":
                        if (!string.IsNullOrEmpty(data.EditUser) && data.EditDate.HasValue)
                        {
                            return string.Format("{0}\r\n[{1}]", data.EditUser.Split('\\')[0],
                                data.EditDate.Value.ToString(ConstValue.Invoice_LongTimeFormat));
                        }
                        else
                        {
                            return string.Empty;
                        }

                    case "OrderSysNo":
                        if (data.OrderType == SOIncomeOrderType.RO_Balance)
                        {
                            return data.OrderSysNo.ToString();
                        }
                        else
                        {
                            return data.LinkSysNo.ToString();
                        }

                    case "Hyperlink_Edit":
                        return (AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_ARWindowQuery_UpdateTrackingInfo)) ?
                            Visibility.Visible :
                            Visibility.Collapsed;

                    case "TextBlock_Disabled":
                        return (AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_ARWindowQuery_UpdateTrackingInfo)) ?
                            Visibility.Collapsed :
                            Visibility.Visible;
                }
            }
            return data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}