using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
    public class ProductChannelInfoQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 渠道编号
        /// </summary>
        public int ChannelSysNo { get; set; }

        /// <summary>
        /// 一级类
        /// </summary>
        public int? C1SysNo { get; set; }

        /// <summary>
        /// 二级类
        /// </summary>
        public int? C2SysNo { get; set; }

        /// <summary>
        /// 三级类
        /// </summary>
        public int? C3SysNo { get; set; }


        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 渠道商品编号对于数据库SynProductID
        /// </summary>
        public string ChannelProductID { get; set; }

        /// <summary>
        /// 淘宝SKU码
        /// </summary>
        public string TaobaoSKU { get; set; }
    }

    public class ProductChannelPeriodPriceQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 一级类
        /// </summary>
        public int? ChannelProductSysNo { get; set; }

    }
}
