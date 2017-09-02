using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class ComputerConfigQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public string ChannelID { get; set; }

        public string ComputerConfigName { get; set; }

        public int? ComputerConfigType { get; set; }

        public ComputerConfigStatus? Status { get; set; }

        public ComputerConfigOwner? Owner { get; set; }

        public int? ProductSysNo { get; set; }

        public string ProductID { get; set; }

        public decimal? MinPriceRange { get; set; }

        public decimal? MaxPriceRange { get; set; }

        public int? Priority { get; set; }

        public string EditUser { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }
    }
}
