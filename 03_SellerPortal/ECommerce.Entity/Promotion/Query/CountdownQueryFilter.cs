using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;

namespace ECommerce.Entity.Promotion
{
    public class CountdownQueryFilter: QueryFilter
    {
        
        public string PromotionTitle
        {
            get;
            set;
        }

       

        /// <summary>
        /// 促销开始时间区间从
        /// </summary>
        public DateTime? CountdownFromStartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 促销开始时间区间到
        /// </summary>
        public DateTime? CountdownToStartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 促销结束时间区间从
        /// </summary>
        public DateTime? CountdownFromEndTime
        {
            get;
            set;
        }

        /// <summary>
        /// 促销结束时间区间到
        /// </summary>
        public DateTime? CountdownToEndTime
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
        /// 单据状态
        /// </summary>
        public int? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 商家系统编号
        /// </summary>
        public int? SellerSysNo
        {
            get;
            set;
        }
    }
}
