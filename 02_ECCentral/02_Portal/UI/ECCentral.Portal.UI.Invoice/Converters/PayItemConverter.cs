using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;

namespace ECCentral.Portal.UI.Invoice.Converters
{
    public class PayItemConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as PayItemVM;
            if (data != null)
            {
                var par = parameter.ToString();
                switch (par)
                {
                    case "ReferenceID":
                        return string.IsNullOrEmpty(data.ReferenceID) ? "N/A" : data.ReferenceID;

                    case "InvoiceUpdate":
                        string invoiceUpdateTime = "";
                        if (data.InvoiceUpdateTime.HasValue)
                        {
                            invoiceUpdateTime = data.InvoiceUpdateTime.Value.ToString(ConstValue.Invoice_LongTimeFormat);
                        }
                        return string.Format("{0} [{1}]", data.UpdateInvoiceUserName, invoiceUpdateTime);

                    case "Status_Color":
                        switch (data.Status)
                        {
                            case PayItemStatus.Abandon:
                                return new SolidColorBrush(Colors.Gray);
                            case PayItemStatus.Paid:
                                return new SolidColorBrush(Colors.Green);
                            case PayItemStatus.Locked:
                                return new SolidColorBrush(Colors.Purple);
                        }
                        return new SolidColorBrush(Colors.Black);

                    case "SapImportedStatus":
                        if (data.SapImportedStatus == null)
                        {
                            return ECCentral.Portal.UI.Invoice.Resources.ResPayItemQuery.Message_SapImportUnhandled;
                        }
                        return data.SapImportedStatus.ToDescription();

                    #region 设置凭证号

                    case "Hyperlink_ReferenceID":
                        return AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayItemQuery_SetReferenceID) ? Visibility.Visible : Visibility.Collapsed;

                    case "TextBlock_ReferenceID":
                        return !AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayItemQuery_SetReferenceID) ? Visibility.Visible : Visibility.Collapsed;

                    #endregion 设置凭证号

                    #region 设置发票状态

                    case "Hyperlink_InvoiceStatus":
                        return AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayItemQuery_UpdateInvoice) ? Visibility.Visible : Visibility.Collapsed;

                    case "TextBlock_InvoiceStatus":
                        return !AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayItemQuery_UpdateInvoice) ? Visibility.Visible : Visibility.Collapsed;

                    #endregion 设置发票状态

                    #region 单据ID

                    case "OrderID":
                        string batchnum = data.BatchNumber.HasValue ? "-" + data.BatchNumber.Value.ToString().PadLeft(2, '0') : string.Empty;
                        if (data.OrderType == PayableOrderType.PO)
                        {
                            return data.OrderID + batchnum;
                        }
                        else if (data.OrderType == PayableOrderType.POAdjust)
                        {
                            return data.OrderID + "A";
                        }
                        else
                        {
                            return data.OrderID;
                        }

                    case "Hyperlink_OrderID":
                        if (true)
                        {
                            return Visibility.Visible;
                        }
                       // return Visibility.Collapsed;

                    case "TextBlock_OrderID":
                        if (true)
                        {
                            return Visibility.Collapsed;
                        }
                        //else
                        //{
                        //    return Visibility.Visible;
                        //}

                    #endregion 单据ID

                    #region 付款单操作

                    case "Hyperlink_Modify":
                        if (data.Status == PayItemStatus.Origin)
                        {
                            if (data.OrderType != PayableOrderType.RMAPOR && data.OrderType != PayableOrderType.Commission)
                            {
                                return Visibility.Visible;
                            }
                        }
                        return Visibility.Collapsed;

                    case "Hyperlink_Abandon":
                        if (data.Status == PayItemStatus.Origin)
                        {
                            if (data.OrderType != PayableOrderType.RMAPOR)
                            {
                                return Visibility.Visible;
                            }
                        }
                        return Visibility.Collapsed;

                    case "Hyperlink_CancelAbandon":
                        if (data.Status == PayItemStatus.Abandon)
                        {
                            if (data.OrderType != PayableOrderType.RMAPOR)
                            {
                                if (data.IsVendorHolded == false)
                                {
                                    return Visibility.Visible;
                                }
                            }
                            else
                            {
                                return Visibility.Visible;
                            }
                        }
                        return Visibility.Collapsed;

                    case "Hyperlink_Pay":
                        if (data.Status == PayItemStatus.Origin)
                        {
                            return Visibility.Visible;
                        }
                        return Visibility.Collapsed;

                    case "Hyperlink_CancelPay":
                        if (data.Status == PayItemStatus.Paid)
                        {
                            if (data.OrderType == PayableOrderType.PO || data.OrderType == PayableOrderType.VendorSettleOrder || data.OrderType == PayableOrderType.CollectionSettlement)
                            {
                                if (data.IsVendorHolded == false)
                                {
                                    return Visibility.Visible;
                                }
                            }
                            else
                            {
                                return Visibility.Visible;
                            }
                        }
                        return Visibility.Collapsed;

                    case "Hyperlink_View":
                        if (data.Status == PayItemStatus.Paid)
                        {
                            return Visibility.Visible;
                        }
                        return Visibility.Collapsed;

                    case "Hyperlink_Lock":
                        if (data.Status == PayItemStatus.Origin)
                        {
                            if (data.OrderType == PayableOrderType.PO || data.OrderType == PayableOrderType.VendorSettleOrder || data.OrderType == PayableOrderType.CollectionSettlement)
                            {
                                //if (AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayItemNew_PayItemLock))
                                //{
                                    return Visibility.Visible;
                                //}
                            }
                        }
                        return Visibility.Collapsed;

                    case "Hyperlink_CancelLock":
                        if (data.Status == PayItemStatus.Locked)
                        {
                            if (data.OrderType == PayableOrderType.PO || data.OrderType == PayableOrderType.VendorSettleOrder || data.OrderType == PayableOrderType.CollectionSettlement)
                            {
                                //if (AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayItemNew_CancelPayItemLock))
                                //{
                                    return Visibility.Visible;
                                //}
                            }
                        }
                        return Visibility.Collapsed;

                    #endregion 付款单操作
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