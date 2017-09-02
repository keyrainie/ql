using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    public class SAPVendorQueryFilter
    {
        public PagingInfo PagingInfo
        {
            get;
            set;
        }
        public int? VendorSysNo { get; set; }

        public string CompanyCode { get; set; }
    }

    public class SAPCompanyQueryFilter
    {
        public PagingInfo PagingInfo
        {
            get;
            set;
        }
        /// <summary>
        /// 仓库编号
        /// </summary>
        public int? StockID { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string StockName { get; set; }

        /// <summary>
        /// 公司SAP代码
        /// </summary>
        public string SapCompanyCode { get; set; }

        public string CompanyCode { get; set; }
    }

    public class SAPIPPUserQueryFilter
    {
        public PagingInfo PagingInfo
        {
            get;
            set;
        }
        /// <summary>
        /// 支付方式
        /// </summary>
        public int? PayType { get; set; }
        /// <summary>
        /// SAP客户ID
        /// </summary>
        public string CustID { get; set; }
        /// <summary>
        /// 账户ID
        /// </summary>
        public string SystemConfirmID { get; set; }

        public string CompanyCode { get; set; }
    }
}
