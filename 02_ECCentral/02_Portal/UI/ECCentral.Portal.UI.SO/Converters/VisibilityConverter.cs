using System;
using System.Windows;
using System.Windows.Data;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Models;

namespace ECCentral.Portal.UI.SO.Converters
{
    public class VisibilityConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility result = Visibility.Collapsed;

            var data = value as DynamicXml;
            string para = parameter.ToString();
            switch (para)
            {
                #region SOPending
                case "SOPending_ModifySO":
                    if ((SOPendingStatus)data["Status"] == SOPendingStatus.Origin
                    && (int)data["IsPartialShipping_Value"] == 0)
                    {
                        return Visibility.Visible;
                    }
                    break;
                case "SOPending_UpdateSO":
                    if ((SOPendingStatus)data["Status"] == SOPendingStatus.Origin
                    && (int)data["IsPartialShipping_Value"] != 0)
                    {
                        return Visibility.Visible;
                    }
                    break;
                case "SOPending_CloseSO":
                    if ((SOPendingStatus)data["Status"] != SOPendingStatus.Complete)
                    {
                        return Visibility.Visible;
                    }
                    break;
                case "SOPending_OpenSO":
                    if ((SOPendingStatus)data["Status"] == SOPendingStatus.Complete
                    && data["SoStatusShow"] != null
                    && (int)data["SoStatusShow"] > 0
                    && (int)data["SoStatusShow"] < 4)
                    {
                        return Visibility.Visible;
                    }
                    break;
                #endregion
                #region SOInternalMemo
                case "SOInternalMemo_Close":
                    if ((SOInternalMemoStatus)data["Status"] == SOInternalMemoStatus.FollowUp)
                    {
                        return Visibility.Visible;
                    }
                    break;
                #endregion
                #region SOComplain
                case "SOComplain_IsUpgrade":
                    if ((SOComplainStatus)data["Status"] == SOComplainStatus.Upgrade)
                    {
                        return Visibility.Visible;
                    }
                    break;
                #endregion
                #region SOMaintain
                case "SOMaintain_img_Car_OrdeQuantityAddsubtract":
                    var dataSICO = (SOItemInfoVM)value;
                    if (dataSICO.ProductType == SOProductType.Product && dataSICO.IsEditItem)
                    {
                        result = Visibility.Visible;
                    }
                    break;
                case "SOMaintain_img_Car_OrdeQuantityAddsubtract_Temp":
                    var dataSICOT = (SOItemInfoVM)value;
                    if (
                        !(dataSICOT.ProductType == SOProductType.Product && dataSICOT.IsEditItem)
                        ||
                        (   !string.IsNullOrEmpty(dataSICOT.MasterProductSysNo)
                            && dataSICOT.ProductType != SOProductType.Product
                        )
                        ||(dataSICOT.ProductType != SOProductType.Coupon && dataSICOT.ProductID.ToUpper().Contains("GC-002-"))                     
                        || dataSICOT.ProductType == SOProductType.Coupon
                        )
                    {
                        result = Visibility.Visible;
                    }
                    break;
                case "SOMaintain_hlkb_Car_DeleteItem":
                    if (((SOItemInfoVM)value).ProductType != SOProductType.Coupon && ((SOItemInfoVM)value).IsEditItem
                        && ((SOItemInfoVM)value).SOVM.SysNo.GetValueOrDefault() <= 0) //改单不能修改订单中的商品
                    {
                        result = Visibility.Visible;
                    }
                    break;
                case "SOMaintain_IsBackOrder":
                    if (value != null && value is bool && (bool)value)
                    {
                        return Visibility.Visible;
                    }
                    break;
                case "SOMaintain_NotEoughStock":
                    if (value != null && value is bool && (bool)value)
                    {
                        return Visibility.Visible;
                    }
                    break;
                case "SOMaintain_NeedPO":
                    if (value != null && value is SOItemInfoVM && ((SOItemInfoVM)value).IsWaitPO)
                    {
                        return Visibility.Visible;
                    }
                    break;
                case "SOMaintain_RuleSale":
                    if (value != null && value is SOVM && ((SOVM)value).ComboPromotionsVM.Count > 0)
                    {
                        return Visibility.Visible;
                    }
                    break;
                case "SOMaintain_HoldSpace":
                    if (value != null
                        && (SOHoldStatus)value == SOHoldStatus.BackHold
                        && (SOHoldStatus)value == SOHoldStatus.WebHold
                        )
                    {
                        return Visibility.Visible;
                    }
                    break;
                #endregion
                default:
                    break;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
