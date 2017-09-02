using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.Common.IDataAccess.NoBizQuery
{

    public partial interface ITariffInfoQueryDA
    {

        DataTable QueryTariffInfo(TariffInfoQueryFilter queryCriteria, out int totalCount);
    }
}
