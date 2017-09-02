using System;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;


namespace ECCentral.QueryFilter.MKT
{
    public class InternetKeywordQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKeyword { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public IsDefaultStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 创建开始日期
        /// </summary>
        public DateTime? BeginDate
        {
            get;
            set;
        }

        /// <summary>
        /// 创建结束日期
        /// </summary>
        public DateTime? EndDate
        {
            get;
            set;
        }
    }
}
