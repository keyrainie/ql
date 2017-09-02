using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.RMA.Restful.RequestMsg
{
    public class SendDunEmailReq
    {
        /// <summary>
        /// 发送邮件次数
        /// </summary>
        public int SendMailCount { get; set; }

        /// <summary>
        /// 送修系统编号
        /// </summary>
        public int OutboundSysNo { get; set; }

        /// <summary>
        /// 单件编号
        /// </summary>
        public int RegisterSysNo { get; set; }

        /// <summary>
        /// SO编号
        /// </summary>
        public int SOSysNo { get; set; }
    }
}
