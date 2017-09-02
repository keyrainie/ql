using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    /// <summary>
    /// 简洁的商品对象实体
    /// </summary>
    public class SimpleItemEntity
    {
        public int ProductSysNo { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public int Weight { get; set; }
        public string DefaultImage { get; set; }
        public int BrandSysNo { get; set; }
        public int C3SysNo { get; set; }
        public int MerchantSysNo { get; set; }


    }
}
