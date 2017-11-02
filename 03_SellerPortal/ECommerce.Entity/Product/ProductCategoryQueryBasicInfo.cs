using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ProductCategoryQueryBasicInfo
    {
        public int SysNo { get; set; }

        public string ProductSysNo { get; set; }

        public string ProductName { get; set; }

        public string Quantity { get; set; }

        public string InDate { get; set; }

        public string InUser { get; set; }

        public string EditDate { get; set; }

        public string EditUser { get; set; }

        public int SellerID { get; set; }
    }
}
