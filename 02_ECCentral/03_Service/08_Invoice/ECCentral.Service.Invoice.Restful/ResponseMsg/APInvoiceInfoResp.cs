using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice.Invoice;

namespace ECCentral.Service.Invoice.Restful.ResponseMsg
{
    public class APInvoiceInfoResp
    {
        public int? VendorSysNo { get; set; }

        public string VendorName { get; set; }

        public List<APInvoicePOItemInfo> poItemList { get; set; }

        public List<APInvoiceInvoiceItemInfo> invoiceItemList { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMsg { get; set; }
    }
}
