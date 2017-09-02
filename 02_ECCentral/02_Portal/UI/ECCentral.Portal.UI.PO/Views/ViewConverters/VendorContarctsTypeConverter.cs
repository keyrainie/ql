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

namespace ECCentral.Portal.UI.PO.Views.ViewConverters
{
    public class VendorContarctsTypeConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string getVendorContractsSysNosString = value.ToString();
            string contractInfo = string.Empty;

            if (!string.IsNullOrEmpty(getVendorContractsSysNosString))
            {
                string[] Nos = getVendorContractsSysNosString.Split(new char[] { ',' });
                for (int i = 0; i < Nos.Length; i++)
                {
                    if (contractInfo != string.Empty)
                        contractInfo += "," + EnumConverter.GetDescription(Nos[i], typeof(VendorContractType));
                    else
                        contractInfo += EnumConverter.GetDescription(Nos[i], typeof(VendorContractType));
                }
            }
            return contractInfo;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
