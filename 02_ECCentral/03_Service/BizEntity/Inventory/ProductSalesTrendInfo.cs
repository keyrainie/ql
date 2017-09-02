using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.Inventory
{
    public class ProductSalesTrendInfo : IIdentity
    {
        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        #endregion IIdentity Members

        public int? ProductSysNo { get; set; }

        public StockInfo Stock { get; set; }

        public WarehouseInfo Warehouse { get; set; }

        #region 前7天的销售量

        public int? D1 { get; set; }
        
        public int? D2 { get; set; }

        public int? D3 { get; set; }

        public int? D4 { get; set; }

        public int? D5 { get; set; }

        public int? D6 { get; set; }

        public int? D7 { get; set; }

        #endregion 前7天的销售量


        #region 前7周的销售量

        public int? W1 { get; set; }

        public int? W2 { get; set; }

        public int? W3 { get; set; }

        public int? W4 { get; set; }

        public int? W5 { get; set; }

        public int? W6 { get; set; }

        public int? W7 { get; set; }

        #endregion 前7周的销售量


        #region 前7月的销售量

        public int? M1 { get; set; }

        public int? M2 { get; set; }

        public int? M3 { get; set; }

        public int? M4 { get; set; }

        public int? M5 { get; set; }

        public int? M6 { get; set; }

        public int? M7 { get; set; }

        #endregion 前7月的销售量

    }
}
