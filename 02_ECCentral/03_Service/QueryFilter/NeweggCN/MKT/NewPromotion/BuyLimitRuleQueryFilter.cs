using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    public class BuyLimitRuleQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 限购类型,0-单品限购，1-套餐限购
        /// </summary>
        public int? LimitType { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        public string ProductID { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ComboSysNo { get; set; }

        /// <summary>
        /// 套餐名称
        /// </summary>
        public string ComboName { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 状态：0-无效，1-有效
        /// </summary>
        public int? Status { get; set; }

        public string CompanyCode { get; set; }

        public string ChannelID { get; set; }
    }
}
