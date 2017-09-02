using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Customer
{
    public class QCSubjectFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public string ChannelID { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        public QCSubjectStatus? Status { get; set; }
    }
}
