using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.SO;
using ECCentral.Service.SO.AppService;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;
using ECCentral.Service.SO.Restful.RequestMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.SO.Restful
{
    public partial class SOService
    {
        /// <summary>
        /// 处理从SellerPortal发过来的SSB消息
        /// </summary>
        /// <param name="reqMsg"></param>
        [WebInvoke(UriTemplate = "/SO/ProcessSellerPortalMessage", Method = "PUT")]
        public void ProcessSellerPortalMessage(RequestMessage reqMsg)
        {
            ObjectFactory<SO4SellerPortalAppService>.Instance.ProcessSellerPortalMessage(reqMsg);
        }
    }
}
