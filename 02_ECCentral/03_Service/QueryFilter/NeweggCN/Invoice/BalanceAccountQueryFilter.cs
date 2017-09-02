using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    public class BalanceAccountQueryFilter
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        public int? CustomerSysNo
        {
            get;
            set;
        }

        public string CustomerID
        {
            get;
            set;
        }

        public DateTime? CreateTimeFrom
        {
            get;
            set;
        }

        public DateTime? CreateTimeTo
        {
            get;
            set;
        }

        public BalanceAccountDetailType? DetailType
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }

        public string WebChannelID
        {
            get;
            set;
        }
    }
}