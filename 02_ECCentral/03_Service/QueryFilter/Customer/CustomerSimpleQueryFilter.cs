using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.BizEntity.Customer
{
    public class CustomerSimpleQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? CustomerSysNo
        {
            get;
            set;
        }

        public string CustomerID
        {
            get;
            set;
        }
        public int? Status
        {
            get;
            set;
        }

        public string CustomerName
        {
            get;
            set;
        }

        public string CustomerEmail
        {
            get;
            set;
        }

        public string CustomerPhone
        {
            get;
            set;
        }

        public string CompanyCode { get; set; }
    }
}
