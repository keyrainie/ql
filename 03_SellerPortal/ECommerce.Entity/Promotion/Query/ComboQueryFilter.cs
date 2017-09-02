using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;

namespace ECommerce.Entity.Promotion
{
    public class ComboQueryFilter: QueryFilter
    {
        /// <summary>
        /// 商家系统编号
        /// </summary>
        public int? SellerSysNo { get; set; }

        /// <summary>
        /// 套餐活动状态
        /// </summary>
        public int? Status
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
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 套餐名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 规则编号
        /// </summary>
        public int? SysNo { get; set; }
    }
}
