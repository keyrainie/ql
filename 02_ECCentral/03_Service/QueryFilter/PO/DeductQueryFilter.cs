using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.PO;

namespace ECCentral.QueryFilter.PO
{
    public class DeductQueryFilter
    {
        public DeductQueryFilter()
        {
            PageInfo = new PagingInfo { PageIndex = 0, PageSize = 25 };
        }

        public PagingInfo PageInfo { get; set; }

        public int SysNo { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 扣款项目类型
        /// </summary>
        public DeductType? DeductType { get; set; }
        /// <summary>
        /// 记成本/费用
        /// </summary>
        public AccountType? AccountType { get; set; }
        /// <summary>
        /// 扣款方式
        /// </summary>
        public DeductMethod? DeductMethod { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public Status? Status { get; set; }
    }
}
