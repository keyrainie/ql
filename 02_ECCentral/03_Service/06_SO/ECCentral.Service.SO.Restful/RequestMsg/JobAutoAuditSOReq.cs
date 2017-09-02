using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.SO.Restful.RequestMsg
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
