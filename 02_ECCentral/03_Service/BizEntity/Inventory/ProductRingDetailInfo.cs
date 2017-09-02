using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 库存报警信息
    /// </summary>
    public class ProductRingDetailInfo
    {
        public int BrandSysNo { get; set; }

        public int C3SysNo { get; set; }

        public int ProductSysNo { get; set; }

        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public string BatchNumber { get; set; }

        public int ActualQty { get; set; }

        public int LeftRingDays { get; set; }

        public DateTime ExpDate { get; set; }

        public string Email { get; set; }
    }
}
