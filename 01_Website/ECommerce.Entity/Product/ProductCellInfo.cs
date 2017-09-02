using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Category;

namespace ECommerce.Entity.Product
{
    public class ProductCellInfo : ProductBasicInfo
    {
        public ProductSalesInfo SalesInfo { get; set; }

        public CategoryInfo Category { get; set; }

        public ProductPriceInfo Price { get; set; }

        public int DisplayQuantity { get; set; }
    }
}
