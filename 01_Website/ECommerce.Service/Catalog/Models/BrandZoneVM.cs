using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using ECommerce.Entity.Product;
using ECommerce.Entity.SolrSearch;

namespace ECommerce.Facade.Catalog
{
    public class BrandZoneVM
    {
        public int BrandSysNo { get; set; }

        public ProductSearchResult ProductSearchResult { get; set; }

        public List<ProductItemInfo> HotProductList { get; set; }

        public List<BannerInfo> BannerList { get; set; }

        public BrandInfoExt BrandInfo { get; set; }
    }
}
