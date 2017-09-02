using System;
using System.Collections.Generic;
using ECCentral.Portal.UI.Invoice.Models;

namespace ECCentral.Portal.UI.Invoice.Utility
{
    /// <summary>
    /// 判断是否是同一张发票的比较器
    /// </summary>
    public class InvoiceEqualityComparer : IEqualityComparer<InvoiceVM>
    {
        #region IEqualityComparer<InvoiceVM> Members

        public bool Equals(InvoiceVM x, InvoiceVM y)
        {
            return (x.SysNo == y.SysNo) && (x.StockID == y.StockID);
        }

        public int GetHashCode(InvoiceVM obj)
        {
            return obj.GetHashCode() ^ 2;
        }

        #endregion IEqualityComparer<InvoiceVM> Members
    }
}