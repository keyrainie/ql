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

namespace ECCentral.Portal.UI.PO.Converters
{
    public class SettleRuleConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as DynamicXml;
            var status = (ConsignSettleRuleStatus?)data["Status"];
            if (data != null)
            {
                string param = parameter.ToString();
                switch (param)
                {
                    //case "OperationText":
                    //    if ((ConsignSettleRuleStatus?)data["Status"] == ConsignSettleRuleStatus.Wait_Audit)
                    //    {
                    //        return "编辑";
                    //    }

                    //    else
                    //    {
                    //        return "查看";
                    //    }
                    //default:
                    //    return null;
                    case "Audit":
                        if (status == ConsignSettleRuleStatus.Wait_Audit)
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    case "Stop":
                        if (status == ConsignSettleRuleStatus.Available || status == ConsignSettleRuleStatus.Enable)
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    case "Abandon":
                        if (status == ConsignSettleRuleStatus.Wait_Audit)
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
            return null;
        }

        #endregion
    }
}
