using System;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品价格日志
    /// </summary>
    public class PriceChangeLogInfo
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 新价格
        /// </summary>
        public decimal NewPrice { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }

    }
}
