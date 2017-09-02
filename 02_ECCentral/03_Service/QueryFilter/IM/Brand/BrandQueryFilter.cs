using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.IM;

namespace ECCentral.QueryFilter.IM
{
    public class BrandQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? BrandSysNo { get; set; }

        /// <summary>
        /// 品牌ID
        /// </summary>
        public string BrandID { get; set; }

        /// <summary>
        /// 品牌本地化名称
        /// </summary>
        public string BrandNameLocal { get; set; }


        /// <summary>
        /// 品牌国际化名称
        /// </summary>
        public string BrandNameGlobal { get; set; }

        /// <summary>
        /// 生产商SysNo
        /// </summary>
        public int? ManufacturerSysNo { get; set; }

        /// <summary>
        /// 生产商名称
        /// </summary>
        public string ManufacturerName { get; set; }
        /// <summary>
        /// 优先级 
        /// </summary>
        public string Priority { get; set; }
        /// <summary>
        /// 品牌状态
        /// </summary>
        public ValidStatus? Status { get; set; }

        /// <summary>
        /// 是否包含授权
        /// </summary>
        public BooleanEnum? IsAuthorized { get; set; }

        /// <summary>
        /// 是否包含品牌故事
        /// </summary>
        public BooleanEnum? IsBrandStory { get; set; }

        /// <summary>
        /// 类别1SysNo
        /// </summary>
        public int? Category1SysNo { get; set; }

        /// <summary>
        /// 类别2SysNo
        /// </summary>
        public int? Category2SysNo { get; set; }

        /// <summary>
        /// 类别3SysNo
        /// </summary>
        public int? Category3SysNo { get; set; }

        /// <summary>
        /// 授权牌状态
        /// </summary>
        public AuthorizedStatus? AuthorizedStatus { get; set; }
    }
}
