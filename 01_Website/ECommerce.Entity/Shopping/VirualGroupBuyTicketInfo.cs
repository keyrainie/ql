using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Shopping
{
    /// <summary>
    /// 虚拟团购团购券生成信息
    /// </summary>
    public class VirualGroupBuyTicketInfo
    {
        /// <summary>
        /// 团购券数量
        /// </summary>
        public int TicketCount { get; set; }

        /// <summary>
        /// 团购标题
        /// </summary>
        public string GroupBuyingTitle { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime AvailableDate { get; set; }

        /// <summary>
        /// 商家电话
        /// </summary>
        public string VendorStoreTel { get; set; }

        /// <summary>
        /// 用户电话
        /// </summary>
        public string CustomerTel { get; set; }
    }

    /// <summary>
    /// 虚拟团购团购券支付信息
    /// </summary>
    public class GroupBuyTicketPayInfo
    {
        public int OrderSysNo { get; set; }
        public int PayTypeID { get; set; }
        public string PayTypeName { get; set; }
        public decimal Amounts { get; set; }
    }
}
