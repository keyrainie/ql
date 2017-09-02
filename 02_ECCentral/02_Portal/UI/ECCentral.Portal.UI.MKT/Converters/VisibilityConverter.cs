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
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.UI.MKT.Models;

namespace ECCentral.Portal.UI.MKT.Converters
{
    public class MKTVisibilityConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// 如果是厂商回复就不能执行批量操作
    /// </summary>
    public class ProductCommentShowCheckConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()) && value.ToString() != "M")
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// 针对厂商回复的状态来操作是否可选
    /// </summary>
    public class FactoryReplyShowCheckConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()) && value.ToString() == "O")
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class CouponsOperationEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = false;
            var data = value as DynamicXml;
            string para = parameter.ToString().ToUpper();
            CouponsStatus status = CouponsStatus.Finish;
            Enum.TryParse<CouponsStatus>(data["Status"].ToString(), out status);
            if (para == "EDIT")
            {
                if (status == CouponsStatus.Init || status == CouponsStatus.Ready)
                {
                    result = true;
                }
            }
            else if (para == "MGT")
            {
                if (status == CouponsStatus.Init || status == CouponsStatus.Ready
                    || status == CouponsStatus.Run || status == CouponsStatus.WaitingAudit)
                {
                    result = true;
                }
            }

            return result;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SaleGiftOperationEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = false;
            var data = value as DynamicXml;
            string para = parameter.ToString().ToUpper();
            SaleGiftStatus status = SaleGiftStatus.Finish;
            int reqestSysNo = 0; //商家创建的商品不能复制新建
            int.TryParse(data["RequestSysNo"].ToString(), out reqestSysNo);
            Enum.TryParse<SaleGiftStatus>(data["Status"].ToString(), out status);

            if (para == "EDIT")
            {
                if (status == SaleGiftStatus.Init)
                {
                    result = true;
                }
            }
            else if (para == "MGT")
            {
                if (status == SaleGiftStatus.Init || status == SaleGiftStatus.Ready
                    || status == SaleGiftStatus.Run || status == SaleGiftStatus.WaitingAudit)
                {
                    result = true;
                }
            }
            else if (para == "CNEW")
            {
                if ((status == SaleGiftStatus.Finish || status == SaleGiftStatus.Stoped
                    || status == SaleGiftStatus.Void) && reqestSysNo==0)
                {
                    result = true;
                }
            }

            return result;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GiftComboTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }
            SaleGiftGiftItemType? data = (SaleGiftGiftItemType?)value;
            bool result = false;

            if (data.Value == SaleGiftGiftItemType.AssignGift)
            {
                result = true;
            }
            else
            {
                result = false;
            }


            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SaleGiftGiftItemType result = SaleGiftGiftItemType.AssignGift;
            bool data = (bool)value;
            if (data)
            {
                result = SaleGiftGiftItemType.AssignGift;
            }
            else
            {
                result = SaleGiftGiftItemType.GiftPool;
            }
            return result;
        }
    }

    public class ComboCheckedConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ComboStatus status = ComboStatus.Deactive;
            Enum.TryParse<ComboStatus>(value.ToString(), out status);

            return status == ComboStatus.Active;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class ComboStatusColorConverter : IValueConverter
    {
        //待审核状态显示红色
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ComboStatus status = ComboStatus.WaitingAudit;
            Enum.TryParse<ComboStatus>(value.ToString(), out status);
            return status == ComboStatus.WaitingAudit ? "Red" : "Black";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ComboPriceDiffColorConverter : IValueConverter
    {
        //差价小于0显示红色
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal priceDiff = 0m;
            decimal.TryParse(value == null ? "0" : value.ToString(), out priceDiff);
            return priceDiff < 0 ? "Red" : "Black";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// 根据Combo的SysNo是否大于0，控制Combo状态的RadioButton是否显示
    /// </summary>
    public class ComboStatusVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            int sysNo;
            if (int.TryParse(value.ToString(), out sysNo))
            {
                return sysNo > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class GroupBuyOperationEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = false;
            var data = value as DynamicXml;
            string para = parameter.ToString().ToUpper();
            GroupBuyingStatus status = GroupBuyingStatus.Pending;
            Enum.TryParse<GroupBuyingStatus>(data["Status"].ToString(), out status);

            if (para == "EDIT")
            {
                if (status != GroupBuyingStatus.WaitingAudit)
                {
                    result = true;
                }
            }
            else if (para == "MGT")
            {
                if (status != GroupBuyingStatus.Deactive && status != GroupBuyingStatus.Finished)
                {
                    result = true;
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GroupBuyStatusColorConverter : IValueConverter
    {
        //状态显示颜色
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string returnVal = string.Empty;
            GroupBuyingStatus status = GroupBuyingStatus.WaitingAudit;
            Enum.TryParse<GroupBuyingStatus>(value.ToString(), out status);
            switch (status)
            {
                case GroupBuyingStatus.WaitingAudit:
                    returnVal = "#8B1F11";
                    break;
                case GroupBuyingStatus.VerifyFaild:
                    returnVal = "RED";
                    break;
                case GroupBuyingStatus.Pending:
                    returnVal = "GREEN";
                    break;
                case GroupBuyingStatus.WaitHandling:
                    returnVal = "LIME";
                    break;
                case GroupBuyingStatus.Active:
                    returnVal = "BLUE";
                    break;
                case GroupBuyingStatus.Deactive:
                    returnVal = "GRAY";
                    break;
                case GroupBuyingStatus.Finished:
                    returnVal = "#C66666";
                    break;
                default:
                    returnVal = "Black";
                    break;
            }
            return returnVal;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class SaleAdvItemStatusConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ADStatus status = (ADStatus)value;
            string para = parameter.ToString();
            if (para.ToUpper() == "A")
            {
                return status == ADStatus.Active ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return status == ADStatus.Active ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class CountdownOperationEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = false;
            var data = value as DynamicXml;
            string para = parameter.ToString().ToUpper();
            CountdownStatus status = (CountdownStatus)data["Status"];

            if (para == "EDIT")
            {
                if (status == CountdownStatus.Init
                    || status == CountdownStatus.Ready
                    || status == CountdownStatus.VerifyFaild)
                {
                    result = true;
                }
            }
            else if (para == "MGT")
            {
                if (status == CountdownStatus.Init
                    || status == CountdownStatus.WaitForVerify
                    || status == CountdownStatus.Ready
                    || status == CountdownStatus.VerifyFaild)
                {
                    result = true;
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CountdownIsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null && parameter.Equals("SecondKill"))
            {
                return value.Equals("DC") ? "是" : "否";
            }
            else
            {
                return value.Equals(0) ? "否" : "是";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Grid行内操作的控制
    /// </summary>
    public class SegmentActionConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            KeywordsStatus status = (KeywordsStatus)value;
            if (parameter.ToString() == "Verify")
            {
                if (status == KeywordsStatus.Waiting)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            else
            {
                if (status == KeywordsStatus.Waiting)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class SearchedKeywordStatusConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ADStatus status = (ADStatus)value;
            if (status == ADStatus.Active)
            {
                return "展示";
            }
            else
            {
                return "屏蔽";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HotKeyWordsIsOnlineShowConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "是";
            }
            var status = (ECCentral.BizEntity.MKT.NYNStatus)value;
            if (status == ECCentral.BizEntity.MKT.NYNStatus.No)
            {
                return "是";
            }
            else
            {
                return "否";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FeedbackStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            GroupBuyingFeedbackStatus status = (GroupBuyingFeedbackStatus)value;
            if (status == GroupBuyingFeedbackStatus.Readed)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CooperationStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BusinessCooperationStatus status = (BusinessCooperationStatus)value;
            if (status == BusinessCooperationStatus.Handled)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
