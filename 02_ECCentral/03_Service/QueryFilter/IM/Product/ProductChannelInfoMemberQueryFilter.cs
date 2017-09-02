using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;
using System;

namespace ECCentral.QueryFilter.IM
{
    public class ProductChannelInfoMemberQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 渠道编号
        /// </summary>
        public int ChannelSysNo { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get; set; }

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
        /// 开始时间
        /// </summary>
        public DateTime? StartDay { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDay { get; set; }
    }
}
