using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;

namespace ECCentral.Portal.UI.Invoice.Converters
{
    public class PostIncomeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as PostIncomeVM;
            if (data != null)
            {
                var par = parameter.ToString();
                switch (par)
                {
                    case "Edit_View":
                        if (data.HandleStatus == PostIncomeHandleStatus.Handled)
                        {
                            return Visibility.Visible;
                        }
                        return Visibility.Collapsed;

                    case "Edit_Modify":
                        if (data.HandleStatus == PostIncomeHandleStatus.WaitingHandle)
                        {
                            return Visibility.Visible;
                        }
                        return Visibility.Collapsed;

                    case "Edit_Confirm":
                    case "Edit_Abandon":
                        if (data.ConfirmStatus == PostIncomeStatus.Origin)
                        {
                            return Visibility.Visible;
                        }
                        return Visibility.Collapsed;

                    case "Edit_CancelConfirm":
                        if (data.ConfirmStatus == PostIncomeStatus.Confirmed && data.HandleStatus != PostIncomeHandleStatus.Handled)
                        {
                            return Visibility.Visible;
                        }
                        return Visibility.Collapsed;

                    case "Edit_CancelAbandon":
                        if (data.ConfirmStatus == PostIncomeStatus.Abandon && data.HandleStatus != PostIncomeHandleStatus.Handled)
                        {
                            return Visibility.Visible;
                        }
                        return Visibility.Collapsed;

                    case "HandleStatus":
                        if (data.HandleStatus == PostIncomeHandleStatus.Handled)
                        {
                            return PostIncomeHandleStatus.Handled.ToDescription();
                        }
                        else
                        {
                            return (data.ConfirmStatus == PostIncomeStatus.Confirmed) ?
                                PostIncomeHandleStatusUI.UnHandled.ToDescription() :
                                PostIncomeHandleStatusUI.UnConfirmed.ToDescription();
                        }

                    case "HandleStatus_Color":
                        if (data.HandleStatus == PostIncomeHandleStatus.WaitingHandle)
                        {
                            if (data.ConfirmStatus == PostIncomeStatus.Confirmed)
                            {
                                return new SolidColorBrush(Colors.Green);
                            }
                            return new SolidColorBrush(Colors.Black);
                        }
                        else
                        {
                            return new SolidColorBrush(Colors.Purple);
                        }
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}