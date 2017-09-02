using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.Service.Utility.WCF;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace ECCentral.Service.ExternalSYS.Restful
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    public partial class ExternalSYSService
    {
        delegate DataTable NoBizQueryListHanding<T>(T filter, out int dataCount);

        QueryResult QueryList<T>(T filter, NoBizQueryListHanding<T> handing) where T : class,new()
        {
            QueryResult result = new QueryResult();
            int dataCount = 0;
            result.Data = handing(filter, out dataCount);
            result.TotalCount = dataCount;
            return result;
        }
    }
}
