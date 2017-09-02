using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.GiftCard
{
    /// <summary>
    /// 礼品卡消费记录
    /// </summary>
    [Serializable]
    public class UsedRecordQuery : QueryFilterBase
    {
        public UsedRecordQuery()
        {
            this.PageInfo = new PageInfo();
        }

        /// <summary>
        /// 礼品卡卡号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public int CustomerSysNo { get; set; }
    }
}
