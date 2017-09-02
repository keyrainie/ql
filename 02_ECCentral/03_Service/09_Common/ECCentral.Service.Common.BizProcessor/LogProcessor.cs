using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.IDataAccess;
using System.Threading;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(LogProcessor))]
    public class LogProcessor
    {
        public virtual int CreateOperationLog(string note, BizLogType logType, int ticketSysNo, string companyCode, string ipAddress)
        {
            return ObjectFactory<ILogDA>.Instance.InsertOperationLog(note, logType, ticketSysNo, companyCode, ipAddress);
        }
    }
}
