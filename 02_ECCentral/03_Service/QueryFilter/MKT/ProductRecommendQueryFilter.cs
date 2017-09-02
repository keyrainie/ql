using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    /// <summary>
    /// 商品推荐查询条件
    /// </summary>
    public class ProductRecommendQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public string CompanyCode { get; set; }

        public string ChannelID { get; set; }

        /// <summary>
        /// 页面类型ID
        /// </summary>
        public int? PageType
        {
            get;
            set;
        }

        /// <summary>
        /// 页面ID
        /// </summary>
        public int? PageID
        {
            get;
            set;
        }

        /// <summary>
        /// 位置编号
        /// </summary>
        public int? PositionID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 生效开始时间from
        /// </summary>
        public DateTime? BeginDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 生效开始时间to
        /// </summary>
        public DateTime? BeginDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 失效结束日期from
        /// </summary>
        public DateTime? EndDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 失效结束日期to
        /// </summary>
        public DateTime? EndDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID
        {
            get;
            set;
        }

        /// <summary>
        /// 商品状态
        /// </summary>
        public ProductStatus? ProductStatus { get; set; }
    }
}
