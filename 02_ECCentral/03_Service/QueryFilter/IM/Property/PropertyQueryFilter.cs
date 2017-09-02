using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.IM;

namespace ECCentral.QueryFilter.IM
{
    public class PropertyQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }


        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }


        /// <summary>
        /// 属性状态
        /// </summary>
        public PropertyStatus? Status { get; set; }

    }
}
