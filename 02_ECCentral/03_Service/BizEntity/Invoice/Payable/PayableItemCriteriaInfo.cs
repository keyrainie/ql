using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice
{
    public class PayableItemCriteriaInfo : ICompany
    {
        public int? SysNo { get; set; }
        public int? PaySysNo { get; set; }
        public PayItemStatus? Status { get; set; }
        public PayItemStyle? PayStyle { get; set; }
        public PayableOrderType? OrderType { get; set; }
        public int? OrderSysNo { get; set; }    

        #region ICompany Members

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
        #endregion ICompany Members
    }
}
