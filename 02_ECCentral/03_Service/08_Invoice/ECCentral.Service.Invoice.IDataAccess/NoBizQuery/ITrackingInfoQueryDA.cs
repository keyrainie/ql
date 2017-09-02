using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface ITrackingInfoQueryDA
    {
        DataSet QueryTrackingInfo(TrackingInfoQueryFilter filter, out int totalCount);

        DataTable QueryResponsibleUser(ResponsibleUserQueryFilter filter, out int totalCount);

        DataTable QueryOrder(OrderQueryFilter filter, out int totalCount);

        List<CodeNamePair> GetResponsibleUsers(string companyCode);
    }
}