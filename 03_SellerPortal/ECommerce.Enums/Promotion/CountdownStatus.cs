using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECommerce.Enums.Promotion
{
    /// <summary>
    /// 限时促销活动状态
    /// </summary>
    public enum CountdownStatus
    {
        /// <summary>
        /// 初始态
        /// </summary>
        [Description("初始态")]
        Init = -5,
        
        /// <summary>
        /// 审核未通过
        /// </summary>
        [Description("审核未通过")]
        VerifyFaild = -4,
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        WaitForPrimaryVerify = -6,
        /// <summary>
        /// 待高级审核
        /// </summary>
        //[Obsolete("此字段已弃用")]
        //WaitForVerify = -3,
        /// <summary>
        /// 就绪
        /// </summary>
        [Description("就绪")]
        Ready = 0,
        /// <summary>
        /// 运行
        /// </summary>
        [Description("运行")]
        Running = 1,
        /// <summary>
        /// 完成
        /// </summary>
        [Description("已完成")]
        Finish = 2,
        //终止
        //[Description("已终止")]
        //Interupt = -2,
        /// <summary>
        /// 已作废
        /// </summary>
        [Description("已作废")]
        Abandon = -1
    }
}
