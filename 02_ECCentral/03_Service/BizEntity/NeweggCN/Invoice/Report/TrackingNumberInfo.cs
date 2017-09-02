using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice.Report
{
    public class TrackingNumberInfo : IIdentity
    {
        public string OrderID
        {
            get;
            set;
        }

        public string OrderType
        {
            get;
            set;
        }

        public string InvoiceNumber
        {
            get;
            set;
        }

        public string TrackingNumber
        {
            get;
            set;
        }

        public int? StockSysNo
        {
            get;
            set;
        }

        #region IIdentity Members

        public int? SysNo
        {
            get;
            set;
        }

        #endregion IIdentity Members
    }
}