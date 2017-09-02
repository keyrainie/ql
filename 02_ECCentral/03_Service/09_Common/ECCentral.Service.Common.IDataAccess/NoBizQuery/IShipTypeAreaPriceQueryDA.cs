using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using System.Data;

namespace ECCentral.Service.Common.IDataAccess
{
   public interface IShipTypeAreaPriceQueryDA
    {
        /// <summary>
        /// 查询配送方式-地区-价格
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryShipTypeAreaPriceList(ShipTypeAreaPriceQueryFilter filter, out int totalCount);
    }
}
