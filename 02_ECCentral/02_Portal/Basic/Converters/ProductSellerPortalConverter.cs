using System;
using System.Globalization;
using System.Windows.Data;

using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.Basic.Converters
{
    public class ProductSellerPortalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SellerProductRequestStatus)
            {
                switch ((SellerProductRequestStatus)value)
                {
                    case SellerProductRequestStatus.WaitApproval:
                        return "审核";
                    case SellerProductRequestStatus.Approved:
                        return "编辑";
                    default:
                        return "查看";
                }
            }

            return "查看";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
