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
        [WebInvoke(UriTemplate = "/ShipTypeSMSTemplate/Query", Method = "POST")]
        public QueryResult QueryShipTypeSMSTemplate(ShipTypeSMSTemplateQueryFilter query)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<IShipTypeSMSTemplateQueryDA>.Instance.Query(query, out totalCount),
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/ShipTypeSMSTemplate/Create", Method = "POST")]
        public void CreateShipTypeSMSTemplate(SMSTemplate entity)
        {
            ObjectFactory<SMSTemplateService>.Instance.Create(entity);
        }
        [WebInvoke(UriTemplate = "/ShipTypeSMSTemplate/Update", Method = "PUT")]
        public void UpdateShipTypeSMSTemplate(SMSTemplate query)
        {
            ObjectFactory<SMSTemplateService>.Instance.Update(query);
        }
    }
}
