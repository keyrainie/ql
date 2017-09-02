using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Common
{
    public class SMSInfo
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string CellPhoneNum { get; set; }

        /// <summary>
        /// 短信内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 发送优先级:1(L)～5(H)
        /// </summary>
        public int Priority { get; set; }
    }
}
