using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.EIMS
{
    public class EIMSInvoiceInfoMessage : IEventMessage
    {
        public bool IsPage { get; set; }

        public int VendorSysNo { get; set; }
        public int PMSysNo { get; set; }
        public int InvoiceNumber { get; set; }
        public string ReceiveType { get; set; }
        public string CompanyCode { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string SortField { get; set; }

        /// <summary>
        /// 分页查询时使用
        /// </summary>
        public List<ReturnPointMsg> ResultList { get; set; }
        /// <summary>
        /// 根据sysno查询时使用
        /// </summary>
        public ReturnPointMsg Result { get; set; }

        public string Subject
        {
            get { return "EIMSInvoiceInfoMessage"; }
        }
    }

    public class ReturnPointMsg
    {
        public int SysNo { get; set; }
        public string ReturnPointName { get; set; }
        public decimal ReturnPoint { get; set; }
        public decimal RemnantReturnPoint { get; set; }
    }
}
