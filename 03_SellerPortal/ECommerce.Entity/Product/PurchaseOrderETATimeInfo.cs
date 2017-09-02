using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    public class PurchaseOrderETATimeInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? ETATimeSysNo { get; set; }

        /// <summary>
        /// PO编号
        /// </summary>
        /// 
        public int? POSysNo { get; set; }

        /// <summary>
        /// 预计到货时间(ETATime)
        /// </summary>
        public DateTime? ETATime { get; set; }

        /// <summary>
        /// 预计到货时段 (上午下午)
        /// </summary>
        public PurchaseOrderETAHalfDayType? HalfDay { get; set; }

        /// <summary>
        /// 状态 : (申请，取消，通过)
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 理由
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 创建人系统编号
        /// </summary>
        public int? CreateUserSysNo { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 更新人名称
        /// </summary>
        public string EditUser { get; set; }
    }
}
