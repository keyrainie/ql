using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    public class ProducCommonQueryFilter : QueryFilter
    {
        public string ProductID { get; set; }
        public string ProductTitle { get; set; }
        public string Status { get; set; }
        public string FrontCategorySysNo { get; set; }
        public DateTime CreateTimeBegin { get; set; }
        public DateTime CreateTimeEnd { get; set; }
        public string GroupName { get; set; }
        public string VendorSysNo { get; set; }
        public string BrandSysNo { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string ProductTradeType { get; set; }
    }
}
