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
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.PO.Converters
{
    public class PurchaseOrderConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as DynamicXml;
            if (data != null)
            {
                string param = parameter.ToString();
                if (param.Equals("Status"))
                {
                    switch (param)
                    {
                        case "Status":
                            if ((PurchaseOrderStatus?)data["status"] == PurchaseOrderStatus.AutoAbandoned || (PurchaseOrderStatus?)data["status"] == PurchaseOrderStatus.Abandoned)
                            {
                                return new SolidColorBrush(Colors.Gray);
                            }
                            //else if ((PurchaseOrderStatus?)data["status"] == PurchaseOrderStatus.WaitingApportion)
                            //{
                            //    return new SolidColorBrush(Colors.Purple);
                            //}
                            else if ((PurchaseOrderStatus?)data["status"] == PurchaseOrderStatus.WaitingInStock)
                            {
                                return new SolidColorBrush(Colors.Red);
                            }
                            else if ((PurchaseOrderStatus?)data["status"] == PurchaseOrderStatus.InStocked)
                            {
                                return new SolidColorBrush(Colors.Blue);
                            }
                            else
                            {
                                return new SolidColorBrush(Colors.Black);
                            }
                        default:
                            return null;
                    }
                }
                else if (param.Equals("OperateName"))
                {
                    if ((PurchaseOrderStatus?)data["status"] == PurchaseOrderStatus.InStocked)
                    {
                        return "查看";
                    }
                    else
                    {
                        return "编辑";
                    }
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
