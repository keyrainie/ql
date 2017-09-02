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
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.RMA;

namespace ECCentral.Portal.UI.Invoice.Converters
{
    public class FinanceConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FinanceVM model = value as FinanceVM;
            if (model == null)
            {
                return null;
            }
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }
            switch (parameter.ToString())
            {
                case "OrderStatus":
                    return GetOrderStatusDescription(model.OrderStatus, model.OrderType);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        private string GetOrderStatusDescription(int orderStatus, PayableOrderType? orderType)
        {
            string st = orderStatus.ToString();
            switch (orderType)
            {
                case PayableOrderType.PO:
                case PayableOrderType.POAdjust:
                    st = EnumConverter.GetDescription((PurchaseOrderStatus)orderStatus);
                    break;
                case PayableOrderType.VendorSettleOrder:
                    st = EnumConverter.GetDescription((SettleStatus)orderStatus);
                    break;
                //case PayableOrderType.ReturnPointCashAdjust:
                //case PayableOrderType.SubAccount:
                //case PayableOrderType.SubInvoice:
                //    st = EnumConverter.GetDescription((EIMSInvoiceStatus)orderStatus);
                //    break;
                case PayableOrderType.RMAPOR:
                    st = EnumConverter.GetDescription((VendorRefundStatus)orderStatus);
                    break;
                case PayableOrderType.CollectionSettlement:
                    st = EnumConverter.GetDescription((GatherSettleStatus)orderStatus);
                    break;
                case PayableOrderType.Commission:
                    st = EnumConverter.GetDescription((VendorCommissionMasterStatus)orderStatus);
                    break;
                case PayableOrderType.CollectionPayment:
                    st = EnumConverter.GetDescription((POCollectionPaymentSettleStatus)orderStatus);
                    break;
                default:
                    break;
            }
            return st;
        }
    }
}
