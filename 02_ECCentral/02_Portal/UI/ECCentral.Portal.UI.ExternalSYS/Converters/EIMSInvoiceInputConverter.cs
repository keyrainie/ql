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
using ECCentral.Portal.UI.ExternalSYS.Models;

namespace ECCentral.Portal.UI.ExternalSYS.Converters
{
    public class EIMSInvoiceInputConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as EIMSInvoiceEntryVM;
            if (data != null)
            {
                var par = parameter.ToString();
                switch (par)
                {
                    #region 操作
                    case "Hyperlink_Input":
                        if (data.IsSAPImported == "N" && data.InvoiceInputStatus == "未录入")
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    case "Hyperlink_Edit":
                        if (data.IsSAPImported == "N" && data.InvoiceInputStatus == "已录入")
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    case "Hyperlink_View":
                        if (data.IsSAPImported == "Y")
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    case "Hyperlink_Remove":
                        if (data.IsSAPImportedDes == "已上传")
                            return Visibility.Collapsed;
                        else
                            return Visibility.Visible;
                    #endregion
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
