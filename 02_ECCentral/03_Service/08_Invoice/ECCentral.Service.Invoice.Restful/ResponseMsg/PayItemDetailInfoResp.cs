using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.Restful.ResponseMsg
{
    public class PayDetailInfoResp
    {
        public TotalInfo TotalInfo
        {
            get;
            set;
        }

        public OrderInfo OrderInfo
        {
            get;
            set;
        }

        public List<PayItemInfo> PayItemList
        {
            get;
            set;
        }
    }

    public class TotalInfo
    {
        public decimal? TotalAmt
        {
            get;
            set;
        }

        public decimal? PaidAmt
        {
            get;
            set;
        }

        public decimal? OrderAmt
        {
            get;
            set;
        }

        public int? OrderSysNo
        {
            get;
            set;
        }
    }

    public class OrderInfo
    {
        public PayItemStyle? PayStyle
        {
            get;
            set;
        }

        public decimal? OrderAmt
        {
            get;
            set;
        }

        public int? OrderSysNo
        {
            get;
            set;
        }

        public string OrderID
        {
            get;
            set;
        }

        public PayableOrderType? OrderType
        {
            get;
            set;
        }

        public int? PaySysNo
        {
            get;
            set;
        }

        public int? OrderStatus
        {
            get;
            set;
        }

        public int? BatchNumber
        {
            get;
            set;
        }

        public bool? IsVendorHoldedControl
        {
            get;
            set;
        }
    }
}