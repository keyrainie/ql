using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    public class HeaderDataQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public DateTime? UploadDateFrom { get; set; }

        public DateTime? UploadDateTo { get; set; }

        public DateTime? UploadDate { get; set; }

        public string SapCoCode { get; set; }

        public string DocType { get; set; }
    }
}
