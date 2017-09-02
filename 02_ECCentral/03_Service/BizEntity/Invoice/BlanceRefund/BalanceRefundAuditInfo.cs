using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice
{
    public class BalanceRefundAuditInfo : IIdentity
    {

        public int? SysNo { get; set; }

        public int AuditUserSysNo { get; set; }

    }
}
