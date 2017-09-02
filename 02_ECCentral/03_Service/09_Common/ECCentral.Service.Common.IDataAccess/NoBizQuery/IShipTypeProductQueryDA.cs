using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public  interface IShipTypeProductQueryDA
    {
        /// <summary>
        /// 查询配送方式-产品
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryShipTypeProductList(ShipTypeProductQueryFilter filter, out int totalCount);
    }
}
