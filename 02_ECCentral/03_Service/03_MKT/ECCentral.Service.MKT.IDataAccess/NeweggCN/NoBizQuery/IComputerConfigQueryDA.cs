using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IComputerConfigQueryDA
    {
        DataTable QueryMaster(ComputerConfigQueryFilter filter, out int totalCount);

        List<ComputerConfigMaster> GetComputerConfigMasterList();
    }
}
