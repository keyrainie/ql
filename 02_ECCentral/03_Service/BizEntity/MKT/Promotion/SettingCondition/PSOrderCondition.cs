using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT 
{
    /// <summary>
    /// 活动订单需要满足的条件
    /// </summary>
    public class PSOrderCondition
    {
        /// <summary>
        /// 支付方式SysNo
        /// </summary>
        public List<int> PayTypeSysNoList { get; set; }

        /// <summary>
        /// 配置方式SysNo
        /// </summary>
        public List<int> ShippingTypeSysNoList { get; set; }

        /// <summary>
        /// 订单金额下限
        /// </summary>
        public decimal? OrderMinAmount { get; set; }

 


    }
}
