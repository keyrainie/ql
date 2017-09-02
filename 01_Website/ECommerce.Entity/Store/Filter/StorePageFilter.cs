using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Store.Filter
{
    public class StorePageFilter
    {
        public StorePageFilter()
        {
            IsPreview = false;
        }
        public int? SellerSysNo { get; set; }
        public int? PublishPageSysNo { get; set; }
        public string PageType { get; set; }
        public bool IsPreview { get; set; }
    }
}
