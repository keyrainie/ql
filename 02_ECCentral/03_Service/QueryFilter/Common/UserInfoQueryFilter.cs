using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.Common
{
    public class UserInfoQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// 商场公司编号
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 人员状态
        /// </summary>
        public BizOperationUserStatus? Status
        {
            get;
            set;
        }
    }
}
