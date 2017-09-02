using System;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
    public class ProductERPCategoryQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        ///// <summary>
        ///// 大类码ID
        ///// </summary>
        //public string SP_ID { get; set; }

        /// <summary>
        /// 大类码Code
        /// </summary>
        public string SPCode { get; set; }
        /// <summary>
        /// 大类码名称
        /// </summary>
        public string SPName { get; set; }
    }

}
