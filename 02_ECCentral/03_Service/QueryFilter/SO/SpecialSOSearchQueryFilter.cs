using System;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.SO
{
    public class SpecialSOSearchQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        //订单创建时间[起]       
        public DateTime? StartDate
        {
            get;
            set;
        }

        //订单创建时间[至] 
        public DateTime? EndDate
        {
            get;
            set;
        }

        //订单状态
        public SOStatus? SOStatus
        {
            get;
            set;
        }

        //虚库仓      
        public int? StockV
        {
            get;
            set;
        }

        //实库仓     
        public int? StockNV
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }
    }
}
