using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{
    /// <summary>
    /// 用户留言(网站，App都可以使用)
    /// </summary>
    public class CustomerLeaveWords : EntityBase
    {
        /// <summary>
        /// 留言主題
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 內容
        /// </summary>
        public string LeaveWords { get; set; }
        
        /// <summary>
        /// 相关订单号
        /// </summary>
        public int? SoSysno { get; set; }

        public int CustomerSysNo { get; set; }

        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }

        public string ReplyContent { get; set; }
    }
}
