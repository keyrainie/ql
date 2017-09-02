using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.MKT
{
    public class HelpCenterCategoryQueryFilter
    {
        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public string ChannelID { get; set; }

        public string Status { get; set; }
    }
}
