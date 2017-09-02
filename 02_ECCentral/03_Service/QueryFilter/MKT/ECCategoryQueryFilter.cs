using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class ECCategoryQueryFilter
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
        /// 前台1级分类系统编号
        /// </summary>
        public int? C1SysNo { get; set; }

        /// <summary>
        /// 前台2级分类系统编号
        /// </summary>
        public int? C2SysNo { get; set; }

        /// <summary>
        /// 前台3级分类系统编号
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 分类状态
        /// </summary>
        public ADStatus? Status { get; set; }
    }
}
