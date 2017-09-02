using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(ILogDA))]
    public class LogDA : ILogDA
    {
        public virtual int InsertOperationLog(string note, BizLogType logType, int ticketSysNo, string companyCode, string ipAddress)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertOperationLog");
            dc.SetParameterValueAsCurrentUserSysNo("@OperationUserSysNo");
            dc.SetParameterValue("@OperationIP", ipAddress);
            dc.SetParameterValue("@TicketType", logType);
            dc.SetParameterValue("@TicketSysNo", ticketSysNo);
            dc.SetParameterValue("@Note", note);
            dc.SetParameterValue("@CompanyCode", companyCode);
            dc.SetParameterValue("@StoreCompanyCode", companyCode);
            dc.SetParameterValue("@LanguageCode", "zh-CN");
            int sysNo = dc.ExecuteScalar<int>();
            return sysNo;
        }
    }
}
