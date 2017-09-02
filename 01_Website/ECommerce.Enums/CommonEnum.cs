using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ECommerce.Enums
{
    public enum CommonStatus
    {
        [Description("有效")]
        Actived = 1,
        [Description("无效")]
        DeActived = 0
    }
    public enum CommonYesOrNo
    {
        [Description("是")]
        Yes = 1,
        [Description("否")]
        No = 0
    }
    public enum EHaveNo
    {
        [Description("有")]
        Have = 1,

        [Description("无")]
        No = 0
    }

    public enum EYesNo
    {
        [Description("是")]
        Yes = 1,

        [Description("否")]
        No = 0
    }


    public enum ADStatus
    {
        [Description("无效")]
        D,

        [Description("有效")]
        A
    }

    /// <summary>
    /// 发送邮件的类型
    /// </summary>
    public enum EmailType
    {
        [Description("找回密码")]
        FindPassword = 0,
        [Description("验证邮件地址")]
        ValidateEmail = 1,
        [Description("支付")]
        Payment = 2
    }

    public enum EmailStatus
    {
        [Description("已发送")]
        Sent = 1,
        [Description("未发送")]
        NotSend = 0
    }
    
    public enum ImageSize
    {
        P60,
        P80,
        P100,
        P120,
        P130,
        P140,
        P160,
        P200,
        P220,
        P240,
        P450,
        P800
    }

}
