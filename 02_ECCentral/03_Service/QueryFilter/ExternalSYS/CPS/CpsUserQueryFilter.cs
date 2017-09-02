using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.ExternalSYS
{
    public class CpsUserQueryFilter
    {


        public PagingInfo PageInfo { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public AuditStatus? AuditStatus { get; set; }

        /// <summary>
        /// 账户类型
        /// </summary>
        public UserType? UserType { get; set; }

        /// <summary>
        /// 网站类型
        /// </summary>
        public string WebSiteType { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public IsActive? IsActive { get; set; }

        /// <summary>
        /// 账户ID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 收款人姓名
        /// </summary>
        public string ReceivablesName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// QQ/MSN 
        /// </summary>
        public string ImMessenger { get; set; }

        /// <summary>
        /// 注册日期从
        /// </summary>
        public DateTime? RegisterDateFrom { get; set; }

        /// <summary>
        /// 注册日期到
        /// </summary>
        public DateTime? RegisterDateTo { get; set; }

       

    }
}
