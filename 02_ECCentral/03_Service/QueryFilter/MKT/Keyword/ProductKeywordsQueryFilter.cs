using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    /// <summary>
    /// 品页面关键字 
    /// </summary>
    public class ProductKeywordsQueryFilter
    {
        public PagingInfo PageInfo { get; set; }


        public int? Category1SysNo  { get; set; }
        public int? Category2SysNo  { get; set; }
        public int? Category3SysNo { get; set; }

        public string ProductID { get; set; }


        public int? PMUserSysNo { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 产品状态操作符
        /// </summary>
        public string StatusOP { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string ProductMode { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public ECCentral.BizEntity.IM.ProductStatus? Status { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 生产商
        /// </summary>
        public int? SelectedManufacturerSysNo { get; set; }

        public string EditUser { get; set; }

        public int? PropertySysNo { get; set; }

        public int? PropertyValueSysNo { get; set; }

        public string InputValue { get; set; }
        /// <summary>
        /// 更新时间开始
        /// </summary>
        public DateTime? EditDateFrom { get; set; }

        /// <summary>
        /// 更新时间结束
        /// </summary>
        public DateTime? EditDateTo { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }

        public string CompanyCode { get; set; }
    }
}
