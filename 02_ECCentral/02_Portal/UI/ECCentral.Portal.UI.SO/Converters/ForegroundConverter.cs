using System;
using System.Windows;
using System.Windows.Data;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;
using System.Windows.Media;

namespace ECCentral.Portal.UI.SO.Converters
{
    public class ForegroundConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as DynamicXml;
            if (data != null)
            {
                string para = parameter.ToString();
                switch (para)
                {
                    #region Complain
                    
                    case "SOComplain_Status":
                        DateTime complainNow = DateTime.Now;
                        DateTime timeA = DateTimeHelper.AddWorkMinute(complainNow, -1 * 60 * 2);
                        // 1个工作日前的时间点
                        DateTime timeB = DateTimeHelper.AddWorkMinute(complainNow, -1 * 60 * 9);
                        // 3个工作日前的时间点
                        DateTime timeC = DateTimeHelper.AddWorkMinute(complainNow, -1 * 60 * 9 * 3);

                        DateTime? tempCreateTime = (DateTime)data["CreateDate"];
                        // 根据不同的情况显示不同的颜色
                        SOComplainStatus complainStatus = (SOComplainStatus)data["Status"];
                        if (complainStatus != SOComplainStatus.Abandoned  // 未作废
                            && complainStatus != SOComplainStatus.Complete // 未完成
                            && tempCreateTime <= timeC)
                        {
                            return "Red";
                        }
                        else if (complainStatus != SOComplainStatus.Abandoned  // 未作废
                            && complainStatus != SOComplainStatus.Complete // 未完成 
                            && tempCreateTime <= timeB)
                        {
                            return "Orange";
                        }
                        else if (complainStatus != SOComplainStatus.Abandoned  // 未作废
                            && complainStatus != SOComplainStatus.Complete // 未完成 
                            && tempCreateTime <= timeA)
                        {
                            return "Blue";
                        }
                        break;
                    #endregion
                    #region SOOutStockSearch4Finance
                    case "SOOutStockSearch4Finance_IsVat":
                        if ((int)data["IsVat"] == 1)
                        {
                            return "Red";
                        }
                        break;
                    case "SOOutStockSearch4Finance_IsEnough":
                        if ((EnoughFlag)data["IsEnough"] == EnoughFlag.WaitForMoveWarehouse
                            || (EnoughFlag)data["IsEnough"] == EnoughFlag.WaitForPurchase)
                        {
                            return "Red";
                        }
                        break;
                    #endregion
                    default:
                        break;
                }
            }
            return "Black";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
