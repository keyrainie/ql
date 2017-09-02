using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    /// <summary>
    /// 自动确认收款单时查询未确认收款的订单
    /// </summary>
    public class SOQueryFilter
    {
        public string SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 分页信息
        /// </summary>
        public PagingInfo PagingInfo
        {
            get;
            set;
        }
    }
}