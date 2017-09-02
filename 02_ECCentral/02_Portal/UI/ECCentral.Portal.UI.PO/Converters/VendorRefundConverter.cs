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
using ECCentral.Portal.UI.PO.Models;

namespace ECCentral.Portal.UI.PO.Converters
{
    public class VendorRefundConverter : IValueConverter
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
                        if ((VendorRefundStatus?)data["Status"] == VendorRefundStatus.PMCCVerify || (VendorRefundStatus?)data["Status"] == VendorRefundStatus.PMCCToVerify || (VendorRefundStatus?)data["Status"] == VendorRefundStatus.PMVerify)
                        {
                            return new SolidColorBrush(Colors.Orange);
                        }
                        else if ((VendorRefundStatus?)data["Status"] == VendorRefundStatus.PMDVerify)
                        {
                            return new SolidColorBrush(Colors.Red);
                        }
                        else if ((VendorRefundStatus?)data["Status"] == VendorRefundStatus.Origin)
                        {
                            return new SolidColorBrush(Colors.Blue);
                        }
                        else if ((VendorRefundStatus?)data["Status"] == VendorRefundStatus.Verify)
                        {
                            return new SolidColorBrush(Colors.Brown);
                        }
                        else if ((VendorRefundStatus?)data["Status"] == VendorRefundStatus.Abandon)
                        {
                            return new SolidColorBrush(Colors.Gray);
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

    public class VendorMaintainConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as VendorAgentInfoVM;
            if (data != null)
            {
                string par = parameter.ToString();
                switch (par)
                {
                    case "ViewDetails":
                        var tmp = value as VendorAgentInfoVM;
                        if (!string.IsNullOrEmpty(tmp.Content))
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    default:
                        return Visibility.Collapsed;
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
