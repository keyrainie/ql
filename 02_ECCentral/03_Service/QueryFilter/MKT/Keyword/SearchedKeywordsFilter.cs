using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class SearchedKeywordsFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 编辑用户
        /// </summary>
        public string EditUser { get; set; }

        /// <summary>
        /// 编辑时间开始于
        /// </summary>
        public DateTime? InDateFrom { get; set; }

        /// <summary>
        /// 编辑时间结束于
        /// </summary>
        public DateTime? InDateTo { get; set; }

        /// <summary>
        /// 展示状态
        /// </summary>
        public ECCentral.BizEntity.MKT.ADStatus? Status { get; set; }

        /// <summary>
        /// 添加用户类型
        /// </summary>
        public KeywordsOperateUserType? CreateUserType { get; set; }

        public string CompanyCode { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }
    }
}
