using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.QueryFilter.Inventory
{
    public class ConvertRequestQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? SysNo { get; set; }

        /// <summary>
        ///  单据编号
        /// </summary>
        public string RequestID { get; set; }

        /// <summary>
        /// 转换商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }
        
        /// <summary>
        /// 渠道仓库编号
        /// </summary>
        public int? StockSysNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ConvertRequestStatus? RequestStatus { get; set; }

        /// <summary>
        /// 创建时间 - 从
        /// </summary>
        public DateTime? CreateDateFrom { get; set; }

        /// <summary>
        /// 创建时间 -到
        /// </summary>
        public DateTime? CreateDateTo { get; set; }

        public string CompanyCode { get; set; }

        public BizEntity.Common.PMQueryType? PMQueryRightType { get; set; }

        public int? UserSysNo { get; set; }

    }
}
