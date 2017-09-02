using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECommerce.Enums.Promotion
{
    public enum ComboStatus
    {
         //无效 -1,有效 0,待审核 1
        [Description("无效")]
        Deactive = -1,
        [Description("有效")]
        Active = 0,
        [Description("待审核")]
        WaitingAudit=1
    }
}
