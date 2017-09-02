using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class ThesaurusKeywordsQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public ThesaurusWordsType? Type { get; set; }

        /// <summary>
        /// 同义词内容
        /// </summary>
        public string ThesaurusWords { get; set; }

        /// <summary>
        /// 状态      D=无效  A=有效
        /// </summary>
        public ADTStatus? Status { get; set; }

        public string CompanyCode { get; set; }
    }
}
