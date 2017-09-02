using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
    public class ProductSimpleQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? ProductSysNo
        {
            get;
            set;
        }

        public string ProductID
        {
            get;
            set;
        }
 
        public string ProductName
        {
            get;
            set;
        }

        public string CompanyCode { 
            get; 
            set; 
        }

    }
}
