using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface ILogDA
    {
        int InsertOperationLog(string note, BizLogType logType, int ticketSysNo, string companyCode, string ipAddress);
    }
}
