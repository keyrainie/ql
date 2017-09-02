using System;
using System.Windows;
using System.Windows.Data;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Resources;

namespace ECCentral.Portal.UI.Invoice.Converters
{
    public class PayItemListOrdeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as dynamic;
            if (data != null)
            {
                var par = parameter.ToString();
                var orderType = (PayableOrderType?)data.OrderType;
                switch (par)
                {
                    case "OrderID":
                        return data.OrderID.ToString() + "-" + data.BatchNumber.ToString().PadLeft(2, '0');

                    case "OrderStatus":
                        switch (orderType)
                        {
                            case PayableOrderType.PO:
                                var orderStatus = (PurchaseOrderStatus?)data.OrderStatus;
                                return orderStatus.ToDescription();
                            case PayableOrderType.VendorSettleOrder:
                                var vendorSettleOrderStatus = (SettleStatus?)data.OrderStatus;
                                return vendorSettleOrderStatus.ToDescription();
                        }
                        break;

                    case "Operation_Content":
                        if (orderType == PayableOrderType.PO)
                        {
                            var orderStatus = (PurchaseOrderStatus?)data.OrderStatus;
                            if (orderStatus == PurchaseOrderStatus.InStocked || orderStatus == PurchaseOrderStatus.PartlyInStocked)
                            {
                                //已入库的采购单只能添加货到付款单
                                return ResPayItemListOrderQuery.HyperlinkButton_AddNormalPayItem;
                            }
                            //else if (orderStatus == PurchaseOrderStatus.WaitingApportion || orderStatus == PurchaseOrderStatus.WaitingInStock)
                            else if (orderStatus == PurchaseOrderStatus.WaitingInStock)
                            {
                                //等待入库和等待分配的采购单只能添加预付款单
                                return ResPayItemListOrderQuery.HyperlinkButton_AddAdvancedPayItem;
                            }
                        }
                        else if (orderType == PayableOrderType.VendorSettleOrder)
                        {
                            var orderStatus = (SettleStatus?)data.OrderStatus;
                            if (orderStatus == SettleStatus.SettlePassed)
                            {
                                //已结算的代销结算只能添加货到付款单
                                return ResPayItemListOrderQuery.HyperlinkButton_AddNormalPayItem;
                            }
                        }
                        break;

                    case "Operation_Add":
                        if (orderType == PayableOrderType.PO)
                        {
                            var orderStatus = (PurchaseOrderStatus?)data.OrderStatus;
                            if (orderStatus == PurchaseOrderStatus.InStocked || orderStatus == PurchaseOrderStatus.PartlyInStocked
                                || orderStatus == PurchaseOrderStatus.WaitingInStock)
                                //|| orderStatus == PurchaseOrderStatus.WaitingApportion || orderStatus == PurchaseOrderStatus.WaitingInStock)
                            {
                                return Visibility.Visible;
                            }
                            else
                            {
                                return Visibility.Collapsed;
                            }
                        }
                        else if (orderType == PayableOrderType.VendorSettleOrder)
                        {
                            var orderStatus = (SettleStatus?)data.OrderStatus;
                            if (orderStatus == SettleStatus.SettlePassed)
                            {
                                return Visibility.Visible;
                            }
                            else
                            {
                                return Visibility.Collapsed;
                            }
                        }
                        break;

                    case "Operation_Disabled":
                        if (orderType == PayableOrderType.PO)
                        {
                            var orderStatus = (PurchaseOrderStatus?)data.OrderStatus;
                            if (orderStatus == PurchaseOrderStatus.InStocked || orderStatus == PurchaseOrderStatus.PartlyInStocked
                                || orderStatus == PurchaseOrderStatus.WaitingInStock)
                                //|| orderStatus == PurchaseOrderStatus.WaitingApportion || orderStatus == PurchaseOrderStatus.WaitingInStock)
                            {
                                return Visibility.Collapsed;
                            }
                            else
                            {
                                return Visibility.Visible;
                            }
                        }
                        else if (orderType == PayableOrderType.VendorSettleOrder)
                        {
                            var orderStatus = (SettleStatus?)data.OrderStatus;
                            if (orderStatus == SettleStatus.SettlePassed)
                            {
                                return Visibility.Collapsed;
                            }
                            else
                            {
                                return Visibility.Visible;
                            }
                        }
                        break;
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