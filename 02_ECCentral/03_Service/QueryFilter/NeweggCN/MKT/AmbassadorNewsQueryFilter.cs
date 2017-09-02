using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Enum;

namespace ECCentral.QueryFilter.MKT
{
    /// <summary>
    /// 公告查询
    /// </summary>
    public class AmbassadorNewsQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

    
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }


        /// <summary>
        /// 创建时间开始于
        /// </summary>
        public DateTime? InDateFrom { get; set; }

        /// <summary>
        /// 创建时间结束于
        /// </summary>
        public DateTime? InDateFromTo { get; set; }

     

        public string CompanyCode { get; set; }
      
        /// <summary>
        /// 大区
        /// </summary>
        public int? ReferenceSysNo
        {
            get;
            set;
        }
    }
}
