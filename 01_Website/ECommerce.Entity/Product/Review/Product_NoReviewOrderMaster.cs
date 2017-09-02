using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Product.Review
{
    public class Product_NoReviewOrderMaster
    {
        public int SOSysNo { get; set; }

        public SOType? SOType { get; set; }

        public DateTime OrderDate { get; set; }

        public List<Product_ReviewSimpleProductInfo> NoReviewOrderProducts { get; set; }
    }

    public class Product_ReviewSimpleProductInfo
    {
        public int SOSysNo { get; set; }

        public int SysNo { get; set; }

        public string ProductID { get; set; }

        public string ProductTitle { get; set; }
    }

}
