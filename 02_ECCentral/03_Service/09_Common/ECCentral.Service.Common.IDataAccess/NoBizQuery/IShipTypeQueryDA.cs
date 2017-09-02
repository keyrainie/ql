using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface IShipTypeQueryDA
    {
        /// <summary>
        /// 查询配送方式
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryShipTypeList(ShipTypeQueryFilter filter, out int totalCount);


    }
}
