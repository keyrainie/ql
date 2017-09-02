using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface INeweggAmbassadorQueryDA
    {
        DataSet QueryAmbassadorBasicInfo(NeweggAmbassadorQueryFilter filter, out int totalCount);

        DataSet QueryPurchaseOrderInfo(NeweggAmbassadorOrderQueryFilter filter, out int totalCount);

        DataSet QueryRecommendOrderInfo(NeweggAmbassadorOrderQueryFilter filter, out int totalCount);

        DataSet QueryPointInfo(NeweggAmbassadorOrderQueryFilter filter, out int totalCount);
    }
}
