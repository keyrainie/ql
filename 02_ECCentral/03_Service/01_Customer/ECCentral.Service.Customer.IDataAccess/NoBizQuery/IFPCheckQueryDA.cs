using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Customer;

namespace ECCentral.Service.Customer.IDataAccess.NoBizQuery
{
    public interface IFPCheckQueryDA
    {
        DataTable Query(FPCheckQueryFilter filter, out int totalCount);
        DataTable QueryCHSet(CHQueryFilter filter, out int totalCount);
        DataTable GetETC(string webChannelID,out int totalCount);
    }
}
