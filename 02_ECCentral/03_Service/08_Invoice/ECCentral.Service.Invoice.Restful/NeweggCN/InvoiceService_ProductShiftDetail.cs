using System.ServiceModel.Web;

using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.Invoice.Restful.NeweggCN.ResponseMsg;
using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;
using System.Data;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.AppService;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        [WebInvoke(UriTemplate = "/Invoice/QueryProductShiftDetail", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public ProductShiftDetailResp QueryProductShiftDetail(ProductShiftDetailReportQueryFilter filter)
        {
            int totalCount = 0;
            ProductShiftDetail needManualItem = null;
            ProductShiftDetailAmtInfo outAmt = null,inAmt = null;
            var list = ObjectFactory<IProductShiftDetailQueryDA>.Instance.Query(filter, out totalCount,ref outAmt,ref inAmt,ref needManualItem);
            return new ProductShiftDetailResp() { Data = list, TotalCount = totalCount, OutAmt = outAmt, InAmt = inAmt, NeedManualItem = needManualItem };
        }

        [WebInvoke(UriTemplate = "/Invoice/ExportProductShiftDetail", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult ExportProductShiftDetail(ProductShiftDetailReportQueryFilter filter)
        {
            int totalCount = 0;
            ProductShiftDetail needManualItem = null;
            ProductShiftDetailAmtInfo outAmt = null, inAmt = null;
            var list = ObjectFactory<IProductShiftDetailQueryDA>.Instance.Query(filter, out totalCount, ref outAmt, ref inAmt, ref needManualItem);
            DataTable dt = new DataTable();

            var propertys = typeof(ProductShiftDetail).GetProperties();
            foreach (var property in propertys)
            {
                dt.Columns.Add(property.Name);
            }

            foreach (var data in list)
            {
                var newRow = dt.NewRow();
                foreach (var property in propertys)
                {
                    newRow[property.Name] = property.GetValue(data, null);
                }
                dt.Rows.Add(newRow);
            }

            return new QueryResult()
            {
                TotalCount = totalCount,
                Data = dt
            };
        }

        [WebInvoke(UriTemplate = "/Invoice/CreateProductShiftDetail", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public int CreateProductShiftDetail(List<ProductShiftDetailQueryEntity> entity)
        {
            return ObjectFactory<ProductShiftDetailAppService>.Instance.CreateProductShiftDetail(entity);
        }

        [WebInvoke(UriTemplate = "/Invoice/ImportProductShiftDetail", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public bool ImportProductShiftDetail(string serverFilePath)
        {
            return ObjectFactory<ProductShiftDetailAppService>.Instance.ImportProductShiftDetail(serverFilePath);
        }
    }
}
