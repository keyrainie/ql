using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 代收结算单
    /// </summary>
    public class GatherSettlementInfo : SettleRequestBase
    {
        /// <summary>
        /// 代收结算单状态
        /// </summary>
        public GatherSettleStatus? SettleStatus { get; set; }
        /// <summary>
        /// 代收结算商品列表
        /// </summary>
        public List<GatherSettlementItemInfo> GatherSettlementItemInfoList { get; set; }


        public int? VendorSysNo { get; set; }

        public int? StockSysNo { get; set; }

    }
}
