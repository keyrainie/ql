using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        [WebInvoke(UriTemplate = "/ProductShiftDetail/CreateProductShiftDetail", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public void CreateProductShiftDetail(List<ProductShiftDetailQueryEntity> entity)
        {
            ObjectFactory<ProductShiftDetailAppService>.Instance.CreateProductShiftDetail(entity);
        }

        [WebInvoke(UriTemplate = "/ProductShiftDetail/ImportProductShiftDetail", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]        
        public bool ImportProductShiftDetail(string serverFilePath)
        {
            return ObjectFactory<ProductShiftDetailAppService>.Instance.ImportProductShiftDetail(serverFilePath);
        }
    }
}