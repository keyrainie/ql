using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.ExternalSYS
{
    public class EIMSInvoiceInfo
    {
        public List<EIMSInvoiceEntryInfo> InvoiceInfoList { get; set; }

        public List<EIMSInvoiceInfoEntity> EIMSList { get; set; }

        public List<EIMSInvoiceInfoEntity> OldEIMSList { get; set; }
    }
}
