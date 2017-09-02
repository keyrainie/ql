using ECommerce.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Store.Vendor
{
    public class ManufacturerQueryFilter : QueryFilter
    {
        /// <summary>
        /// 生产商ID
        /// </summary>
        public string ManufacturerID { get; set; }

        /// <summary>
        /// 生产商本地化名称
        /// </summary>
        public string ManufacturerNameLocal { get; set; }

        /// <summary>
        /// 生产商国际化名称
        /// </summary>
        public string ManufacturerNameGlobal { get; set; }

        /// <summary>
        /// 生产商状态
        /// </summary>
        public ManufacturerStatus? Status { get; set; }
    }
}
