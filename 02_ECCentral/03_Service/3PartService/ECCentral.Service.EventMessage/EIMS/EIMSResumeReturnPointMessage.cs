using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.EIMS
{
    public class EIMSResumeReturnPointMessage : IEventMessage
    {
        //是否撤销扣减积分
        public bool IsComeBackPoint { get; set; }

        public int VendorSettleSysNo { get; set; }
        public int PM_ReturnPointSysNo { get; set; }
        public decimal UsingReturnPoint { get; set; }
        public int AuditUserSysNo { get; set; }
        public string AuditUser { get; set; }
        public string SettleID { get; set; }

        public string Error { get; set; }
        public bool IsSucceed { get; set; }

        public string Subject
        {
            get { return "EIMSResumeReturnPointMessage"; }
        }
    }
}
