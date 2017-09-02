using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity
{
    public class SEOInfo
    {
        public int SysNo { get; set; }
        public int PageID { get; set; }
        public int PageType { get; set; }
        public string PageTitle { get; set; }
        public string PageDescription { get; set; }
        public string PageKeywords { get; set; }
        public string PageAdditionContent { get; set; }
    }
}
