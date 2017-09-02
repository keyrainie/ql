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
    public class CostChangeConverter : IValueConverter
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
                            if ((int?)data["Status"] == (int)CostChangeStatus.Abandoned)
                            {
                                return new SolidColorBrush(Colors.Gray);
                            }
                            else if ((int?)data["Status"] == (int)CostChangeStatus.WaitingForAudited)
                            {
                                return new SolidColorBrush(Colors.Red);
                            }
                            else if ((int?)data["Status"] == (int)CostChangeStatus.Created)
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
                    if ((int?)data["Status"] == (int)CostChangeStatus.Audited || (int?)data["Status"] == (int)CostChangeStatus.Abandoned)
                    {
                        return "查看";
                    }
                    else if ((int?)data["Status"] == (int)CostChangeStatus.Created || (int?)data["Status"] == (int)CostChangeStatus.WaitingForAudited)
                    {
                        return "编辑";
                    }
                    else
                    {
                        return string.Empty;
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
