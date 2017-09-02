using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility.DataAccess.SearchEngine;
using ECommerce.Utility;

namespace ECommerce.Entity.Promotion
{
    public class GroupBuyingQueryFilter : QueryFilter
    {

        /// <summary>
        /// 所属商家
        /// </summary>
        public int? SellerSysNo { get; set; }

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
        public string Status { get; set; }

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

        /// <summary>
        /// 团购标题
        /// </summary>
        public string GroupBuyingTitle { get; set; }
    }
}
