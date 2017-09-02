using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{
    public enum CategoryStatus
    {
        [Description("无效")]
        Invalid = -1,
        [Description("有效")]
        Valid = 0
    }
}
