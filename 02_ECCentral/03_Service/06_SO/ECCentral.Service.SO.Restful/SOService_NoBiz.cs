using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.WCF;
using System.ServiceModel.Web;
using ECCentral.QueryFilter.SO;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.AppService;
using ECCentral.Service.Utility;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;
using System.ServiceModel;
using System.Data;

namespace ECCentral.Service.SO.Restful
{
    public partial class SOService
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
