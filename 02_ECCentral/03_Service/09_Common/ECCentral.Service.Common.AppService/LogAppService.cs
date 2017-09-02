using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.BizProcessor;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(LogAppService))]
    public class LogAppService
    {
        public virtual int CreateOperationLog(string note, BizLogType logType, int ticketSysNo, string companyCode)
        {
            return ObjectFactory<LogProcessor>.Instance.CreateOperationLog(note, logType, ticketSysNo, companyCode, ServiceContext.Current.ClientIP);
        }
    }
}
