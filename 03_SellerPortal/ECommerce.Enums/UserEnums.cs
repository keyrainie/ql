using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{
    public enum SecondDomainStatus
    {
        [Description("未通过审核")]
        FailedAudit = -1,
        [Description("待审核")]
        ToAudit = 0,
        [Description("通过审核")]
        PassAudit = 1,
        [Description("已生效")]
        Online = 2
    }
    public enum UserStatus
    {
        [Description("有效")]
        Active,
        [Description("无效")]
        DeActive
    }

    public enum RoleStatus
    {
        [Description("有效")]
        Active,
        [Description("无效")]
        DeActive
    }
}
