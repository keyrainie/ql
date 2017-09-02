using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    /// <summary>
    /// 公告及促销评论
    /// </summary>
    public class NewsAdvReplyQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 所属SysNo
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 创建时间开始于
        /// </summary>
        public DateTime? InDateFrom { get; set; }

        /// <summary>
        /// 创建时间结束于
        /// </summary>
        public DateTime? InDateTo { get; set; }

        /// <summary>
        /// 顾客SysNo
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 相关处理人员
        /// </summary>
        public int? LastEditUserSysNo { get; set; }

        /// <summary>
        /// 评论类型
        /// </summary>
        public string ReferenceType { get; set; }

        public int? ReferenceSysNo { get; set; }

        /// <summary>
        /// 最后修改人(name)
        /// </summary>
        public string LastEditUserID { get; set; }

        /// <summary>
        /// 标题关键字
        /// </summary>
        public string ReplyContent { get; set; }

        /// <summary>
        /// 是否上传图片
        /// </summary>
        public NYNStatus? IsUploadImage { get; set; }

        /// <summary>
        /// 前台展示状态
        /// </summary>
        public NewsAdvReplyStatus? Status { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 渠道ID
        /// </summary>
        public int? ChannelID { get; set; }
    }
}
