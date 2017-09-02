using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit
{
    /// <summary>
    /// 自动审单请求体
    /// </summary>
    public class JobAutoAuditSOReq
    {
        public string Interorder { get; set; }

        public string CompanyCode { get; set; }

        public int AutoAuditUserSysNo { get; set; }
    }
}
