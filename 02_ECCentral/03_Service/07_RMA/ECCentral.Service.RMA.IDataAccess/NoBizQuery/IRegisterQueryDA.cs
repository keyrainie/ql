using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.RMA;
using ECCentral.BizEntity.RMA;

namespace ECCentral.Service.RMA.IDataAccess.NoBizQuery
{
    public interface IRegisterQueryDA
    {
        DataTable QueryRegister(RegisterQueryFilter request, out int totalCount);

        List<RegisterDunLog> QueryRegisterDunLog(int registerSysNo);
    }
}
