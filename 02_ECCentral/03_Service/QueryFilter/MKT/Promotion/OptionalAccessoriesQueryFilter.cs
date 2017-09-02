using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class OptionalAccessoriesQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? SysNo { get; set; }

        public List<int> SysNoList { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public string ChannelID { get; set; }

        /// <summary>
        /// 商家系统编号
        /// </summary>
        public int? MerchantSysNo { get; set; }

        /// <summary>
        /// 套餐活动状态
        /// </summary>
        public ComboStatus? Status { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 产品经理
        /// </summary>
        public int? PM { get; set; }

        /// <summary>
        /// 套餐名称
        /// </summary>
        public string SaleRuleName { get; set; }

        /// <summary>
        /// 创建人系统编号
        /// </summary>
        public int? CreateUserSysNumber { get; set; }

        /// <summary>
        /// 主、次商品
        /// </summary>
        public int OptionalAccessoriesItemType { get; set; }

        public int Category1SysNo { get; set; }
        public int Category2SysNo { get; set; }
        public int Category3SysNo { get; set; }
    }
}
