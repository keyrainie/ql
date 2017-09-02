using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.RMA;
using ECCentral.QueryFilter.RMA;
using ECCentral.Service.RMA.AppService;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.RMA.Restful.ResponseMsg;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.RMA.Restful
{
    public partial class RMAService
    {
        [WebInvoke(UriTemplate = "/RMA/ProcessSellerPortalMessage", Method = "PUT")]
        public void ProcessSellerPortalMessage(RequestMessage reqMsg)
        {
            ObjectFactory<SellerRMAAppService>.Instance.ProcessSellerPortalMessage(reqMsg);
        }
    }
}
