using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.Common
{
    public class ShipTypeQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 配送方式ID
        /// </summary>
        public string ShipTypeID { get; set; }

        public int? SysNo { get; set; }

        /// <summary>
        /// 本地仓库发货
        /// </summary>
        public int? StockSysNo { get; set; }

        /// <summary>
        /// 前台显示
        /// </summary>
        public HYNStatus? IsOnlineShow { get; set; }

        /// <summary>
        ///收取包裹费
        /// </summary>
        public SYNStatus? IsWithPackFee { get; set; }

        /// <summary>
        ///配送方式名称
        /// </summary>
        public string ShipTypeName { get; set; }

        public string CompanyCode { get; set; }
    }
}
