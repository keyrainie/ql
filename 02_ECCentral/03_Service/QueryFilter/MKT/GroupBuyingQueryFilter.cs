using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class GroupBuyingQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public string CompanyCode { get; set; }

        public string ChannelID { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        //public int? GroupBuyingTypeSysNo { get; set; }
        public int? GroupBuyingCategorySysNo { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public int? GroupBuyingAreaSysNo { get; set; }

        /// <summary>
        /// 商家
        /// </summary>
        public int? GroupBuyingVendorSysNo { get; set; }

        /// <summary>
        /// 三级分类系统编号
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 二级分类系统编号
        /// </summary>
        public int? C2SysNo { get; set; }

        /// <summary>
        /// 一级分类系统编号
        /// </summary>
        public int? C1SysNo { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 团购活动状态
        /// </summary>
        public GroupBuyingStatus? Status { get; set; }

        /// <summary>
        /// 团购开始日期范围从
        /// </summary>
        public DateTime? BeginDateFrom { get; set; }

        /// <summary>
        /// 团购开始日期范围到
        /// </summary>
        public DateTime? BeginDateTo { get; set; }

        public DateTime? EndDateFrom { get; set; }

        public DateTime? EndDateTo { get; set; }

        public DateTime? InDateFrom { get; set; }

        public DateTime? InDateTo { get; set; }

        public GroupBuyingCategoryType? CategoryType { get; set; }
    }
}
