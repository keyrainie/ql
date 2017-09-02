using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Inventory
{
    public class ShiftRequestItemBasketQueryFilter
    {

        public PagingInfo PagingInfo { get; set; }
        /// <summary>
        /// 移仓员名称
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 移入仓编号
        /// </summary>
        public string ShiftInStockSysNo { get; set; }

        /// <summary>
        /// 移出仓编号
        /// </summary>
        public string ShiftOutStockSysNo { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode { get; set; }
    }
}
