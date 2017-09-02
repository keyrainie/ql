using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.SO
{
    public class SOItemUpdateInfo
    {
        public int SysNo { get; set; }
        /// <summary>
        /// Price
        /// </summary>
        public decimal Price { get; set; } 
        public decimal TariffAmt { get; set; }
        /// <summary>
        /// 调整值OriginalPrice，Price值通过它进行计算
        /// </summary>
        public decimal OriginalPrice { get; set; }
    }
}
