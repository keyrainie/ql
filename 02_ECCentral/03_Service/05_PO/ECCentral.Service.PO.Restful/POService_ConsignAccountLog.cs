using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.PO;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {
        [WebInvoke(UriTemplate = "/Vendor/QueryConsignToAccountLog", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryConsignToAccountLogList(ConsignToAccountLogQueryFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IConsignToAccountLogQueryDA>.Instance.QueryConsignToAccountLog(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }


        [WebInvoke(UriTemplate = "/Vendor/QueryConsignToAccountLogTotalAmt", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryConsignToAccountLogTotalAmt(ConsignToAccountLogQueryFilter request)
        {

            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IConsignToAccountLogQueryDA>.Instance.QueryConsignToAccountLogTotalAmt(request)
            };
            returnResult.TotalCount = returnResult.Data.Rows.Count;
            return returnResult;
        }
    }
}
