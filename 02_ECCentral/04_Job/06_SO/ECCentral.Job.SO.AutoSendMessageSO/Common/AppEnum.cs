using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ECCentral.Job.SO.AutoSendMessageSO.Common
{
   public  enum  FPStatus:int
    {
        [Description("Normal")]
        Normal = 0,
        [Description("Suspect")]
        Suspect = 1,
        [Description("ChuanHuo")]
        ChuanHuo = 2,
        [Description("ChaoHuo")]
        ChaoHuo = 3

    }
}
