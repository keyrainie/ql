using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    public class SaleAdvTemplateQueryFilter
    {
        public int? SysNo { get; set; }

        public string CreateUser { get; set; }

        public DateTime? CreateDateFrom { get; set; }

        public DateTime? CreateDateTo { get; set; }

        public SaleAdvStatus? Status { get; set; }
			
        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>        
        public string ChannelID { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
