using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {
        [WebInvoke(UriTemplate = "/Vendor/GetVendorPayTermsList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<VendorPayTermsItemInfo> QueryVendorPayTermsList(string companyCode)
        {
            return ObjectFactory<IVendorPayTermsDA>.Instance.GetAllVendorPayTerms(companyCode);
        }
    }
}
