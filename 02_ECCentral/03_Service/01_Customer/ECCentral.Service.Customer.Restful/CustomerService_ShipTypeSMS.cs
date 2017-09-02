using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.AppService;

namespace ECCentral.Service.Customer.Restful
{
    public partial class CustomerService
    {
        [WebInvoke(UriTemplate = "/ShipTypeSMS/Query", Method = "POST")]
        public QueryResult QueryShipTypeSMS(ShipTypeSMSQueryFilter query)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<IShipTypeSMSQueryDA>.Instance.Query(query, out totalCount),
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/ShipTypeSMS/Create", Method = "POST")]
        public void CreateShipTypeSMS(ShipTypeSMS entity)
        {
            ObjectFactory<ShipTypeSMSService>.Instance.Create(entity);
        }
        [WebInvoke(UriTemplate = "/ShipTypeSMS/Update", Method = "PUT")]
        public void UpdateShipTypeSMS(ShipTypeSMS entity)
        {
            ObjectFactory<ShipTypeSMSService>.Instance.Update(entity);
        }
    }
}
