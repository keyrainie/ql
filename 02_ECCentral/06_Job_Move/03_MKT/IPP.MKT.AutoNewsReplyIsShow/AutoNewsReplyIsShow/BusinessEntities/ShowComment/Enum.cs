using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using System.ComponentModel;

namespace IPP.CN.ECommerceMgmt.AutoCommentShow.BusinessEntities
{

    [Serializable]
    public enum BlockedServiceType
    {
        [Description("OZZO奥硕物流不可用")]
        NeweggDelivery,
        [Description("该周日OZZO奥硕物流可用")]
        CanNeweggDeliverySunday,
        [Description("OZZO奥硕物流一天一送")]
        OneTimeOneDay,
        [Description("法定节假日")]
        NormalHoliday
    }

    [Serializable]
    public enum Status : int
    {
        [Description("手工屏蔽")]
        HandShield = -2,
        [Description("系统屏蔽")]
        SystemShield = -1,
        [Description("展示")]
        Show = 0
    }
}
