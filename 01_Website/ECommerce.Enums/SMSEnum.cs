using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{

    public enum SMSStatus
    {
        /// <summary>
        /// 待发送
        /// </summary>
        [Description("待发送")]
        NoSend = 0,

        /// <summary>
        /// 已发送
        /// </summary>
        [Description("已发送")]
        Sended = 1
    }



    public enum SMSType
    {
        [Description("验证手机号")]
        VerifyPhone = 0,
        [Description("支付")]
        Payment = 1,
        [Description("找回密码")]
        FindPassword = 2,
        [Description("修改密码提示")]
        AlertUpdatePassword = 3,
        [Description("虚拟团购")]
        VirualGroupBuy = 4
    }

}
