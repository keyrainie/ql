using System;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.SO;

namespace ECCentral.QueryFilter.SO
{
    public class SOInvoiceChangeLogQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        //修改类型
        public InvoiceChangeType ? ChangeType
        {
            get;
            set;
        }

        public DateTime? CreateTimeBegin
        {
            get;
            set;
        }

        public DateTime? CreateTimeEnd
        {
            get;
            set;
        }

        //仓库编号
        public int? StockSysNo
        {
            get;
            set;
        }

        public int? SOSysNo
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
