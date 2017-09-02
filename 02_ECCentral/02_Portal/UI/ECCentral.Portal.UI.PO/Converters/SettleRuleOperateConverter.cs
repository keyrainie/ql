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
    public class SettleRuleOperateConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as DynamicXml;
            var status = (ConsignSettleRuleStatus?)data["Status"];
            if (data != null)
            {
                if (status == ConsignSettleRuleStatus.Wait_Audit)
                    return "编辑";
                else 
                    return "查看";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
