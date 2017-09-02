using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.RMA.BizProcessor;

namespace ECCentral.Service.RMA.AppService
{
    [VersionExport(typeof(OutBoundAppService))]
    public class OutBoundAppService
    {
        /// <summary>
        /// 发送催讨邮件
        /// </summary>
        /// <param name="OutboundSysNo">OutBound系统编号</param>
        /// <param name="RegisterSysNo">单件系统编号</param>
        /// <param name="SendMailCount">邮件次数</param>
        public virtual void SendDunEmail(int OutboundSysNo, int RegisterSysNo, int SendMailCount, int SOSysNo)
        {
            ObjectFactory<OutBoundProcessor>.Instance.SendDunEmail(OutboundSysNo, RegisterSysNo, SendMailCount, SOSysNo);
        }
    }
}
