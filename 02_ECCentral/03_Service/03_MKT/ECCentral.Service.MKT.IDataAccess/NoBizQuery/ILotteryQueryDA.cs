using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.MKT;
using System.Data;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface ILotteryQueryDA
    {
        DataSet Query(LotteryQueryFilter filter, out int totalCount);
    }
}
