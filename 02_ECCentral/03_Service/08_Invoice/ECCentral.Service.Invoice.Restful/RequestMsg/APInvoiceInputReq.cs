using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Invoice.Invoice;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    public class APInvoiceInputReq
    {
        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? VendorSysNo { get; set; }

        public decimal? TaxRate { get; set; }
        /// <summary>
        /// item编号（三种方式：单一、以'.'分隔，以'-'分隔）
        /// </summary>
        public string ItemsNoList { get; set; }
        /// <summary>
        /// 发票日期
        /// </summary>
        public DateTime? InvoiceDate { get; set; }
        /// <summary>
        /// 发票金额
        /// </summary>
        public decimal? InvoiceAmt { get; set; }
        ///// <summary>
        ///// PO列表
        ///// </summary>
        //public List<APInvoicePOItemInfo> POItemList { get; set; }
        ///// <summary>
        ///// Invoice列表
        ///// </summary>
        //public List<APInvoiceInvoiceItemInfo> InvoiceItemList { get; set; }

        public PayableOrderType? OrderType { get; set; }
        /// <summary>
        /// PO单起始日期
        /// </summary>
        public DateTime? PODateFrom { get; set; }

        public string CompanyCode { get; set; }
    }
}
