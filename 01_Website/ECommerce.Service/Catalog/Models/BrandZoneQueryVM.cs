using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Facade.Catalog
{
    public class BrandZoneQueryVM
    {
        public string Keyword { get; set; }

        public int BrandSysNo { get; set; }

        public string SubCategoryEnID { get; set; }

        public int PageNumber { get; set; }

        public int SortMode { get; set; }
    }
}
