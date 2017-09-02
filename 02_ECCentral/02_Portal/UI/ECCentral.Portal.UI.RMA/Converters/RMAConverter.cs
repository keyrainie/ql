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
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.UI.RMA.Resources;

namespace ECCentral.Portal.UI.RMA.Converters
{
    /// <summary>
    /// 根据ProcessType和相关状态信息控制按钮是否可用，不隐藏按钮
    /// 控制SellerMemo是否可见
    /// </summary>
    public class RMAConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            RegisterVM vm = value as RegisterVM;
            string para = parameter.ToString().Trim();

            #region FunctionPanel按钮的控制
            //Comment by Hax 改为在View的.cs中控制
            //if (para == "SetWaitingOutbound")//processtype:met not support
            //{
            //    return vm.BasicInfo.OutBoundStatus == null 
            //        && vm.BasicInfo.ProcessType != BizEntity.RMA.ProcessType.MET;
            //}
            //else if (para == "CancelWaitingOutbound")//processtype:met not support
            //{
            //    return vm.BasicInfo.OutBoundStatus == RMAOutBoundStatus.Origin 
            //        && vm.BasicInfo.ProcessType != BizEntity.RMA.ProcessType.MET;
            //}
            //else if (para == "SetWaitingRevert")//Processtype:met not support
            //{
            //    return vm.RevertInfo.RevertStatus == null
            //        &&vm.BasicInfo.ProcessType != BizEntity.RMA.ProcessType.MET;
            //}
            //else if (para == "CancelWaitingRevert")//Processtype:met not support
            //{
            //    return vm.BasicInfo.ProcessType != BizEntity.RMA.ProcessType.MET
            //            && (vm.RevertInfo.RevertStatus == RMARevertStatus.WaitingRevert
            //                || vm.RevertInfo.RevertStatus == RMARevertStatus.WaitingAudit);
            //}
            //else if (para == "SetWaitingRefund")//InvoceType:MET met not support
            //{
            //    return vm.BasicInfo.ProcessType == BizEntity.RMA.ProcessType.NEG
            //            && vm.BasicInfo.RefundStatus == null;
            //}
            //else if (para == "CancelWaitingRefund")//InvoceType:MET met not support
            //{
            //    return vm.BasicInfo.ProcessType == BizEntity.RMA.ProcessType.NEG 
            //            && vm.BasicInfo.RefundStatus == RMARefundStatus.WaitingRefund;
            //}
            //else if (para == "Close")//貌似没有用，IPP3中没有注册对应的js事件
            //{
            //    return vm.BasicInfo.Status == RMARequestStatus.Handling;                
            //}
            //else if (para == "CloseCase")//单件结案 
            //{
            //    return vm.BasicInfo.ProcessType == ProcessType.MET
            //        && vm.BasicInfo.Status==RMARequestStatus.Handling
            //        && vm.BasicInfo.RefundStatus != RMARefundStatus.WaitingRefund;
            //}
            //else if (para == "ReOpen")
            //{
            //    return vm.BasicInfo.Status == RMARequestStatus.Complete;
            //}
            //else if (para == "SetAbandon")
            //{
            //    return vm.BasicInfo.Status == RMARequestStatus.Origin
            //            && vm.BasicInfo.ProcessType == ProcessType.MET
            //            && vm.BasicInfo.SellerReceived.ToUpper() == "N";
            //}

            #endregion

            if (para == "SellerMemo")
            {
                return vm.BasicInfo.ProcessType == ProcessType.MET ? Visibility.Visible : Visibility.Collapsed;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class RevertStatusConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            RMARevertStatus? revertStatus = (RMARevertStatus?)value;

            if (parameter != null && parameter.ToString() == "NewProductStatus")
            {
                return revertStatus == null ? true : false;
            }

            return revertStatus == RMARevertStatus.WaitingAudit;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// 根据商品种类决定按钮是否显示
    /// </summary>
    public class ProductTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var productType = (SOProductType)value;
            return productType != SOProductType.ExtendWarranty ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class ReturnPriceTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ReturnPriceType returnPriceType = (ReturnPriceType)value;
            string para = parameter.ToString();
            if (returnPriceType == ReturnPriceType.InputPrice)
            {
                return para == "Input" ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return para == "Input" ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// 当ShipViaCode为邮局普包的时候，邮资转积分数可编辑
    /// </summary>
    public class ShipViaCodeConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null && value.ToString() == "邮局普包" ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class RefundStatusToVisibleConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)//TextBox
            {
                return parameter.ToString() == "Y" ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return parameter.ToString() == "N" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class RequestConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as dynamic;
            RMARequestStatus status = (RMARequestStatus)data.Status;
            string para = parameter.ToString();
            switch (para)
            {
                case "operate":
                    {
                        if (status == RMARequestStatus.WaitingAudit)
                            return ResRequestQuery.Button_Audit;
                        else
                            return ResRequestQuery.Button_Edit;
                    }
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
