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
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.UI.Customer.Resources;

namespace ECCentral.Portal.UI.Customer.Converter
{
    public class AgentTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string para = parameter.ToString();
            AgentType type = (AgentType)value;
            switch (type)
            {
                case AgentType.Personal:
                    if (para.ToUpper() == "P")
                        return Visibility.Visible;
                    return Visibility.Collapsed;
                case AgentType.Campus:
                    if (para.ToUpper() == "C")
                        return Visibility.Visible;
                    return Visibility.Collapsed;
                case AgentType.Enterprise:
                    if (para.ToUpper() == "E")
                        return Visibility.Visible;
                    return Visibility.Collapsed;
                default:
                    return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
    public class DynamaicToCustomerRankConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var par = parameter.ToString();
            switch (par)
            {
                case "Hyperlink_NewSo":
                    if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerQuery_CreateSO))
                        return Visibility.Collapsed;
                    else
                        return Visibility.Visible;
                case "Hyperlink_Edit":
                    if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerQuery_Edit))
                        return Visibility.Collapsed;
                    else
                        return Visibility.Visible;
                case "Hyperlink_CustomerRights":
                    if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerQuery_SetCustomerRights))
                        return Visibility.Collapsed;
                    else
                        return Visibility.Visible;
                case "Hyperlink_PointLog":
                    if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerQuery_PointLog))
                        return Visibility.Collapsed;
                    else
                        return Visibility.Visible;
                case "Hyperlink_ExperienceLog":
                    if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerQuery_ExperienceLog))
                        return Visibility.Collapsed;
                    else
                        return Visibility.Visible;
                case "Hyperlink_Promotion":
                    if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerQuery_PromotionLog))
                        return Visibility.Collapsed;
                    else
                        return Visibility.Visible;
                case "Hyperlink_SecurityQues":
                    if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerQuery_ViewSecurityQuestion))
                        return Visibility.Collapsed;
                    else
                        return Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class CustomerPointAudit : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            dynamic status = (CustomerPointsAddRequestStatus)(value as dynamic).status;
            var par = parameter.ToString();
            switch (par)
            {
                case "Audit":
                    if (status == CustomerPointsAddRequestStatus.AuditWaiting)
                        return Visibility.Visible;
                    else
                        return Visibility.Collapsed;
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class RefundAdjustDisplayConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var vm = value as RefundAdjustMaintainVM;
            string pars = parameter.ToString();
            switch (pars)
            {
                case "btnSave":
                    {
                        if (vm.Action == "Create")
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    }
                case "btnAudit":
                    {
                        if (vm != null && vm.Status == RefundAdjustStatus.Initial && vm.Action == "Edit")
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    }
                case "btnVoid":
                    {
                        if (vm != null
                            && (vm.Status == RefundAdjustStatus.Initial || vm.Status == RefundAdjustStatus.AuditRefuesed)
                            && vm.Action == "Edit")
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    }
                case "btnRefund":
                    {
                        if (vm != null
                            && (vm.Status == RefundAdjustStatus.Audited))
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    }
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class RefundAdjustActionConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var vm = value as RefundAdjustVM;
            string pars = parameter.ToString();
            switch (pars)
            {
                case "hlbtnEdit":
                    {
                        if (vm.Status == RefundAdjustStatus.Initial || vm.Status == RefundAdjustStatus.Audited)
                            return Visibility.Visible;
                        else
                            return
                                Visibility.Collapsed;
                    }
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class SecurityQuesDisplayConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as dynamic;
            string pars = parameter.ToString();
            switch (pars)
            {
                #region 密保问题显示
                case "TheFirstQues":
                    return string.Format(ResSecurityQuestion.TextBlock_Question, "一");
                case "TheSecondQues":
                    return string.Format(ResSecurityQuestion.TextBlock_Question, "二");
                case "TheThirdQues":
                    return string.Format(ResSecurityQuestion.TextBlock_Question, "三");
                #endregion
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
