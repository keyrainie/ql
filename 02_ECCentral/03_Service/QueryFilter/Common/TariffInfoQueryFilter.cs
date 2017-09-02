using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.Common
{
    public class TariffInfoQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }


        public int? SysNo { get; set; }

        public int? ParentSysNo { get; set; }

        public string ItemCategoryName
        {
            get;
            set;
        }

        public string TariffCode
        {
            get;
            set;
        }


        public string TariffRate
        {
            get;
            set;
        }


        public TariffStatus? Status { get; set; }

        /// <summary>
        /// 有值查有效，无值查所有
        /// </summary>
        public int? SearchCode { get; set; }
    }
}
