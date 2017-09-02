using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.SO;

namespace ECCentral.Service.SO.IDataAccess
{
    public interface ISOLogisticDA
    {
        DataTable QueryDiffSODelivery(SODeliveryDiffFilter filter, out int totalCount);

        int MarkDeliveryExp(int orderSysNo, int orderType, string companyCode, int opUser);

    }
}
