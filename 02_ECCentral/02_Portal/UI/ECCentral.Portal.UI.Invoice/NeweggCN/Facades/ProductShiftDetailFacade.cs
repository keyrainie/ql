using System;
using System.Linq;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Service.Invoice.Restful.NeweggCN.ResponseMsg;
using ECCentral.BizEntity.Invoice;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Facades
{
    public class ProductShiftDetailFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        /// <summary>
        /// InvoiceService服务基地址
        /// </summary>
        private string InvoiceServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Invoice", "ServiceBaseUrl");
            }
        }

        public ProductShiftDetailFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(InvoiceServiceBaseUrl, page);
        }

        public void Query(ProductShiftDetailReportQueryFilter query, EventHandler<RestClientEventArgs<ProductShiftDetailResp>> callback)
        {
            restClient.Query<ProductShiftDetailResp>("/InvoiceService/Invoice/QueryProductShiftDetail", query, callback);
        }

        public void ExportQuery(ProductShiftDetailReportQueryFilter query, ColumnSet[] columnSet)
        {
            restClient.ExportFile("/InvoiceService/Invoice/ExportProductShiftDetail", query, columnSet);
        }

        public void CreateProductShiftDetails(List<ProductShiftDetailQueryEntity> entity, EventHandler<RestClientEventArgs<int>> callback)
        {
            restClient.Update("/InvoiceService/Invoice/CreateProductShiftDetail", entity, callback);
        }

        public void ImportProductShiftDetail(string serverFilePath, EventHandler<RestClientEventArgs<bool>> callback)
        {
            restClient.Update("/InvoiceService/Invoice/ImportProductShiftDetail", serverFilePath, callback);
        }
    }
}
