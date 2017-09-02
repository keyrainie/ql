using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.Entity.Product;

namespace ECommerce.Entity.Category
{
    [Serializable]
    [DataContract]
    public class CategoryBrand
    {
        public CategoryInfo Category { get; set; }
        public List<BrandInfoExt> BrandInfos { get; set; }
    }
}
