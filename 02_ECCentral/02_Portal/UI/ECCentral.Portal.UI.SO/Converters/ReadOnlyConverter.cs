using System;
using System.Windows;
using System.Windows.Data;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Models;

namespace ECCentral.Portal.UI.SO.Converters
{
    public class ReadOnlyConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string para = parameter.ToString();
            switch (para)
            {
                #region SOMaintain
                case "SOMaintain_txtInputOrderQuantity":
                    if (
                        //改单不能修改订单中的商品
                        (
                            (((SOItemInfoVM)value).ProductType == SOProductType.Product || ((SOItemInfoVM)value).ProductType == SOProductType.Gift) && 
                            ((SOItemInfoVM)value).SOVM.SysNo.GetValueOrDefault() > 0 
                        )
                        || 
                        (((SOItemInfoVM)value).ProductID.ToUpper().Contains("GC-002-")
                        ||
                        (((SOItemInfoVM)value).ProductType != SOProductType.Product
                            &&
                            ((SOItemInfoVM)value).ProductType != SOProductType.Gift
                        )
                        || !((SOItemInfoVM)value).IsEditItem
                        || (((SOItemInfoVM)value).SOVM != null && ((SOItemInfoVM)value).SOVM.BaseInfoVM.SplitType == SOSplitType.SubSO))
                       )
                    {
                        return true;
                    }
                    break;
                #endregion
                #region SOPriceCompensation
                case "SOPriceCompensation_txtInputOrderQuantity":
                    if (((SOItemInfoVM)value).ProductType != SOProductType.Product)
                    {
                        return true;
                    }
                    break;
                case "SOPriceCompensation_txtPrice_AdjustPrice_Reason":
                    if (((SOItemInfoVM)value).ProductType != SOProductType.Product)
                    {
                        return true;
                    }
                    break;
                #endregion
                default:
                    break;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
