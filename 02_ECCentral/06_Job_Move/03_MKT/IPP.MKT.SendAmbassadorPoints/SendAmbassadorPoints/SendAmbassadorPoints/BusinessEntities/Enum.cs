using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using System.ComponentModel;

namespace IPP.ECommerceMgmt.SendAmbassadorPoints.BusinessEntities
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
    [Serializable]
    public enum OrderStatus : int
    {
        [Description("系统自动作废")]
        SystemCancel = -4,
        [Description("主管作废")]
        ManagerCancel = -3,
        [Description("客户作废")]
        CustomerCancel = -2,
        [Description("新蛋作废")]
        EmployeeCancel = -1,
        [Description("待审核")]
        Origin = 0,
        [Description("待出库")]
        WaitingOutStock = 1,
        [Description("待支付")]
        WaitingPay = 2,
        [Description("待主管审")]
        WaitingManagerAudit = 3,
        [Description("已出库")]
        OutStock = 4
    }
}
