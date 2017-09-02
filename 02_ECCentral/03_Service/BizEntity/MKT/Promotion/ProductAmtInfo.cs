using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 订单商品的促销折扣金额信息
    /// </summary>
    public class SOItemAmtInfo
    {
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }        

        /// <summary>
        /// 促销折扣金额
        /// </summary>
        public decimal? PromotionAmount { get; set; }       
    }
}