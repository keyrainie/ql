using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Payment
{
    /// <summary>
    /// 关务对接相关信息
    /// </summary>
    public class VendorCustomsInfo
    {
        public int SysNo { get; set; }
        public int MerchantSysNo { get; set; }
        public string CBTMerchantCode { get; set; }
        public string CBTMerchantName { get; set; }
        public string CBTSRC_NCode { get; set; }
        public string CBTREC_NCode { get; set; }
        public string EasiPaySecretKey { get; set; }
        public string ReceiveCurrencyCode { get; set; }
        public string PayCurrencyCode { get; set; }
        public string CBTSODeclareSecretKey { get; set; }
        public string CBTProductDeclareSecretKey { get; set; }
    }
}
